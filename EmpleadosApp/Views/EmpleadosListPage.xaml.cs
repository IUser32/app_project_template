using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class EmpleadosListPage : ContentPage
{
    private readonly ObservableCollection<Empleado> _empleadosFiltrados = new();
    private string _filtroActual = string.Empty;

    public EmpleadosListPage()
    {
        InitializeComponent();
        EmpleadosCollection.ItemsSource = _empleadosFiltrados;
        EmpleadosService.Empleados.CollectionChanged += OnEmpleadosCambiados;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
        AplicarFiltro();
    }

    private void OnEmpleadosCambiados(object? sender, NotifyCollectionChangedEventArgs e) => AplicarFiltro();

    private void OnBuscarTextChanged(object? sender, TextChangedEventArgs e)
    {
        _filtroActual = e.NewTextValue?.Trim() ?? string.Empty;
        AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        _empleadosFiltrados.Clear();

        IEnumerable<Empleado> resultado = EmpleadosService.Empleados;

        if (!string.IsNullOrWhiteSpace(_filtroActual))
        {
            var termino = _filtroActual;
            resultado = resultado.Where(empleado =>
                empleado.NombreCompleto.Contains(termino, StringComparison.OrdinalIgnoreCase)
                || empleado.Cedula.Contains(termino, StringComparison.OrdinalIgnoreCase)
                || empleado.CargoNombre.Contains(termino, StringComparison.OrdinalIgnoreCase)
                || empleado.DepartamentoNombre.Contains(termino, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var empleado in resultado)
        {
            _empleadosFiltrados.Add(empleado);
        }
    }

    private async void OnNuevoEmpleadoClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//empleado-form");
    }

    private async void OnEmpleadoSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Empleado empleado) return;

        EmpleadosCollection.SelectedItem = null;

        await Shell.Current.GoToAsync($"//empleado-form?id={empleado.Id}");
    }
}
