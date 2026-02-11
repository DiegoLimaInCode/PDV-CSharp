namespace PDVCSharp.WPF.ViewModels;

public sealed class VendaViewModel : BaseViewModel
{
    public VendaViewModel()
    {
        PDVCSharp.WPF.AppSession.State.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PDVCSharp.WPF.AppSession.SessionState.OperatorName))
                OnPropertyChanged(nameof(OperatorName));
        };
    }

    public string OperatorName
        => string.IsNullOrWhiteSpace(PDVCSharp.WPF.AppSession.State.OperatorName)
            ? "Operador"
            : PDVCSharp.WPF.AppSession.State.OperatorName;
}