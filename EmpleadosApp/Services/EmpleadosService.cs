using System.Collections.ObjectModel;
using EmpleadosApp.Models;

namespace EmpleadosApp.Services;

public static class EmpleadosService
{
    private static int _siguienteId = 1;

    public static ObservableCollection<Empleado> Empleados { get; } = new();

    public static void Agregar(Empleado empleado)
    {
        empleado.Id = _siguienteId++;
        Empleados.Add(empleado);
    }

    public static void Actualizar(Empleado empleado)
    {
        var indice = BuscarIndicePorId(empleado.Id);
        if (indice >= 0)
        {
            Empleados[indice] = empleado;
        }
    }

    public static void Eliminar(int id)
    {
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
