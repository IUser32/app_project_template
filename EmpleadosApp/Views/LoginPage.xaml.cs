using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
    }

    private async void OnIniciarSesionClicked(object? sender, EventArgs e)
    {
        var usuario = UsuarioEntry.Text?.Trim() ?? string.Empty;
        var contrasena = ContrasenaEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
        {
            await DisplayAlertAsync("Datos incompletos", "Ingresa usuario y contraseña.", "Aceptar");
            return;
        }

        var autenticado = UsuariosService.Autenticar(usuario, contrasena);
        if (autenticado is null)
        {
            await DisplayAlertAsync("Credenciales inválidas", "Usuario o contraseña incorrectos.", "Aceptar");
            return;
        }

        UsuarioEntry.Text = string.Empty;
        ContrasenaEntry.Text = string.Empty;

        await Shell.Current.GoToAsync("//principal");
    }
}
