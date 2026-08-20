using System.Collections.ObjectModel;
using PDVCSharp.Application.Services;
using PDVCSharp.Domain.Entities;

namespace PDVCSharp.WPF.ViewModels;

public sealed class EstoqueViewModel : BaseViewModel
{
    private readonly EstoqueService _estoqueService;

    public EstoqueViewModel(EstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
        Produtos = new ObservableCollection<Produto>();
        Movimentacoes = new ObservableCollection<MovimentacaoEstoque>();
    }

    public ObservableCollection<Produto> Produtos { get; }
    public ObservableCollection<MovimentacaoEstoque> Movimentacoes { get; }

    public async Task CarregarAsync()
    {
        Produtos.Clear();
        foreach (var produto in await _estoqueService.ListarProdutosAsync())
        {
            Produtos.Add(produto);
        }

        Movimentacoes.Clear();
        foreach (var movimento in await _estoqueService.ObterHistoricoGeralAsync())
        {
            Movimentacoes.Add(movimento);
        }
    }
}
