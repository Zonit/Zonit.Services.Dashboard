using Microsoft.AspNetCore.Components;

namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Base class for drawer extensions providing AOT-safe component rendering.
/// Inherit from this class to create a type-safe drawer extension.
/// </summary>
/// <typeparam name="TComponent">The Blazor component type for the drawer.</typeparam>
public abstract class DrawerExtensionBase<TComponent> : IDrawerExtension
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
    public RenderFragment CreateDrawerContent() => builder =>
    {
        builder.OpenComponent<TComponent>(0);
        ConfigureComponent(builder);
        builder.CloseComponent();
    };

    /// <summary>
    /// Override to add additional parameters to the component.
    /// </summary>
    /// <param name="builder">The RenderTreeBuilder.</param>
    protected virtual void ConfigureComponent(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
    }
}

/// <summary>
/// Base class for toolbar extensions providing AOT-safe component rendering.
/// Inherit from this class to create a type-safe toolbar extension.
/// </summary>
/// <typeparam name="TComponent">The Blazor component type for the toolbar.</typeparam>
public abstract class ToolbarExtensionBase<TComponent> : IToolbarExtension
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
    public virtual ToolbarPosition Position => ToolbarPosition.Right;

    /// <inheritdoc />
    public RenderFragment CreateToolbarContent() => builder =>
    {
        builder.OpenComponent<TComponent>(0);
        ConfigureComponent(builder);
        builder.CloseComponent();
    };

    /// <summary>
    /// Override to add additional parameters to the component.
    /// </summary>
    /// <param name="builder">The RenderTreeBuilder.</param>
    protected virtual void ConfigureComponent(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
    }
}

/// <summary>
/// Base class for settings extensions providing AOT-safe component rendering.
/// Inherit from this class to create a type-safe settings extension.
/// </summary>
/// <typeparam name="TComponent">The Blazor component type for the settings.</typeparam>
public abstract class SettingsExtensionBase<TComponent> : ISettingsExtension
    where TComponent : IComponent
{
    /// <inheritdoc />
    public abstract string Id { get; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Section { get; }

    /// <inheritdoc />
    public virtual int Order => 0;

    /// <inheritdoc />
    public virtual bool IsEnabled => true;

    /// <inheritdoc />
    public RenderFragment CreateSettingsContent() => builder =>
    {
        builder.OpenComponent<TComponent>(0);
        ConfigureComponent(builder);
        builder.CloseComponent();
    };

    /// <summary>
    /// Override to add additional parameters to the component.
    /// </summary>
    /// <param name="builder">The RenderTreeBuilder.</param>
    protected virtual void ConfigureComponent(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
    }
}

/// <summary>
/// Base class for navigation extensions.
/// Inherit from this class to create a navigation extension that provides menu items.
/// </summary>
public abstract class NavigationExtensionBase : INavigationExtension
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
    public abstract IEnumerable<NavigationItem> GetNavigationItems(DashboardNavigationContext context);
}
