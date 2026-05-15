namespace EmpleadosApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    private async void OnCerrarSesionClicked(object? sender, EventArgs e)
    {
        var confirmar = await DisplayAlertAsync(
            "Cerrar sesión",
            "¿Seguro que quieres salir?",
            "Sí",
            "No");

        if (confirmar)
        {
            FlyoutIsPresented = false;
            await GoToAsync("//login");
        }
    }
}
