using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PDVCSharp.WPF;

public static class AppSession
{
    public static SessionState State { get; } = new();
    public static Dictionary<string, string> Operators { get; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Diego"] = "1234",
            ["Bernardo"] = "1234",
            ["Admin"] = "admin"
        };
    public sealed class SessionState : INotifyPropertyChanged
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
