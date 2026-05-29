using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using EmpleadosApp.Services;

namespace EmpleadosApp.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty] private int totalEmpleados;
    [ObservableProperty] private int empleadosActivos;
    [ObservableProperty] private int empleadosInactivos;
    [ObservableProperty] private int totalDepartamentos;
    [ObservableProperty] private int totalCargos;
    [ObservableProperty] private int totalUsuarios;

    public DashboardViewModel()
    {
        EmpleadosService.Empleados.CollectionChanged += OnColeccionCambiada;
        DepartamentosService.Departamentos.CollectionChanged += OnColeccionCambiada;
        CargosService.Cargos.CollectionChanged += OnColeccionCambiada;
        UsuariosService.Usuarios.CollectionChanged += OnColeccionCambiada;
    }

    public void Refrescar()
    {
        TotalEmpleados = EmpleadosService.Empleados.Count;
        EmpleadosActivos = EmpleadosService.ContarPorEstado("Activo");
        EmpleadosInactivos = EmpleadosService.ContarPorEstado("Inactivo");
        TotalDepartamentos = DepartamentosService.Departamentos.Count;
        TotalCargos = CargosService.Cargos.Count;
        TotalUsuarios = UsuariosService.Usuarios.Count;
    }

    private void OnColeccionCambiada(object? sender, NotifyCollectionChangedEventArgs e) => Refrescar();
}
