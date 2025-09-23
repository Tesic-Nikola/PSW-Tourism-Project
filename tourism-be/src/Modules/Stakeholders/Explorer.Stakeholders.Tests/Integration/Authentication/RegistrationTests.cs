using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Tests.Integration.Authentication;

[Collection("Sequential")]
public class RegistrationTests : BaseStakeholdersIntegrationTest
{
    public RegistrationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Successfully_registers_tourist_with_interests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "newuser@gmail.com",
            Email = "newuser@gmail.com",
            Password = "newuser123",
            Name = "Marko",
            Surname = "Marković",
            Interests = new List<string> { "priroda", "umetnost", "sport" }
        };

        // Act
        var authenticationResponse = ((ObjectResult)controller.RegisterTourist(account).Result).Value as AuthenticationTokensDto;

        // Assert - Response
        authenticationResponse.ShouldNotBeNull();
        authenticationResponse.Id.ShouldNotBe(0);
        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");

        personId.ShouldNotBeNull();
        personId.Value.ShouldNotBe("0");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedUser = dbContext.Users.FirstOrDefault(u => u.Username == account.Email);
        storedUser.ShouldNotBeNull();
        storedUser.Role.ShouldBe(UserRole.Tourist);

        var storedPerson = dbContext.People.FirstOrDefault(p => p.Email == account.Email);
        storedPerson.ShouldNotBeNull();
        storedPerson.UserId.ShouldBe(storedUser.Id);
        storedPerson.Name.ShouldBe(account.Name);
        storedPerson.Surname.ShouldBe(account.Surname);

        // Assert - Interests
        var personInterests = dbContext.PersonInterests
            .Where(pi => pi.PersonId == storedPerson.Id)
            .ToList();
        personInterests.Count.ShouldBe(3);

        var interestNames = personInterests.Select(pi => pi.Interest.Name).ToList();
        interestNames.ShouldContain("priroda");
        interestNames.ShouldContain("umetnost");
        interestNames.ShouldContain("sport");
    }

    [Fact]
    public void Successfully_registers_tourist_with_single_interest()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "singleinterest@gmail.com",
            Email = "singleinterest@gmail.com",
            Password = "password123",
            Name = "Ana",
            Surname = "Anić",
            Interests = new List<string> { "hrana" }
        };

        // Act
        var authenticationResponse = ((ObjectResult)controller.RegisterTourist(account).Result).Value as AuthenticationTokensDto;

        // Assert
        authenticationResponse.ShouldNotBeNull();

        dbContext.ChangeTracker.Clear();
        var storedPerson = dbContext.People.FirstOrDefault(p => p.Email == account.Email);
        storedPerson.ShouldNotBeNull();

        var personInterests = dbContext.PersonInterests
            .Where(pi => pi.PersonId == storedPerson.Id)
            .ToList();
        personInterests.Count.ShouldBe(1);
        personInterests.First().Interest.Name.ShouldBe("hrana");
    }

    [Fact]
    public void Successfully_registers_tourist_with_all_interests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "allinterests@gmail.com",
            Email = "allinterests@gmail.com",
            Password = "password123",
            Name = "Petar",
            Surname = "Petrović",
            Interests = new List<string> { "priroda", "umetnost", "sport", "šoping", "hrana" }
        };

        // Act
        var authenticationResponse = ((ObjectResult)controller.RegisterTourist(account).Result).Value as AuthenticationTokensDto;

        // Assert
        authenticationResponse.ShouldNotBeNull();

        dbContext.ChangeTracker.Clear();
        var storedPerson = dbContext.People.FirstOrDefault(p => p.Email == account.Email);
        storedPerson.ShouldNotBeNull();

        var personInterests = dbContext.PersonInterests
            .Where(pi => pi.PersonId == storedPerson.Id)
            .ToList();
        personInterests.Count.ShouldBe(5);

        var interestNames = personInterests.Select(pi => pi.Interest.Name).ToList();
        interestNames.ShouldContain("priroda");
        interestNames.ShouldContain("umetnost");
        interestNames.ShouldContain("sport");
        interestNames.ShouldContain("šoping");
        interestNames.ShouldContain("hrana");
    }

    [Fact]
    public void Registration_fails_with_empty_interests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "nointerests@gmail.com",
            Email = "nointerests@gmail.com",
            Password = "password123",
            Name = "Test",
            Surname = "User",
            Interests = new List<string>()
        };

        // Act
        var result = (ObjectResult)controller.RegisterTourist(account).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Registration_fails_with_invalid_interests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "invalidinterest@gmail.com",
            Email = "invalidinterest@gmail.com",
            Password = "password123",
            Name = "Test",
            Surname = "User",
            Interests = new List<string> { "invalidinterest", "priroda" }
        };

        // Act
        var result = (ObjectResult)controller.RegisterTourist(account).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Registration_fails_with_duplicate_username()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "turista1@gmail.com", // Already exists in test data
            Email = "turista1@gmail.com",
            Password = "password123",
            Name = "Test",
            Surname = "User",
            Interests = new List<string> { "priroda" }
        };

        // Act
        var result = (ObjectResult)controller.RegisterTourist(account).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Registration_fails_with_invalid_email()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "invalidemail@gmail.com",
            Email = "not-an-email",
            Password = "password123",
            Name = "Test",
            Surname = "User",
            Interests = new List<string> { "priroda" }
        };

        // Act
        var result = (ObjectResult)controller.RegisterTourist(account).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Registration_fails_with_empty_required_fields()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "",
            Email = "",
            Password = "",
            Name = "",
            Surname = "",
            Interests = new List<string> { "priroda" }
        };

        // Act
        var result = (ObjectResult)controller.RegisterTourist(account).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    private static AuthenticationController CreateController(IServiceScope scope)
    {
        return new AuthenticationController(scope.ServiceProvider.GetRequiredService<IAuthenticationService>());
    }
}