using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PDVCSharp.WPF.Models
{
    public class ProdutoVenda : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private decimal _price;
        private double _estoqueDisponivel;
        private double _quantity;
        private string _imagePath = string.Empty;

        public Guid Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public double EstoqueDisponivel
        {
            get => _estoqueDisponivel;
            set
            {
                _estoqueDisponivel = value;
                OnPropertyChanged();
            }
        }

        public double Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        public decimal Total => Price * (decimal)Quantity;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
