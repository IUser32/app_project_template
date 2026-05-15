using System.Collections.ObjectModel;
using EmpleadosApp.Models;
using SQLite;

namespace EmpleadosApp.Services;

public static class EmpleadosService
{
    private static SQLiteAsyncConnection? _db;
    private static bool _inicializado;

    public static ObservableCollection<Empleado> Empleados { get; } = new();

    public static async Task InicializarAsync()
    {
        if (_inicializado) return;

        var ruta = Path.Combine(FileSystem.AppDataDirectory, "empleados.db3");
        _db = new SQLiteAsyncConnection(ruta);
        await _db.CreateTableAsync<Empleado>();

        var lista = await _db.Table<Empleado>().ToListAsync();
        Empleados.Clear();
        foreach (var empleado in lista)
        {
            Empleados.Add(empleado);
        }

        _inicializado = true;
    }

    public static async Task AgregarAsync(Empleado empleado)
    {
        await _db!.InsertAsync(empleado);
        Empleados.Add(empleado);
    }

    public static async Task ActualizarAsync(Empleado empleado)
    {
        await _db!.UpdateAsync(empleado);

        var indice = BuscarIndicePorId(empleado.Id);
        if (indice >= 0)
        {
            Empleados[indice] = empleado;
        }
    }

    public static async Task EliminarAsync(int id)
    {
        await _db!.DeleteAsync<Empleado>(id);

        var indice = BuscarIndicePorId(id);
        if (indice >= 0)
        {
            Empleados.RemoveAt(indice);
        }
    }

    public static Empleado? ObtenerPorId(int id)
    {
        var indice = BuscarIndicePorId(id);
        return indice >= 0 ? Empleados[indice] : null;
    }

    private static int BuscarIndicePorId(int id)
    {
        for (var i = 0; i < Empleados.Count; i++)
        {
            if (Empleados[i].Id == id) return i;
        }
        return -1;
    }
}
