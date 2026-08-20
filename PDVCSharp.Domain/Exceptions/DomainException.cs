namespace PDVCSharp.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

public class EstoqueInsuficienteException : DomainException
{
    public EstoqueInsuficienteException(string message) : base(message)
    {
    }
}

public class PagamentoInsuficienteException : DomainException
{
    public PagamentoInsuficienteException(string message) : base(message)
    {
    }
}

public class AutenticacaoException : DomainException
{
    public AutenticacaoException(string message) : base(message)
    {
    }
}
