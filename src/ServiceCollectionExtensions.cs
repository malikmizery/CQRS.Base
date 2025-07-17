using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CQRS.Base;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCQRSHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        // Register ICommandHandler<,> and ICommandHandler<>
        var commandHandlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .Select(i => new { HandlerType = t, InterfaceType = i }));

        foreach (var handler in commandHandlerTypes)
            services.AddScoped(handler.InterfaceType, handler.HandlerType);

        // Register IQueryHandler<,>
        var queryHandlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .Select(i => new { HandlerType = t, InterfaceType = i }));

        foreach (var handler in queryHandlerTypes)
            services.AddScoped(handler.InterfaceType, handler.HandlerType);

        return services;
    }
}