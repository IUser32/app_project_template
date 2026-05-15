using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class EmpleadosListPage : ContentPage
{
    public EmpleadosListPage()
    {
        InitializeComponent();
        EmpleadosCollection.ItemsSource = EmpleadosService.Empleados;
    }

    private async void OnNuevoEmpleadoClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//empleado-form");
    }
}
