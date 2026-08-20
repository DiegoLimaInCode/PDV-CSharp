using System.Collections.ObjectModel;
using System.Windows.Input;
using PDVCSharp.Application.Services;
using PDVCSharp.WPF.Commands;
using PDVCSharp.WPF.Contexts;
using PDVCSharp.WPF.Models;

namespace PDVCSharp.WPF.ViewModels;

public sealed class VendaViewModel : BaseViewModel
{
    private readonly CatalogoService _catalogoService;
    private string _busca = string.Empty;
    private string _subtotalTexto = "R$ 0,00";
    private string _totalTexto = "R$ 0,00";

    public VendaViewModel(CatalogoService catalogoService)
    {
        _catalogoService = catalogoService;
        Catalogo = new ObservableCollection<ProdutoVenda>();
        Carrinho = new ObservableCollection<ProdutoVenda>();
        Carrinho.CollectionChanged += (_, _) => AtualizarTotais();
        AdicionarCommand = new RelayCommand(p => AdicionarAoCarrinho(p as ProdutoVenda));
        RemoverCommand = new RelayCommand(p => RemoverDoCarrinho(p as ProdutoVenda));
        AumentarCommand = new RelayCommand(p => AlterarQuantidade(p as ProdutoVenda, 1));
        DiminuirCommand = new RelayCommand(p => AlterarQuantidade(p as ProdutoVenda, -1));
    }

    public ObservableCollection<ProdutoVenda> Catalogo { get; }
    public ObservableCollection<ProdutoVenda> Carrinho { get; }

    public string OperatorName
        => string.IsNullOrWhiteSpace(Master.Usuario?.OperatorName) ? "Operador" : Master.Usuario.OperatorName;

    public string Busca
    {
        get => _busca;
        set
        {
            _busca = value;
            OnPropertyChanged();
        }
    }

    public string SubtotalTexto
    {
        get => _subtotalTexto;
        set { _subtotalTexto = value; OnPropertyChanged(); }
    }

    public string TotalTexto
    {
        get => _totalTexto;
        set { _totalTexto = value; OnPropertyChanged(); }
    }

    public ICommand AdicionarCommand { get; }
    public ICommand RemoverCommand { get; }
    public ICommand AumentarCommand { get; }
    public ICommand DiminuirCommand { get; }

    public async Task CarregarCatalogoAsync()
    {
        var produtos = await _catalogoService.BuscarAsync(Busca);
        Catalogo.Clear();
        foreach (var produto in produtos)
        {
            Catalogo.Add(new ProdutoVenda
            {
                Id = produto.Id,
                Name = produto.Name,
                Sku = produto.Sku,
                Categoria = produto.Categoria,
                Price = produto.Price,
                Quantity = 1,
                EstoqueDisponivel = produto.Quantity,
                ImagePath = produto.ImagePath ?? string.Empty
            });
        }
    }

    public void LimparCarrinho()
    {
        Carrinho.Clear();
        AtualizarTotais();
    }

    public void AdicionarAoCarrinho(ProdutoVenda? produto)
    {
        if (produto is null) return;

        var existente = Carrinho.FirstOrDefault(p => p.Id == produto.Id);
        if (existente != null)
        {
            existente.Quantity += 1;
        }
        else
        {
            Carrinho.Add(new ProdutoVenda
            {
                Id = produto.Id,
                Name = produto.Name,
                Sku = produto.Sku,
                Categoria = produto.Categoria,
                Price = produto.Price,
                Quantity = 1,
                EstoqueDisponivel = produto.EstoqueDisponivel,
                ImagePath = produto.ImagePath
            });
        }

        AtualizarTotais();
    }

    private void RemoverDoCarrinho(ProdutoVenda? produto)
    {
        if (produto is null) return;
        Carrinho.Remove(produto);
        AtualizarTotais();
    }

    private void AlterarQuantidade(ProdutoVenda? produto, int delta)
    {
        if (produto is null) return;
        var nova = produto.Quantity + delta;
        if (nova <= 0)
        {
            Carrinho.Remove(produto);
        }
        else
        {
            produto.Quantity = nova;
        }
        AtualizarTotais();
    }

    public void AtualizarTotais()
    {
        var subtotal = Carrinho.Sum(p => p.Total);
        SubtotalTexto = $"R$ {subtotal:F2}";
        TotalTexto = $"R$ {subtotal:F2}";
    }
}
