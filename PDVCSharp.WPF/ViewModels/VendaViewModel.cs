using PDVCSharp.WPF.Contexts;

namespace PDVCSharp.WPF.ViewModels;

public sealed class VendaViewModel : BaseViewModel
{
    public VendaViewModel()
    {
        if (Master.Usuario != null)
        {
            Master.Usuario.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SessaoUsuario.OperatorName))
                    OnPropertyChanged(nameof(OperatorName));
            };
        }
    }

    public string OperatorName
        => Master.Usuario != null && !string.IsNullOrWhiteSpace(Master.Usuario.OperatorName)
            ? Master.Usuario.OperatorName
            : "Operador";
}