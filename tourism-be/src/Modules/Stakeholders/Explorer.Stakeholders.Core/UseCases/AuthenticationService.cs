using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using FluentResults;

namespace Explorer.Stakeholders.Core.UseCases;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly ICrudRepository<Person> _personRepository;
    private readonly IInterestRepository _interestRepository;

    private static readonly List<string> ValidInterests = new()
    {
        "priroda", "umetnost", "sport", "soping", "hrana"
    };

    public AuthenticationService(IUserRepository userRepository, ICrudRepository<Person> personRepository,
        ITokenGenerator tokenGenerator, IInterestRepository interestRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _personRepository = personRepository;
        _interestRepository = interestRepository;
    }

    public Result<AuthenticationTokensDto> Login(CredentialsDto credentials)
    {
        var user = _userRepository.GetActiveByName(credentials.Username);
        if (user == null || credentials.Password != user.Password) return Result.Fail(FailureCode.NotFound);

        long personId;
        try
        {
            personId = _userRepository.GetPersonId(user.Id);
        }
        catch (KeyNotFoundException)
        {
            personId = 0;
        }
        return _tokenGenerator.GenerateAccessToken(user, personId);
    }

    public Result<AuthenticationTokensDto> RegisterTourist(AccountRegistrationDto account)
    {
        if (_userRepository.Exists(account.Username)) return Result.Fail(FailureCode.NonUniqueUsername);

        // Validate interests
        if (account.Interests == null || !account.Interests.Any())
            return Result.Fail(FailureCode.InvalidArgument).WithError("At least one interest must be selected.");

        var invalidInterests = account.Interests.Where(i => !ValidInterests.Contains(i)).ToList();
        if (invalidInterests.Any())
            return Result.Fail(FailureCode.InvalidArgument).WithError($"Invalid interests: {string.Join(", ", invalidInterests)}");

        try
        {
            var user = _userRepository.Create(new User(account.Username, account.Password, UserRole.Tourist, true));
            var person = _personRepository.Create(new Person(user.Id, account.Name, account.Surname, account.Email));

            // Create interests if they don't exist and link them to person
            foreach (var interestName in account.Interests.Distinct())
            {
                var interest = _interestRepository.GetByName(interestName);
                if (interest == null)
                {
                    interest = new Interest(interestName);
                    _interestRepository.Create(interest);
                    _interestRepository.SaveChanges(); // Save to get the ID
                }

                var personInterest = new PersonInterest(person.Id, interest.Id);
                _interestRepository.CreatePersonInterest(personInterest);
            }

            _interestRepository.SaveChanges();
            return _tokenGenerator.GenerateAccessToken(user, person.Id);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }
}