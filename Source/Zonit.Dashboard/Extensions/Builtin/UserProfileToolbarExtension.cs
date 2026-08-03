using Zonit.Dashboard.Components.Toolbar;

namespace Zonit.Dashboard.Extensions.Builtin;

/// <summary>
/// Built-in toolbar extension that renders the user avatar + profile dropdown in
/// the dashboard's appbar End slot. Always present once <c>AddDashboard()</c>
/// runs; hosts can suppress it via <c>DashboardSiteOptions.ExtensionsWhitelist</c>.
/// </summary>
/// <remarks>
/// <para>Rendering is delegated to <see cref="UserProfileToolbar"/> which handles
/// both the anonymous case (Identity.Empty → Sign in button) and the authenticated
/// case (avatar + MudMenu with Profile / Settings / Logout). The base class wires
/// the AOT-safe RenderFragment factory.</para>
///
/// <para>Order: 1000 so any host-supplied toolbar extension ends up to the left
/// of the avatar by default (lower Order wins). This matches the legacy layout
/// where the avatar was always pinned to the far right.</para>
/// </remarks>
public sealed class UserProfileToolbarExtension : ToolbarExtensionBase<UserProfileToolbar>
{
    public override string Id => "zonit.dashboard.user-profile";
    public override string Name => "User Profile";
    public override int Order => 1000;
    public override ToolbarPosition Position => ToolbarPosition.End;
}
