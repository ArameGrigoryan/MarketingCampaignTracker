namespace Shared.Contracts;

public record RegisterRequest(
    string Email,
    string Password,
    string Role = "Marketing"
);

