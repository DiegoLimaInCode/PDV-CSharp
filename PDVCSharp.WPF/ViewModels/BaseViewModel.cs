using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace PDVCSharp.WPF.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    private DateTime _now;
    private DispatcherTimer _timer;

    protected BaseViewModel()
    {
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        _now = DateTime.Now;
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => Now = DateTime.Now;
        _timer.Start();
    }

    public DateTime Now
    {
        get => _now;
        set
        {
            if (_now == value) return;
            _now = value;
            OnPropertyChanged();
        }
    }

    public DateTime Today => DateTime.Today;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}