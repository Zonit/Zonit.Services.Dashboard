using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Zonit.Dashboard.Extensions;

/// <summary>
/// Convenience base for drawer extensions that render a single <typeparamref name="TComponent"/>
/// inside their drawer body. AOT-safe — no reflection; the generic
/// constraint and <see cref="RenderTreeBuilder.OpenComponent{TComponent}"/> are
/// trimmer-friendly even with full IL trimming on.
/// </summary>
/// <typeparam name="TComponent">The Blazor component rendered as the drawer's body.</typeparam>
/// <example>
/// <code>
/// internal sealed class NotificationsDrawerExtension : DrawerExtensionBase&lt;NotificationsPanel&gt;
/// {
///     public override string Id => "notifications";
///     public override string Name => "Notifications";
///     public override DrawerAnchor Anchor => DrawerAnchor.End;
///     public override string? Icon => MudBlazor.Icons.Material.Filled.Notifications;
/// }
/// </code>
/// </example>
public abstract class DrawerExtensionBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent> : IDrawerExtension
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
    public virtual DrawerAnchor Anchor => DrawerAnchor.End;

    /// <inheritdoc />
    public virtual int Width => 320;

    /// <inheritdoc />
    public virtual string? Icon => null;

    /// <inheritdoc />
    public RenderFragment CreateDrawerContent() => builder =>
    {
        builder.OpenComponent<TComponent>(0);
        ConfigureComponent(builder);
        builder.CloseComponent();
    };

    /// <summary>
    /// Override to inject parameters or attributes into the rendered component
    /// (the typical case is no override — the component pulls dependencies from DI).
    /// </summary>
    protected virtual void ConfigureComponent(RenderTreeBuilder builder)
    {
    }
}
