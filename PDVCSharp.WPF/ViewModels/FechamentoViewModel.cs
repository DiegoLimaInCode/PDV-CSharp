using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PDVCSharp.WPF.ViewModels;

public sealed class FechamentoViewModel : INotifyPropertyChanged
{
    private decimal _totalCaixa;

    public FechamentoViewModel()
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

    public DateTime Today => DateTime.Today;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
