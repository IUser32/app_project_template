namespace EmpleadosApp.Views;

public partial class LoginPage : ContentPage
{
    private const string UsuarioValido = "admin";
    private const string ContrasenaValida = "1234";

    public LoginPage()
    {
        InitializeComponent();
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

        if (usuario == UsuarioValido && contrasena == ContrasenaValida)
        {
            UsuarioEntry.Text = string.Empty;
            ContrasenaEntry.Text = string.Empty;

            await Shell.Current.GoToAsync("//principal");
        }
        else
        {
            await DisplayAlertAsync("Credenciales inválidas", "Usuario o contraseña incorrectos.", "Aceptar");
        }
    }
}
