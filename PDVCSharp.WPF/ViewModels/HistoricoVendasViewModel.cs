using System.Collections.ObjectModel;
using PDVCSharp.Application.Services;
using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;

namespace PDVCSharp.WPF.ViewModels;

public sealed class HistoricoVendasViewModel : BaseViewModel
{
    private readonly HistoricoVendaService _historicoVendaService;
    private decimal _total;
    private int _quantidade;

    public HistoricoVendasViewModel(HistoricoVendaService historicoVendaService)
    {
        _historicoVendaService = historicoVendaService;
        Vendas = new ObservableCollection<Venda>();
    }

    public ObservableCollection<Venda> Vendas { get; }

    public decimal Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(); }
    }

    public int Quantidade
    {
        get => _quantidade;
        set { _quantidade = value; OnPropertyChanged(); }
    }

    public async Task CarregarAsync()
    {
        Vendas.Clear();
        var caixaId = Master.Caixa?.CaixaSessaoId;
        if (caixaId is null || caixaId == Guid.Empty)
        {
            Total = 0;
            Quantidade = 0;
            return;
        }

        var vendas = await _historicoVendaService.ObterPorCaixaAsync(caixaId.Value);
        foreach (var venda in vendas)
        {
            Vendas.Add(venda);
        }

        Quantidade = vendas.Count;
        Total = vendas.Sum(v => v.Total);
    }
}
