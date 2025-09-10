namespace PWC.Challenge.Application.Services;

public interface IAuthService
{
    bool IsAuthenticated();

    string GetFullName();

    Guid GetUserId();
}
