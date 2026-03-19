using PDVCSharp.WPF.Contexts;

namespace PDVCSharp.WPF.ViewModels;

public sealed class AberturaViewModel : BaseViewModel
{
    public AberturaViewModel()
    {
        if (Master.Usuario != null)
        {
            Master.Usuario.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Master.Usuario.OperatorName))
                    OnPropertyChanged(nameof(OperatorName));
            };
        }
    }

    public string OperatorName
        => string.IsNullOrWhiteSpace(Master.Usuario?.OperatorName)
            ? "Operador"
            : Master.Usuario.OperatorName;
}