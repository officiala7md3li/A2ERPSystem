namespace DomainDrivenERP.Persistence.Entities;

/// <summary>
/// Tracks the last-issued sequence number per document-type prefix, company, and day.
/// Used by the SequenceStore for DB-level concurrency-safe increments.
/// </summary>
public sealed class SequenceCounter
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Prefix { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public DateTime SequenceDate { get; private set; }
    public long CounterValue { get; private set; }

    private SequenceCounter() { }

    public static SequenceCounter Create(string prefix, Guid companyId, DateTime date, long initialValue = 1)
    {
        return new SequenceCounter
        {
            Prefix = prefix,
            CompanyId = companyId,
            SequenceDate = date.Date,
            CounterValue = initialValue
        };
    }

    public void Increment() => CounterValue++;
    public void SetValue(long value) => CounterValue = value;
}
