using System.Collections.ObjectModel;
using EmpleadosApp.Models;

namespace EmpleadosApp.Services;

public static class DepartamentosService
{
    public static ObservableCollection<Departamento> Departamentos { get; } = new();

    public static async Task CargarAsync()
    {
        var lista = await DatabaseService.Connection.Table<Departamento>()
            .OrderBy(d => d.Nombre)
            .ToListAsync();

        Departamentos.Clear();
        foreach (var departamento in lista)
        {
            Departamentos.Add(departamento);
        }
    }

    public static async Task AgregarAsync(Departamento departamento)
    {
        await DatabaseService.Connection.InsertAsync(departamento);
        InsertarOrdenado(departamento);
    }

    public static async Task ActualizarAsync(Departamento departamento)
    {
        await DatabaseService.Connection.UpdateAsync(departamento);

        var indice = BuscarIndicePorId(departamento.Id);
        if (indice >= 0)
        {
            Departamentos.RemoveAt(indice);
            InsertarOrdenado(departamento);
        }
    }

    public static async Task EliminarAsync(int id)
    {
        await DatabaseService.Connection.DeleteAsync<Departamento>(id);

        var indice = BuscarIndicePorId(id);
        if (indice >= 0)
        {
            Departamentos.RemoveAt(indice);
        }
    }

    public static Departamento? ObtenerPorId(int id)
    {
        var indice = BuscarIndicePorId(id);
        return indice >= 0 ? Departamentos[indice] : null;
    }

    public static string NombrePorId(int id) => ObtenerPorId(id)?.Nombre ?? string.Empty;

    private static void InsertarOrdenado(Departamento departamento)
    {
        for (var i = 0; i < Departamentos.Count; i++)
        {
            if (string.Compare(departamento.Nombre, Departamentos[i].Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                Departamentos.Insert(i, departamento);
                return;
            }
        }
        Departamentos.Add(departamento);
    }

    private static int BuscarIndicePorId(int id)
    {
        for (var i = 0; i < Departamentos.Count; i++)
        {
            if (Departamentos[i].Id == id) return i;
        }
        return -1;
    }
}
