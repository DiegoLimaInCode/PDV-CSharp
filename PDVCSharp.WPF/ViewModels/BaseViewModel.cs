using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace PDVCSharp.WPF.ViewModels;

// Classe base para todos os ViewModels do app
// "abstract" = não pode ser instanciada diretamente, só herdada (LoginViewModel, VendaViewModel, etc.)
//
// 💡 DICA: ViewModel é o padrão MVVM (Model-View-ViewModel):
//    - Model = dados (entidades como Produto, Usuario)
//    - View = tela (XAML)
//    - ViewModel = ponte entre dados e tela (lógica de apresentação)
//
// INotifyPropertyChanged = interface que permite a tela saber quando um dado mudou
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private DateTime _now;
    private DispatcherTimer _timer;

    protected BaseViewModel()
    {
        InitializeTimer(); // Inicia o relógio ao criar qualquer ViewModel
    }

    // Configura um timer que atualiza a propriedade "Now" a cada segundo
    // Assim qualquer tela que fizer binding com "Now" terá o horário atualizado
    private void InitializeTimer()
    {
        _now = DateTime.Now;
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        // "(_, _) =>" é uma lambda que ignora ambos os parâmetros (sender e EventArgs)
        _timer.Tick += (_, _) => Now = DateTime.Now;
        _timer.Start();
    }

    // Propriedade que contém a data/hora atual (atualizada a cada segundo)
    public DateTime Now
    {
        get => _now;
        set
        {
            if (_now == value) return;
            _now = value;
            OnPropertyChanged(); // Notifica a tela que o horário mudou
        }
    }

    // Propriedade que retorna a data de hoje (sem horário)
    public DateTime Today => DateTime.Today;

    // Evento que a tela escuta para atualizar os bindings
    public event PropertyChangedEventHandler? PropertyChanged;

    // Método que dispara o evento PropertyChanged
    // 💡 DICA: "protected" = acessível nesta classe e nas filhas (LoginViewModel, etc.)
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}