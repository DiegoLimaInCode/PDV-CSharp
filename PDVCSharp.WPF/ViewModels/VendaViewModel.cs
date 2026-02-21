using PDVCSharp.WPF.Contexts;

namespace PDVCSharp.WPF.ViewModels;

// ViewModel da tela de Venda — exibe o nome do operador na tela de vendas
public sealed class VendaViewModel : BaseViewModel
{
    public VendaViewModel()
    {
        // Escuta mudanças no nome do operador para atualizar a tela
        if (Master.Usuario != null)
        {
            Master.Usuario.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SessaoUsuario.OperatorName))
                    OnPropertyChanged(nameof(OperatorName));
            };
        }
    }

    // Nome do operador para exibição na tela de venda
    public string OperatorName
        => string.IsNullOrWhiteSpace(Master.Usuario.OperatorName)
            ? "Operador"
            : Master.Usuario.OperatorName;
}