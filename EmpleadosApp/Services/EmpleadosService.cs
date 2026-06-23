using System.Collections.ObjectModel;
using EmpleadosApp.Models;

namespace EmpleadosApp.Services;

public static class EmpleadosService
{
    private static Task? _inicializacion;

    public static ObservableCollection<Empleado> Empleados { get; } = new();

    // Single-flight: todos los llamados comparten una sola inicializacion,
    // evitando cargas concurrentes que se pisan entre si en el primer arranque.
    public static Task InicializarAsync() => _inicializacion ??= EjecutarInicializacionAsync();

    private static async Task EjecutarInicializacionAsync()
    {
        await DatabaseService.InicializarAsync();
        await UsuariosService.CargarAsync();
        await DepartamentosService.CargarAsync();
        await CargosService.CargarAsync();
        await CargarAsync();
    }

    public static async Task CargarAsync()
    {
        var lista = await DatabaseService.Connection.Table<Empleado>()
            .OrderBy(e => e.Apellido)
            .ToListAsync();

        Empleados.Clear();
        foreach (var empleado in lista)
        {
            CompletarNombresRelacionados(empleado);
            Empleados.Add(empleado);
        }
    }

    public static async Task AgregarAsync(Empleado empleado)
    {
        await DatabaseService.Connection.InsertAsync(empleado);
        CompletarNombresRelacionados(empleado);
        Empleados.Add(empleado);
    }

    public static async Task ActualizarAsync(Empleado empleado)
    {
        await DatabaseService.Connection.UpdateAsync(empleado);
        CompletarNombresRelacionados(empleado);

        var indice = BuscarIndicePorId(empleado.Id);
        if (indice >= 0)
        {
            Empleados[indice] = empleado;
        }
    }

    public static async Task EliminarAsync(int id)
    {
        await DatabaseService.Connection.DeleteAsync<Empleado>(id);

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

    public static int ContarPorEstado(string estado)
        => Empleados.Count(e => string.Equals(e.Estado, estado, StringComparison.OrdinalIgnoreCase));

    public static int ContarPorDepartamento(int departamentoId)
        => Empleados.Count(e => e.DepartamentoId == departamentoId);

    public static void RefrescarNombresRelacionados()
    {
        var snapshot = Empleados.ToList();
        Empleados.Clear();
        foreach (var empleado in snapshot)
        {
            CompletarNombresRelacionados(empleado);
            Empleados.Add(empleado);
        }
    }

    private static void CompletarNombresRelacionados(Empleado empleado)
    {
        empleado.CargoNombre = CargosService.NombrePorId(empleado.CargoId);
        empleado.DepartamentoNombre = DepartamentosService.NombrePorId(empleado.DepartamentoId);
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
