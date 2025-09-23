using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class InterestRepository : IInterestRepository
{
    private readonly StakeholdersContext _context;

    public InterestRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public Interest? GetByName(string name)
    {
        return _context.Interests.FirstOrDefault(i => i.Name == name);
    }

    public Interest Create(Interest interest)
    {
        _context.Interests.Add(interest);
        return interest;
    }

    public PersonInterest CreatePersonInterest(PersonInterest personInterest)
    {
        _context.PersonInterests.Add(personInterest);
        return personInterest;
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}