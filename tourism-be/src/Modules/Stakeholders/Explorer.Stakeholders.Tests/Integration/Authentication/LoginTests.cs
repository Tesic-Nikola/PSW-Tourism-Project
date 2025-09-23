using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Tests.Integration.Authentication;

[Collection("Sequential")]
public class LoginTests : BaseStakeholdersIntegrationTest
{
    public LoginTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Successfully_logs_in_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turista1@gmail.com", Password = "turista1" };

        // Act
        var authenticationResponse = ((ObjectResult)controller.Login(loginSubmission).Result).Value as AuthenticationTokensDto;

        // Assert
        authenticationResponse.ShouldNotBeNull();
        authenticationResponse.Id.ShouldBe(-21);
        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
        personId.Value.ShouldBe("-21");
    }

    [Fact]
    public void Successfully_logs_in_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "autor1@gmail.com", Password = "autor1" };

        // Act
        var authenticationResponse = ((ObjectResult)controller.Login(loginSubmission).Result).Value as AuthenticationTokensDto;

        // Assert
        authenticationResponse.ShouldNotBeNull();
        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
    }

    [Fact]
    public void Successfully_logs_in_administrator()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "admin@gmail.com", Password = "admin" };

        // Act
        var authenticationResponse = ((ObjectResult)controller.Login(loginSubmission).Result).Value as AuthenticationTokensDto;

        // Assert
        authenticationResponse.ShouldNotBeNull();
        var decodedAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedAccessToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
    }

    [Fact]
    public void Not_registered_user_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "nonexistent@gmail.com", Password = "password" };

        // Act
        var result = (ObjectResult)controller.Login(loginSubmission).Result;

        // Assert
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Invalid_password_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turista1@gmail.com", Password = "wrongpassword" };

        // Act
        var result = (ObjectResult)controller.Login(loginSubmission).Result;

        // Assert
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Empty_username_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "", Password = "password" };

        // Act
        var result = (ObjectResult)controller.Login(loginSubmission).Result;

        // Assert
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Empty_password_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turista1@gmail.com", Password = "" };

        // Act
        var result = (ObjectResult)controller.Login(loginSubmission).Result;

        // Assert
        result.StatusCode.ShouldBe(404);
    }

    private static AuthenticationController CreateController(IServiceScope scope)
    {
        return new AuthenticationController(scope.ServiceProvider.GetRequiredService<IAuthenticationService>());
    }
}