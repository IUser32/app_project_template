using System.Collections.ObjectModel;
using EmpleadosApp.Models;

namespace EmpleadosApp.Services;

public static class CargosService
{
    public static ObservableCollection<Cargo> Cargos { get; } = new();

    public static async Task CargarAsync()
    {
        var lista = await DatabaseService.Connection.Table<Cargo>()
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        Cargos.Clear();
        foreach (var cargo in lista)
        {
            Cargos.Add(cargo);
        }
    }

    public static async Task AgregarAsync(Cargo cargo)
    {
        await DatabaseService.Connection.InsertAsync(cargo);
        InsertarOrdenado(cargo);
    }

    public static async Task ActualizarAsync(Cargo cargo)
    {
        await DatabaseService.Connection.UpdateAsync(cargo);

        var indice = BuscarIndicePorId(cargo.Id);
        if (indice >= 0)
        {
            Cargos.RemoveAt(indice);
            InsertarOrdenado(cargo);
        }
    }

    public static async Task EliminarAsync(int id)
    {
        await DatabaseService.Connection.DeleteAsync<Cargo>(id);

        var indice = BuscarIndicePorId(id);
        if (indice >= 0)
        {
            Cargos.RemoveAt(indice);
        }
    }

    public static Cargo? ObtenerPorId(int id)
    {
        var indice = BuscarIndicePorId(id);
        return indice >= 0 ? Cargos[indice] : null;
    }

    public static string NombrePorId(int id) => ObtenerPorId(id)?.Nombre ?? string.Empty;

    public static IEnumerable<Cargo> PorDepartamento(int departamentoId)
        => Cargos.Where(c => c.DepartamentoId == departamentoId);

    private static void InsertarOrdenado(Cargo cargo)
    {
        for (var i = 0; i < Cargos.Count; i++)
        {
            if (string.Compare(cargo.Nombre, Cargos[i].Nombre, StringComparison.OrdinalIgnoreCase) < 0)
            {
                Cargos.Insert(i, cargo);
                return;
            }
        }
        Cargos.Add(cargo);
    }

    private static int BuscarIndicePorId(int id)
    {
        for (var i = 0; i < Cargos.Count; i++)
        {
            if (Cargos[i].Id == id) return i;
        }
        return -1;
    }
}
