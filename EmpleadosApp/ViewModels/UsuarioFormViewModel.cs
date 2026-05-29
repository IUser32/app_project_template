using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmpleadosApp.Models;
using EmpleadosApp.Services;
using SQLite;

namespace EmpleadosApp.ViewModels;

public partial class UsuarioFormViewModel : ObservableObject, IQueryAttributable
{
    private static readonly Regex UsernameValido = new(@"^[a-zA-Z0-9._-]+$");

    private int _idEditando;

    [ObservableProperty] private bool modoEditar;
    [ObservableProperty] private string tituloPagina = "Nuevo usuario";

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string nombreCompleto = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmarPassword = string.Empty;
    [ObservableProperty] private string? estadoSeleccionado = "Activo";
    [ObservableProperty] private string passwordPlaceholder = "Mínimo 4 caracteres";

    [ObservableProperty] private string usernameError = string.Empty;
    [ObservableProperty] private string nombreCompletoError = string.Empty;
    [ObservableProperty] private string passwordError = string.Empty;
    [ObservableProperty] private string confirmarPasswordError = string.Empty;
    [ObservableProperty] private string estadoError = string.Empty;

    public ObservableCollection<string> EstadosDisponibles { get; } = new() { "Activo", "Inactivo" };

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out var id) && id > 0)
        {
            var usuario = UsuariosService.ObtenerPorId(id);
            if (usuario is not null)
            {
                CargarDesde(usuario);
                return;
            }
        }

        ResetearParaNuevo();
    }

    private void CargarDesde(Usuario usuario)
    {
        _idEditando = usuario.Id;
        ModoEditar = true;
        TituloPagina = "Editar usuario";

        Username = usuario.Username;
        NombreCompleto = usuario.NombreCompleto;
        Password = string.Empty;
        ConfirmarPassword = string.Empty;
        PasswordPlaceholder = "Déjalo en blanco para mantener la actual";
        EstadoSeleccionado = usuario.Estado;

        LimpiarErrores();
    }

    private void ResetearParaNuevo()
    {
        _idEditando = 0;
        ModoEditar = false;
        TituloPagina = "Nuevo usuario";

        Username = string.Empty;
        NombreCompleto = string.Empty;
        Password = string.Empty;
        ConfirmarPassword = string.Empty;
        PasswordPlaceholder = "Mínimo 4 caracteres";
        EstadoSeleccionado = "Activo";

        LimpiarErrores();
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar()) return;

        var usuario = new Usuario
        {
            Username = Username.Trim(),
            NombreCompleto = NombreCompleto.Trim(),
            Estado = EstadoSeleccionado!
        };

        try
        {
            if (ModoEditar)
            {
                usuario.Id = _idEditando;
                var nuevoPassword = string.IsNullOrEmpty(Password) ? null : Password;
                await UsuariosService.ActualizarAsync(usuario, nuevoPassword);
                await Shell.Current.DisplayAlertAsync(
                    "Usuario actualizado",
                    $"{usuario.Username} fue actualizado correctamente.",
                    "Aceptar");
            }
            else
            {
                await UsuariosService.AgregarAsync(usuario, Password);
                await Shell.Current.DisplayAlertAsync(
                    "Usuario registrado",
                    $"{usuario.Username} fue agregado correctamente.",
                    "Aceptar");
            }
        }
        catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
        {
            UsernameError = "Ya existe un usuario con ese nombre.";
            return;
        }

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//usuarios");
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (!ModoEditar) return;

        if (UsuariosService.UsuarioActual?.Id == _idEditando)
        {
            await Shell.Current.DisplayAlertAsync(
                "No se puede eliminar",
                "No puedes eliminar el usuario con el que iniciaste sesión.",
                "Entendido");
            return;
        }

        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar usuario",
            $"¿Seguro que quieres eliminar el usuario {Username}?",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar) return;

        await UsuariosService.EliminarAsync(_idEditando);

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//usuarios");
    }

    private bool Validar()
    {
        if (string.IsNullOrWhiteSpace(Username) || Username.Trim().Length is < 3 or > 30)
            UsernameError = "El usuario es obligatorio y debe tener entre 3 y 30 caracteres.";
        else if (!UsernameValido.IsMatch(Username.Trim()))
            UsernameError = "Solo letras, números, puntos, guiones y guion bajo.";
        else
            UsernameError = string.Empty;

        NombreCompletoError = string.IsNullOrWhiteSpace(NombreCompleto) || NombreCompleto.Trim().Length is < 2 or > 80
            ? "El nombre completo es obligatorio y debe tener entre 2 y 80 caracteres."
            : string.Empty;

        var passwordRequerido = !ModoEditar || !string.IsNullOrEmpty(Password) || !string.IsNullOrEmpty(ConfirmarPassword);

        if (passwordRequerido)
        {
            if (string.IsNullOrEmpty(Password) || Password.Length < 4)
                PasswordError = "La contraseña debe tener al menos 4 caracteres.";
            else
                PasswordError = string.Empty;

            ConfirmarPasswordError = Password == ConfirmarPassword
                ? string.Empty
                : "Las contraseñas no coinciden.";
        }
        else
        {
            PasswordError = string.Empty;
            ConfirmarPasswordError = string.Empty;
        }

        EstadoError = string.IsNullOrWhiteSpace(EstadoSeleccionado)
            ? "Selecciona un estado."
            : string.Empty;

        return string.IsNullOrEmpty(UsernameError)
            && string.IsNullOrEmpty(NombreCompletoError)
            && string.IsNullOrEmpty(PasswordError)
            && string.IsNullOrEmpty(ConfirmarPasswordError)
            && string.IsNullOrEmpty(EstadoError);
    }

    private void LimpiarErrores()
    {
        UsernameError = string.Empty;
        NombreCompletoError = string.Empty;
        PasswordError = string.Empty;
        ConfirmarPasswordError = string.Empty;
        EstadoError = string.Empty;
    }
}
