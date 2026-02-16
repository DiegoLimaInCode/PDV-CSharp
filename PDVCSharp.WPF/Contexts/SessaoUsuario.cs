using PDVCSharp.Domain.Entities;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

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

        public static bool Login(string usuario, string senha)
        {
            var usuarios = JsonSerializer.Deserialize<ICollection<Usuario>>(File.ReadAllText("Usuarios.json"));

            if (usuarios is null || usuarios.FirstOrDefault(x => x.Login.ToLower() == usuario.ToLower()) is not Usuario user)
            {
                throw new Exception("Usuário não localizado");
            }

            if (user.Password != senha)
            {
                throw new Exception("Senha incorreta");
            }

            return true;
        }
    }

}