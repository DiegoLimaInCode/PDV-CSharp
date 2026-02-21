using PDVCSharp.WPF.Contexts;
using System.ComponentModel;

namespace PDVCSharp.WPF.ViewModels;

// ViewModel da tela de Fechamento de Caixa
public sealed class FechamentoViewModel : BaseViewModel
{
    // Campo privado para o total do caixa (preparado para uso futuro)
    private decimal _totalCaixa;

    public FechamentoViewModel()
    {
        // Escuta mudanças no nome do operador
        Master.Usuario.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Master.Usuario.OperatorName))
                OnPropertyChanged(nameof(OperatorName));
        };
    }

    // Nome do operador logado
    public string OperatorName
        => string.IsNullOrWhiteSpace(Master.Usuario.OperatorName)
            ? "Operador"
            : Master.Usuario.OperatorName;
}
