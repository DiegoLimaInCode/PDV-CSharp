using PDVCSharp.WPF.Contexts;

namespace PDVCSharp.WPF.ViewModels;

// ViewModel da tela de Abertura de Caixa
// Expõe o nome do operador logado para a tela via Data Binding
public sealed class AperturaViewModel : BaseViewModel
{
    public AperturaViewModel()
    {
        // Se há um usuário logado, escuta mudanças no nome do operador
        if (Master.Usuario != null)
        {
            // "+= " = inscreve-se no evento PropertyChanged do usuário
            Master.Usuario.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Master.Usuario.OperatorName))
                    OnPropertyChanged(nameof(OperatorName)); // Notifica a tela
            };
        }
    }

    // Propriedade que a tela usa via binding para mostrar o nome do operador
    // 💡 DICA: "=>" (expression body) = forma curta de propriedade somente-leitura
    public string OperatorName
        => string.IsNullOrWhiteSpace(Master.Usuario?.OperatorName)
            ? "Operador"
            : Master.Usuario.OperatorName;
}