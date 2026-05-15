namespace EmpleadosApp.Views;

public partial class PrincipalPage : ContentPage
{
    public PrincipalPage()
    {
        InitializeComponent();
    }

    private async void OnVerEmpleadosClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//empleados");
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
            await Shell.Current.GoToAsync("//login");
        }
    }
}
