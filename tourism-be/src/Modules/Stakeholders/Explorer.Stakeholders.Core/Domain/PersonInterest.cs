using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class PersonInterest : Entity
{
    public long PersonId { get; init; }
    public long InterestId { get; init; }

    // Navigation properties
    public Person Person { get; init; }
    public Interest Interest { get; init; }

    public PersonInterest(long personId, long interestId)
    {
        if (personId <= 0) throw new ArgumentException("Invalid PersonId");
        if (interestId == 0) throw new ArgumentException("Invalid InterestId");

        PersonId = personId;
        InterestId = interestId;
    }

    // Private constructor for EF
    private PersonInterest() { }
}