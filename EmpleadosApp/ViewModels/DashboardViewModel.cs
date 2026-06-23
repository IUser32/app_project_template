using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using EmpleadosApp.Services;

namespace EmpleadosApp.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty] private string saludo = "Hola";
    [ObservableProperty] private int totalEmpleados;
    [ObservableProperty] private int empleadosActivos;
    [ObservableProperty] private int empleadosInactivos;
    [ObservableProperty] private string resumenEmpleados = string.Empty;
    [ObservableProperty] private int totalDepartamentos;
    [ObservableProperty] private int totalCargos;
    [ObservableProperty] private int totalUsuarios;
    [ObservableProperty] private bool hayEmpleados;

    public ObservableCollection<BarraDepartamento> EmpleadosPorDepartamento { get; } = new();

    public DashboardViewModel()
    {
        EmpleadosService.Empleados.CollectionChanged += OnColeccionCambiada;
        DepartamentosService.Departamentos.CollectionChanged += OnColeccionCambiada;
        CargosService.Cargos.CollectionChanged += OnColeccionCambiada;
        UsuariosService.Usuarios.CollectionChanged += OnColeccionCambiada;
    }

    public void Refrescar()
    {
        var nombre = UsuariosService.UsuarioActual?.NombreCompleto;
        Saludo = string.IsNullOrWhiteSpace(nombre) ? "Hola" : $"Hola, {nombre}";

        TotalEmpleados = EmpleadosService.Empleados.Count;
        EmpleadosActivos = EmpleadosService.ContarPorEstado("Activo");
        EmpleadosInactivos = EmpleadosService.ContarPorEstado("Inactivo");
        ResumenEmpleados = $"{EmpleadosActivos} activos · {EmpleadosInactivos} inactivos";
        TotalDepartamentos = DepartamentosService.Departamentos.Count;
        TotalCargos = CargosService.Cargos.Count;
        TotalUsuarios = UsuariosService.Usuarios.Count;

        HayEmpleados = TotalEmpleados > 0;
        ConstruirGrafico();
    }

    private void ConstruirGrafico()
    {
        var conteos = DepartamentosService.Departamentos
            .Select(d => new { d.Nombre, Cantidad = EmpleadosService.ContarPorDepartamento(d.Id) })
            .OrderByDescending(x => x.Cantidad)
            .ToList();

        var maximo = conteos.Count > 0 ? conteos.Max(x => x.Cantidad) : 0;

        EmpleadosPorDepartamento.Clear();
        foreach (var c in conteos)
        {
            EmpleadosPorDepartamento.Add(new BarraDepartamento
            {
                Nombre = c.Nombre,
                Cantidad = c.Cantidad,
                Peso = c.Cantidad,
                PesoResto = Math.Max(0, maximo - c.Cantidad)
            });
        }
    }

    private void OnColeccionCambiada(object? sender, NotifyCollectionChangedEventArgs e) => Refrescar();
}
