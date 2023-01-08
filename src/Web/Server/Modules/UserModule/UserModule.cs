﻿using XClaim.Common.Dtos;

namespace XClaim.Web.Server.Modules.UserModule;

public class UserModule : IModule {
    public IServiceCollection RegisterApiModule(IServiceCollection services) {
        services.AddScoped<UserService>();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints) {
        const string name = nameof(User);
        var url = $"{Constants.RootApi}/{name.ToLower()}";
        var group = endpoints.MapGroup(url).WithTags(name);
            
        group.MapGet("/", async (UserService sv, [AsParameters] GenericFilter filter) =>
                await sv.GetAllAsync(null))
            .WithName($"GetAll{name}")
            .WithOpenApi();
            
        group.MapGet("/{id:guid}", async (Guid id, UserService sv) => {
            var result = await sv.GetByIdAsync(id);
            return TypedResults.Ok(result);
        }).WithName($"Get{name}ById").WithOpenApi();
            
        group.MapPost("/", async (User value, UserService sv) => {
            await sv.CreateAsync(value);
            return TypedResults.Created($"{url}/{value.Id}", value);
        }).WithName($"Create{name}").WithOpenApi();
            
        group.MapPut("/", async (User value, UserService sv) => {
            var result = await sv.UpdateAsync(value);
            return result is null ? Results.NotFound() : TypedResults.Ok(value);
        }).WithName($"Update{name}").WithOpenApi();
            
        group.MapDelete("/{id:guid}", async (Guid id, UserService sv) => {
            var item = await sv.DeleteAsync(id);
            return item is null ? Results.NotFound() : TypedResults.Ok(item);
        }).WithName($"Archive{name}").WithOpenApi();

        return group;
    }
}