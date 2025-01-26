using Ardalis.Result;

namespace ModularMonolith.Users.Core.UserAggregate;

public static class UserDomainErrors
{
    public static ValidationError NotFound(Guid id) => new($"User with Id = {id} not found");
}
