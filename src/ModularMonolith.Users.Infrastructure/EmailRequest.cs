namespace ModularMonolith.Users.Infrastructure;

public sealed record EmailRequest(string Email, string Subject, string Message);
