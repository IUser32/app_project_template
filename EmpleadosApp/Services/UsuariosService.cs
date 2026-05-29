using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using EmpleadosApp.Models;

namespace EmpleadosApp.Services;

public static class UsuariosService
{
    public static ObservableCollection<Usuario> Usuarios { get; } = new();

    public static Usuario? UsuarioActual { get; private set; }

    public static async Task CargarAsync()
    {
        var lista = await DatabaseService.Connection.Table<Usuario>()
            .OrderBy(u => u.Username)
            .ToListAsync();

        Usuarios.Clear();
        foreach (var usuario in lista)
        {
            Usuarios.Add(usuario);
        }
    }

    public static async Task AgregarAsync(Usuario usuario, string password)
    {
        usuario.PasswordHash = HashPassword(password);
        await DatabaseService.Connection.InsertAsync(usuario);
        InsertarOrdenado(usuario);
    }

    public static async Task ActualizarAsync(Usuario usuario, string? nuevoPassword)
    {
        if (!string.IsNullOrEmpty(nuevoPassword))
        {
            usuario.PasswordHash = HashPassword(nuevoPassword);
        }

        await DatabaseService.Connection.UpdateAsync(usuario);

        var indice = BuscarIndicePorId(usuario.Id);
        if (indice >= 0)
        {
            Usuarios.RemoveAt(indice);
            InsertarOrdenado(usuario);
        }
    }

    public static async Task EliminarAsync(int id)
    {
        await DatabaseService.Connection.DeleteAsync<Usuario>(id);

        var indice = BuscarIndicePorId(id);
        if (indice >= 0)
        {
            Usuarios.RemoveAt(indice);
        }
    }

    public static Usuario? ObtenerPorId(int id)
    {
        var indice = BuscarIndicePorId(id);
        return indice >= 0 ? Usuarios[indice] : null;
    }

    public static Usuario? Autenticar(string username, string password)
    {
        var hash = HashPassword(password);
        var usuario = Usuarios.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)
            && u.PasswordHash == hash
            && string.Equals(u.Estado, "Activo", StringComparison.OrdinalIgnoreCase));

        UsuarioActual = usuario;
        return usuario;
    }

    public static void CerrarSesion() => UsuarioActual = null;

    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private static void InsertarOrdenado(Usuario usuario)
    {
        for (var i = 0; i < Usuarios.Count; i++)
        {
            if (string.Compare(usuario.Username, Usuarios[i].Username, StringComparison.OrdinalIgnoreCase) < 0)
            {
                Usuarios.Insert(i, usuario);
                return;
            }
        }
        Usuarios.Add(usuario);
    }

    private static int BuscarIndicePorId(int id)
    {
        for (var i = 0; i < Usuarios.Count; i++)
        {
            if (Usuarios[i].Id == id) return i;
        }
        return -1;
    }
}
