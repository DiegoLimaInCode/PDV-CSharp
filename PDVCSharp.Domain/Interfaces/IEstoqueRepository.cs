using PDVCSharp.Domain.Entities;

namespace PDVCSharp.Domain.Interfaces
{
    public interface IEstoqueRepository
    {
        Task RegistrarEntrada(Guid produtoId, double quantidade, string motivo);
        Task RegistrarSaida(Guid produtoId, double quantidade, string motivo);
        Task<IEnumerable<MovimentacaoEstoque>> ObterHistorico(Guid produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterHistoricoGeral();
        Task<IEnumerable<Produto>> ObterProdutosEstoqueBaixo(double estoqueMinimo);
    }
}
