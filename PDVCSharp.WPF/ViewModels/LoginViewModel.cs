namespace PDVCSharp.WPF.ViewModels;

// ViewModel da tela de Login — herda de BaseViewModel (tem relógio, PropertyChanged, etc.)
// "sealed" = esta classe NÃO pode ser herdada por outras classes
// 💡 DICA: Use "sealed" quando você sabe que nenhuma outra classe vai herdar desta.
//    Isso melhora a performance e deixa a intenção clara.
public sealed class LoginViewModel : BaseViewModel
{
    // Por enquanto está vazia — toda lógica está no code-behind (Login.xaml.cs)
    // 💡 DICA: Idealmente, a lógica de login deveria estar aqui (no ViewModel),
    //    e o code-behind ficaria vazio. Isso é o padrão MVVM completo.
}