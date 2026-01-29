namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Provides toast/snackbar notification functionality.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Shows a success toast message.
    /// </summary>
    /// <param name="message">The message to display (will be translated).</param>
    /// <param name="args">Optional format arguments for the message.</param>
    void Success(string message, params object[]? args);

    /// <summary>
    /// Shows an error toast message.
    /// </summary>
    /// <param name="message">The message to display (will be translated).</param>
    /// <param name="args">Optional format arguments for the message.</param>
    void Error(string message, params object[]? args);

    /// <summary>
    /// Shows a warning toast message.
    /// </summary>
    /// <param name="message">The message to display (will be translated).</param>
    /// <param name="args">Optional format arguments for the message.</param>
    void Warning(string message, params object[]? args);

    /// <summary>
    /// Shows an info toast message.
    /// </summary>
    /// <param name="message">The message to display (will be translated).</param>
    /// <param name="args">Optional format arguments for the message.</param>
    void Info(string message, params object[]? args);
}
