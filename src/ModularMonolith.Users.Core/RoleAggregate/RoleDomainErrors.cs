using Ardalis.Result;

namespace ModularMonolith.Users.Core.RoleAggregate;


public static class RoleDomainErrors
{
    public static ValidationError NotFound(Guid id) => new($"Role with Id = {id} not found!");
}