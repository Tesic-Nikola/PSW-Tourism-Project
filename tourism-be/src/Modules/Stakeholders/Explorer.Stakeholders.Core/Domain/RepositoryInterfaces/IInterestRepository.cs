using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IInterestRepository
{
    Interest? GetByName(string name);
    Interest Create(Interest interest);
    PersonInterest CreatePersonInterest(PersonInterest personInterest);
    void SaveChanges();
}