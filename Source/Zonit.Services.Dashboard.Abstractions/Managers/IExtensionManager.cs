namespace Zonit.Services.Dashboard;

public interface IExtensionManager
{
    /// <summary>
    /// Uzyskuje obiekt zarządzania szufladą dla konkretnego rozszerzenia.
    /// </summary>
    /// <param name="extension">Nazwa rozszerzenia</param>
    /// <returns>Interfejs do zarządzania szufladą</returns>
    IDrawer Drawer(string extension);
}

/// <summary>
/// Interfejs do zarządzania pojedynczą szufladą.
/// </summary>
public interface IDrawer
{
    /// <summary>
    /// Sprawdza, czy szuflada jest otwarta.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Otwiera szufladę. Jeśli szuflada jest już otwarta, to nie robi nic.
    /// </summary>
    void Open();

    /// <summary>
    /// Zamyka szufladę. Jeśli szuflada jest już zamknięta, to nie robi nic.
    /// </summary>
    void Close();

    /// <summary>
    /// Zmienia stan szuflady na przeciwny.
    /// </summary>
    /// <returns>Nowy stan szuflady (true - otwarta, false - zamknięta)</returns>
    bool Toggle();

    /// <summary>
    /// Ustawia stan szuflady na otwarty lub zamknięty.
    /// </summary>
    /// <param name="open">Stan szuflady (true - otwarta, false - zamknięta)</param>
    void Set(bool open);

    /// <summary>
    /// Zdarzenie wywoływane po każdej zmianie stanu szuflady.
    /// </summary>
    event Action? OnChange;
}