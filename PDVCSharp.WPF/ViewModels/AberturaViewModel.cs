using PDVCSharp.WPF.Contexts;

namespace PDVCSharp.WPF.ViewModels;

// ViewModel da tela de Abertura de Caixa
// Expõe o nome do operador logado para a tela via Data Binding
public sealed class AperturaViewModel : BaseViewModel
{
    public AperturaViewModel()
    {
        // Se há um usuário logado, escuta mudanças no nome do operador
        // e notifica a tela para atualizar o binding
        if (Master.Usuario != null)
        {
            // "+=" = inscreve-se no evento PropertyChanged do usuário
            // "(_, e) =>" = lambda com dois parâmetros (sender ignorado, EventArgs usado)
            Master.Usuario.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Master.Usuario.OperatorName))
                    OnPropertyChanged(nameof(OperatorName)); // Notifica a tela
            };
        }
    }

    // Propriedade que a tela usa via binding para mostrar o nome do operador
    // Se não há operador logado, mostra "Operador" como padrão
    // 💡 DICA: "=>" (expression body) é uma forma curta de escrever propriedades somente-leitura
    public string OperatorName
        => string.IsNullOrWhiteSpace(Master.Usuario?.OperatorName)
            ? "Operador"
            : Master.Usuario.OperatorName;
}