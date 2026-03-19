using PDVCSharp.Application.Services;
using PDVCSharp.WPF.Contexts;

namespace PDVCSharp.WPF.ViewModels
{
    public sealed class FechamentoViewModel : BaseViewModel
    {
        private readonly FechamentoService _fechamentoService;

        private decimal _valorAbertura;
        private decimal _totalVendas;
        private int _quantidadeVendas;
        private decimal _totalDinheiro;
        private decimal _totalCartaoCredito;
        private decimal _totalCartaoDebito;
        private decimal _totalCheque;
        private decimal _totalCaixal;

        public FechamentoViewModel(FechamentoService fechamentoService)
        {
            _fechamentoService = fechamentoService;

            if (Master.Usuario != null)
            {
                Master.Usuario.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Master.Usuario.OperatorName))
                        OnPropertyChanged(nameof(OperatorName));
                };
            }
        }

        public string OperatorName =>
            string.IsNullOrWhiteSpace(Master.Usuario?.OperatorName)
                ? "Operador"
                : Master.Usuario.OperatorName;

        public decimal ValorAbertura
        {
            get => _valorAbertura;
            set
            {
                _valorAbertura = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalVendas
        {
            get => _totalVendas;
            set
            {
                _totalVendas = value;
                OnPropertyChanged();
            }
        }

        public int QuantidadeVendas
        {
            get => _quantidadeVendas;
            set
            {
                _quantidadeVendas = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalDinheiro
        {
            get => _totalDinheiro;
            set
            {
                _totalDinheiro = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalCartaoCredito
        {
            get => _totalCartaoCredito;
            set
            {
                _totalCartaoCredito = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalCartaoDebito
        {
            get => _totalCartaoDebito;
            set
            {
                _totalCartaoDebito = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalCheque
        {
            get => _totalCheque;
            set
            {
                _totalCheque = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalCaixal
        {
            get => _totalCaixal;
            set
            {
                _totalCaixal = value;
                OnPropertyChanged();
            }
        }

        public async Task CarregarAsync()
        {
            var caixaSessaoId = Master.Caixa?.CaixaSessaoId;
            if (caixaSessaoId is null || caixaSessaoId == Guid.Empty)
            {
                ValorAbertura = 0;
                TotalVendas = 0;
                QuantidadeVendas = 0;
                TotalDinheiro = 0;
                TotalCartaoCredito = 0;
                TotalCartaoDebito = 0;
                TotalCheque = 0;
                TotalCaixal = 0;
                return;
            }

            var resumo = await _fechamentoService.ObterResumoAsync(caixaSessaoId.Value);

            ValorAbertura = resumo.ValorAbertura;
            TotalVendas = resumo.TotalVendas;
            QuantidadeVendas = resumo.QuantidadeVendas;
            TotalDinheiro = resumo.TotalDinheiro;
            TotalCartaoCredito = resumo.TotalCartaoCredito;
            TotalCartaoDebito = resumo.TotalCartaoDebito;
            TotalCheque = resumo.TotalCheque;
            TotalCaixal = resumo.TotalCaixa;
        }

        public Task<bool> FecharCaixaAsync()
        {
            var caixaSessaoId = Master.Caixa?.CaixaSessaoId;
            if (caixaSessaoId is null || caixaSessaoId == Guid.Empty)
            {
                return Task.FromResult(false);
            }

            return _fechamentoService.FecharCaixaAsync(caixaSessaoId.Value);
        }

        public Task<bool> ExisteCaixaAbertoAsync()
        {
            var caixaSessaoId = Master.Caixa?.CaixaSessaoId;
            if (caixaSessaoId is null || caixaSessaoId == Guid.Empty)
            {
                return Task.FromResult(false);
            }

            return _fechamentoService.ExisteCaixaAbertoAsync(caixaSessaoId.Value);
        }
    }
}