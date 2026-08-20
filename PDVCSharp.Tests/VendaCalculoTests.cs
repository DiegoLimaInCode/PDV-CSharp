using PDVCSharp.Domain.Entities;

namespace PDVCSharp.Tests;

public class VendaCalculoTests
{
    [Fact]
    public void ClienteComum_NaoRecebeDesconto()
    {
        var venda = new Venda
        {
            SubTotal = 100m,
            TipoCliente = TipoCliente.Comum
        };

        venda.Calcular();

        Assert.Equal(0m, venda.DescontoAplicado);
        Assert.Equal(100m, venda.Total);
    }

    [Fact]
    public void ClientePremium_RecebeDezPorCento()
    {
        var venda = new Venda
        {
            SubTotal = 100m,
            TipoCliente = TipoCliente.Premium
        };

        venda.Calcular();

        Assert.Equal(10m, venda.DescontoAplicado);
        Assert.Equal(90m, venda.Total);
    }
}
