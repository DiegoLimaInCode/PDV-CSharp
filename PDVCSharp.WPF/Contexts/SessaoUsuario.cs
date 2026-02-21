using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PDVCSharp.WPF.Contexts
{
    // Armazena os dados da sessão do usuário logado
    // Implementa notificação de mudança para que a tela atualize automaticamente
    //
    // 💡 DICA: PropertyChanged é o mecanismo que o WPF usa para saber quando
    //    um dado mudou e atualizar a tela automaticamente (Data Binding).
    public class SessaoUsuario
    {
        private string _operatorName = "";

        // Nome do operador logado (ex: "admin")
        public string OperatorName
        {
            get => _operatorName;
            set
            {
                if (_operatorName == value) return; // Não notifica se o valor não mudou
                _operatorName = value;
                OnPropertyChanged(); // Notifica a tela que o valor mudou
            }
        }

        // Evento que a tela "escuta" para saber quando atualizar
        public event PropertyChangedEventHandler? PropertyChanged;

        // Dispara o evento PropertyChanged
        // [CallerMemberName] = preenche automaticamente com o nome da propriedade que chamou
        // 💡 DICA: Sem [CallerMemberName], você teria que escrever OnPropertyChanged("OperatorName")
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}