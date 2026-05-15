using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class EmpleadosListPage : ContentPage
{
    public EmpleadosListPage()
    {
        InitializeComponent();
        EmpleadosCollection.ItemsSource = EmpleadosService.Empleados;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
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

    private async void OnEliminarEmpleadoClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not Empleado empleado) return;

        var confirmar = await DisplayAlertAsync(
            "Eliminar empleado",
            $"¿Seguro que quieres eliminar a {empleado.NombreCompleto}?",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar) return;

        await EmpleadosService.EliminarAsync(empleado.Id);
    }
}
