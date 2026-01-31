namespace LoanFlow.Domain.Entities;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool Deleted { get; set; }
}
