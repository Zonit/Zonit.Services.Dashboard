using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Zonit.Dashboard.Extensions;

/// <summary>
/// Convenience base for toolbar extensions that render a single
/// <typeparamref name="TComponent"/> inside their appbar slot. AOT-safe twin of
/// <see cref="DrawerExtensionBase{TComponent}"/>.
/// </summary>
/// <typeparam name="TComponent">The Blazor component rendered in the appbar.</typeparam>
public abstract class ToolbarExtensionBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent> : IToolbarExtension
    where TComponent : IComponent
{
    /// <inheritdoc />
    public abstract string Id { get; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public virtual int Order => 0;

    /// <inheritdoc />
    public virtual bool IsEnabled => true;

    /// <inheritdoc />
    public virtual ToolbarPosition Position => ToolbarPosition.End;

    /// <inheritdoc />
    public RenderFragment CreateToolbarContent() => builder =>
    {
        builder.OpenComponent<TComponent>(0);
        ConfigureComponent(builder);
        builder.CloseComponent();
    };

    /// <summary>
    /// Override to inject parameters or attributes into the rendered component.
    /// </summary>
    protected virtual void ConfigureComponent(RenderTreeBuilder builder)
    {
    }
}
