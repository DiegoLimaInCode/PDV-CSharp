using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;

namespace PDVCSharp.Application.Services;

public class CaixaService
{
    private readonly AppDbContext _context;

    public CaixaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CaixaSessao> AbrirCaixaAsync(string loginOperador, decimal valorAbertura)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => !u.IsDeleted && u.Login == loginOperador)
            ?? throw new DomainException("Nenhum usuário encontrado para registrar a abertura de caixa.");

        var sessoesAbertas = await _context.CaixaSessoes.Where(c => c.IsOpen).ToListAsync();
        foreach (var sessao in sessoesAbertas)
        {
            sessao.IsOpen = false;
        }

        var novaSessao = new CaixaSessao
        {
            ValorAbertura = valorAbertura,
            DataHoraAbertura = DateTime.Now,
            IsOpen = true,
            UsuarioId = usuario.Id
        };

        _context.CaixaSessoes.Add(novaSessao);
        _context.MovimentosCaixa.Add(new MovimentoCaixa
        {
            CaixaSessaoId = novaSessao.Id,
            Tipo = TipoMovimentoCaixa.Entrada,
            Origem = OrigemMovimentoCaixa.Abertura,
            Valor = valorAbertura,
            DataHora = DateTime.Now,
            Observacao = "Abertura de caixa",
            LoginOperador = usuario.Login
        });

        await _context.SaveChangesAsync();
        return novaSessao;
    }
}
