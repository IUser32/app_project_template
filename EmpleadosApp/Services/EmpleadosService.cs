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
}
