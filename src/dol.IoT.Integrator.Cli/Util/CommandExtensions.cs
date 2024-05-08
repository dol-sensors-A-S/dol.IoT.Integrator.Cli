using Cocona;
using Cocona.Builder;

namespace dol.IoT.Integrator.Cli.Util;

public static class CommandExtensions
{
    public static CommandConventionBuilder WithIntegratorDescription(
        this CommandConventionBuilder builder,
        string description,
        bool hasSideEffects)
    {
        return WriteDescription(builder, description, hasSideEffects);
    }
    
    public static CommandConventionBuilder WriteDescription(CommandConventionBuilder builder, string description, bool hasSideEffects)
    {
        var sideEffects = hasSideEffects ? "[Side effects]" : "              ";
        return builder.WithDescription($"{sideEffects} {description}");
    }
}