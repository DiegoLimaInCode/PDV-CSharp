namespace PDVCSharp.WPF.Contexts
{
    // Armazena os dados do caixa aberto
    public class SessaoCaixa
    {
        // Valor em dinheiro que o operador informou ao abrir o caixa (troco inicial)
        // 💡 DICA: "decimal" é o tipo ideal para valores monetários — evita erros de arredondamento.
        public decimal ValorAbertura { get; set; }
    }
}
