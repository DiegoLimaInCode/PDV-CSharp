using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;

namespace PDVCSharp.Application.Services;

public class FechamentoService
{
    private readonly AppDbContext _context;

    public FechamentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FechamentoResumo> ObterResumoAsync(Guid caixaSessaoId)
    {
        var caixaSessao = await _context.CaixaSessoes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == caixaSessaoId);

        if (caixaSessao is null)
        {
            return new FechamentoResumo();
        }

        var vendas = await _context.Vendas
            .AsNoTracking()
            .Where(v => !v.IsDeleted && v.CaixaSessaoId == caixaSessaoId)
            .ToListAsync();

        var movimentos = await _context.MovimentosCaixa
            .AsNoTracking()
            .Where(m => !m.IsDeleted && m.CaixaSessaoId == caixaSessaoId)
            .ToListAsync();

        return new FechamentoResumo
        {
            ValorAbertura = caixaSessao.ValorAbertura,
            QuantidadeVendas = vendas.Count,
            TotalVendas = vendas.Sum(v => v.Total),
            TotalDinheiro = vendas
                .Where(v => v.FormaPagamento is FormaPagamento.Dinheiro or FormaPagamento.Caixa)
                .Sum(v => v.Total),
            TotalCartaoCredito = vendas.Where(v => v.FormaPagamento == FormaPagamento.Credito).Sum(v => v.Total),
            TotalCartaoDebito = vendas.Where(v => v.FormaPagamento == FormaPagamento.Debito).Sum(v => v.Total),
            TotalCheque = vendas.Where(v => v.FormaPagamento == FormaPagamento.Cheque).Sum(v => v.Total),
            TotalPix = vendas.Where(v => v.FormaPagamento == FormaPagamento.Pix).Sum(v => v.Total),
            TotalSuprimentos = movimentos
                .Where(m => m.Origem == OrigemMovimentoCaixa.Suprimento)
                .Sum(m => m.Valor),
            TotalSangrias = movimentos
                .Where(m => m.Origem == OrigemMovimentoCaixa.Sangria)
                .Sum(m => m.Valor),
            TotalCaixa = caixaSessao.ValorAbertura
                + vendas.Where(v => v.FormaPagamento is FormaPagamento.Dinheiro or FormaPagamento.Caixa).Sum(v => v.Total)
                + movimentos.Where(m => m.Origem == OrigemMovimentoCaixa.Suprimento).Sum(m => m.Valor)
                - movimentos.Where(m => m.Origem == OrigemMovimentoCaixa.Sangria).Sum(m => m.Valor)
        };
    }

    public async Task<bool> FecharCaixaAsync(Guid caixaSessaoId)
    {
        var caixaSessao = await _context.CaixaSessoes
            .FirstOrDefaultAsync(c => c.Id == caixaSessaoId);

        if (caixaSessao is null || !caixaSessao.IsOpen)
        {
            return false;
        }

        caixaSessao.IsOpen = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExisteCaixaAbertoAsync(Guid caixaSessaoId)
    {
        return _context.CaixaSessoes.AnyAsync(c => c.Id == caixaSessaoId && c.IsOpen);
    }
}
