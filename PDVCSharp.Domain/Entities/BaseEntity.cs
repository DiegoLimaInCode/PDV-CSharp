// Importa tipos básicos do .NET (Guid, DateTime, etc.)
using System;
using System.Collections.Generic;
using System.Text;

// Namespace = "pasta lógica" que organiza as classes do projeto
// Aqui estamos na camada Domain > Entities (onde ficam os modelos de dados)
namespace PDVCSharp.Domain.Entities
{
    // BaseEntity é a classe "pai" que todas as entidades do banco herdam.
    // Isso evita repetir Id, CreatedAt, etc. em cada entidade.
    // 💡 DICA: Esse padrão se chama "classe base" ou "entidade abstrata".
    public class BaseEntity
    {
        // Guid = identificador único universal (ex: "a1b2c3d4-e5f6-...")
        // 💡 DICA: Guid é melhor que int para IDs quando você precisa gerar IDs
        //    sem depender do banco de dados (ex: sistemas distribuídos).
        public Guid Id { get; set; }

        // Data/hora em que o registro foi criado
        public DateTime CreatedAt { get; set; }

        // Data/hora da última atualização do registro
        public DateTime UpdatedAt { get; set; }

        // Flag de "soft delete" — em vez de apagar do banco, marca como deletado
        // 💡 DICA: Soft delete permite "recuperar" dados deletados por engano.
        public bool IsDeleted { get; set; }

        // Construtor: é chamado automaticamente quando você faz "new BaseEntity()"
        // Aqui os valores padrão são definidos para toda entidade nova
        public BaseEntity()
        {
            Id = Guid.NewGuid();       // Gera um ID único automaticamente
            CreatedAt = DateTime.UtcNow; // Hora atual em UTC (horário universal)
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;           // Começa como "não deletado"
        }

        // Método que marca a entidade como deletada (soft delete)
        // Em vez de remover do banco, apenas muda a flag IsDeleted para true
        public void DeleteMethod()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow; // Atualiza a data de modificação
        }
    }
}
