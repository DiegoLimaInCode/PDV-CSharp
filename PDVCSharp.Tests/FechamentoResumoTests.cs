using PDVCSharp.Domain.Entities;

namespace PDVCSharp.Tests;

public class FechamentoResumoTests
{
    [Fact]
    public void SaldoFinal_ConsideraDinheiroSuprimentoESangria()
    {
        var resumo = new FechamentoResumo
        {
            ValorAbertura = 200m,
            TotalDinheiro = 80m,
            TotalSuprimentos = 20m,
            TotalSangrias = 30m,
            TotalVendas = 150m
        };

        Assert.Equal(270m, resumo.SaldoFinal);
    }
}
