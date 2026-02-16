using PDVCSharp.WPF.Contexts;
using System.ComponentModel;

namespace PDVCSharp.WPF.ViewModels;

public sealed class FechamentoViewModel : BaseViewModel
{
    private decimal _totalCaixa;

    public FechamentoViewModel()
    {
        Master.Usuario.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Master.Usuario.OperatorName))
                OnPropertyChanged(nameof(OperatorName));
        };
    }

    public string OperatorName
        => string.IsNullOrWhiteSpace(Master.Usuario.OperatorName)
            ? "Operador"
            : Master.Usuario.OperatorName;
}
