using PDVCSharp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace PDVCSharp.WPF.Sections
{
    public partial class PagamentoCartaoWindow : Window
    {

        public PagamentoCartao pagamentoCartao { get; private set; }
       

        public PagamentoCartaoWindow(decimal valor)
        {
            InitializeComponent();

            pagamentoCartao = new PagamentoCartao {
                Valor = valor
            };

            DataContext = pagamentoCartao;    
        }

        public void AdicionarCartao() {

  
            pagamentoCartao = new PagamentoCartao {
                Valor = decimal.Parse(ValueBox.Text),
                Cartao = TipoCartao.Text,
                Parcela = TipoParcela.Text
            };
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {


            AdicionarCartao();


            DialogResult = true;

            
            Close();


        }

        

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
