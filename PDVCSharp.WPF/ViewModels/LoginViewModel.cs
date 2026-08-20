using System.Windows.Input;
using PDVCSharp.Application.Services;
using PDVCSharp.WPF.Commands;
using PDVCSharp.WPF.Contexts;
using PDVCSharp.WPF.Navigation;

namespace PDVCSharp.WPF.ViewModels;

public sealed class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private string _usuario = string.Empty;
    private string _erro = string.Empty;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        EntrarCommand = new RelayCommand(async _ => await EntrarAsync());
    }

    public string Usuario
    {
        get => _usuario;
        set
        {
            _usuario = value;
            OnPropertyChanged();
        }
    }

    public string Erro
    {
        get => _erro;
        set
        {
            _erro = value;
            OnPropertyChanged();
        }
    }

    public ICommand EntrarCommand { get; }

    public async Task EntrarAsync(string? senha = null)
    {
        try
        {
            Erro = string.Empty;
            var user = await _authService.Login(Usuario, senha ?? string.Empty);
            Master.Usuario = new SessaoUsuario
            {
                OperatorName = user.Login,
                Cargo = user.Cargo
            };

            if (Master.Caixa is null)
            {
                MainWindow.Navigation.Navigate(AppScreen.Abertura);
            }
            else if (Master.Venda != null)
            {
                MainWindow.Navigation.Navigate(AppScreen.Venda);
            }
            else
            {
                MainWindow.Navigation.Navigate(AppScreen.CaixaLivre);
            }
        }
        catch (Exception ex)
        {
            Erro = ex.Message;
        }
    }
}
