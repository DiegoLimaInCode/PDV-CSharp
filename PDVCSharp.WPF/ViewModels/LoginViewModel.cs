namespace PDVCSharp.WPF.ViewModels;

// ViewModel da tela de Login — herda de BaseViewModel (tem relógio, PropertyChanged, etc.)
// "sealed" = esta classe NÃO pode ser herdada por outras classes
public sealed class LoginViewModel : BaseViewModel
{
    // Por enquanto vazia — lógica está no code-behind (Login.xaml.cs)
    // 💡 DICA: Idealmente, a lógica de login deveria estar aqui (padrão MVVM completo)
}