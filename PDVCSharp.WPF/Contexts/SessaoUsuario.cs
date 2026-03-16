using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PDVCSharp.WPF.Contexts
{
    public class SessaoUsuario
    {
        private string _operatorName = "";

        public string OperatorName
        {
            get => _operatorName;
            set
            {
                if (_operatorName == value) return;
                _operatorName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}