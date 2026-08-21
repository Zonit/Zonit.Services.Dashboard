namespace Zonit.Dashboard;

/// <summary>
/// Where the dashboard chrome sends people for identity, and where its built-in identity screens
/// post to. Every value is a URL the <b>host</b> owns.
/// </summary>
/// <remarks>
/// <para><b>The dashboard does not create, write, or destroy sessions.</b> An earlier revision of
/// this package shipped its own <c>POST identity/auth/signin</c> endpoint that called a host hook
/// for credential checking and then wrote the <c>Session</c> cookie itself. That was wrong, and
/// not only on layering grounds — it was reproducibly broken:</para>
///
/// <list type="number">
///   <item>The host's own login endpoint wrote <c>Session</c> with no <c>Path</c>, so it landed at
///   <c>/</c>. The package wrote the same cookie name scoped to the mount, <c>/docs</c>. Both
///   existed at once, holding <em>different sessions for different users</em>.</item>
///   <item>The package's sign-out deleted only its own copy. Measured: after clicking "Sign out"
///   the visitor was still authenticated, because the other <c>Session</c> cookie was untouched.
///   A sign-out that does not sign you out is a security defect, not a cosmetic one.</item>
///   <item>Cookie name, <c>Path</c>, <c>Domain</c>, <c>SameSite</c>, lifetime and rotation are
///   application-wide authentication decisions. A UI package mounted on one path cannot make them
///   correctly for the application, and if it guesses differently from the host — as it did — the
///   two halves silently disagree.</item>
/// </list>
///
/// <para>So the split is now absolute: the dashboard renders identity <em>screens</em>, and the
/// host owns the <em>session</em>. The login form posts to <see cref="SignInEndpoint"/>, a URL the
/// host maps and controls; the package never sees a password, never issues a token and never
/// touches a cookie.</para>
///
/// <para><b>What the host's endpoint is responsible for</b>, none of which the package can do for
/// it: validating an antiforgery token, rate-limiting attempts, rotating the session on privilege
/// change, choosing cookie attributes, and validating the posted <c>returnUrl</c> against its own
/// origin before redirecting to it. An unvalidated return URL is an open redirect.</para>
///
/// <para>Leave a URL <see langword="null"/> and the corresponding chrome entry disappears rather
/// than pointing at a route nobody serves. With <see cref="SignInEndpoint"/> unset, the built-in
/// login screen renders an explicit "not configured" panel instead of a form that cannot work.</para>
///
/// <para><b>Why the built-in screens live under <c>dashboard/</c> and not <c>identity/auth/</c>.</b>
/// They used to sit on <c>identity/auth/login</c> and <c>identity/auth/logout</c>, which is the
/// route namespace an identity PLUGIN owns. A host that mounted such a plugin ended up with two
/// components claiming one route, and routing does not pick a winner — it throws
/// <c>AmbiguousMatchException</c>, so every sign-in URL answered 500 on every mount. Worse, the
/// failure only appeared once someone actually tried to log in, long after the mount looked healthy.
/// Nothing here can yield the route at runtime either: these are Razor components in the app
/// assembly, so they are in the route table whether a host wants them or not.</para>
///
/// <para>Hence a namespace this package owns outright. A host that brings its own login page points
/// <see cref="LoginPage"/> at it and the built-in screen simply stops being linked; a host that
/// brings nothing still gets a working screen. Do not move these back under <c>identity/</c>.</para>
/// </remarks>
public sealed class DashboardIdentityOptions
{
    /// <summary>
    /// Page the chrome links to for signing in. Defaults to the package's built-in screen; set to
    /// <see langword="null"/> to hide every "Sign in" affordance, or to your own path to use your
    /// own page.
    /// </summary>
    /// <remarks>Base-relative (no leading slash) so it resolves inside the mount.</remarks>
    public string? LoginPage { get; set; } = "dashboard/sign-in";

    /// <summary>
    /// Page the chrome links to for signing out — a page, never the endpoint. The chrome offers
    /// "Logout" as a link, a link is a GET, and a GET must not end a session: any third-party page
    /// could embed <c>&lt;img src="…/logout"&gt;</c> and quietly log the visitor out. The page
    /// confirms, then posts.
    /// </summary>
    public string? LogoutPage { get; set; } = "dashboard/sign-out";

    /// <summary>
    /// Page the chrome links to for the signed-in user's profile. Defaults to the package's
    /// built-in read-only screen (name, id, roles, permissions).
    /// </summary>
    public string? ProfilePage { get; set; } = "dashboard/profile";

    /// <summary>
    /// URL the built-in login form POSTs credentials to. <b>Host-owned; no default.</b>
    /// </summary>
    /// <remarks>
    /// <para>The form posts <c>username</c>, <c>password</c> and <c>returnUrl</c> as
    /// <c>application/x-www-form-urlencoded</c>. Everything after that is the host's: verify,
    /// issue a session, set whatever cookie the host's <c>IAuthSource</c> will read back, and
    /// redirect.</para>
    ///
    /// <para>There is deliberately no default. A default would mean the package shipping an
    /// authentication endpoint that every consumer gets whether they want one or not — including
    /// hosts on external OIDC, for whom it is pure attack surface.</para>
    /// </remarks>
    public string? SignInEndpoint { get; set; }

    /// <summary>
    /// URL the built-in sign-out confirmation POSTs to. Host-owned; no default, same reasoning as
    /// <see cref="SignInEndpoint"/>.
    /// </summary>
    public string? SignOutEndpoint { get; set; }

    /// <summary><see langword="true"/> when the host has wired a sign-in endpoint.</summary>
    public bool CanSignIn => !string.IsNullOrWhiteSpace(SignInEndpoint);

    /// <summary><see langword="true"/> when the host has wired a sign-out endpoint.</summary>
    public bool CanSignOut => !string.IsNullOrWhiteSpace(SignOutEndpoint);
}
