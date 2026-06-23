using EmpleadosApp.Services;
using EmpleadosApp.ViewModels;

namespace EmpleadosApp.Views;

public partial class PrincipalPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public PrincipalPage()
    {
        InitializeComponent();
        _viewModel = new DashboardViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
        _viewModel.Refrescar();
    }

    private async void OnEmpleadosTapped(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//empleados");

    private async void OnDepartamentosTapped(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//departamentos");

    private async void OnCargosTapped(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//cargos");

    private async void OnUsuariosTapped(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//usuarios");

    private async void OnNuevoEmpleadoClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//empleado-form");

    private async void OnCerrarSesionClicked(object? sender, EventArgs e)
    {
        var confirmar = await DisplayAlertAsync(
            "Cerrar sesión",
            "¿Seguro que quieres salir?",
            "Sí",
            "No");

        if (confirmar)
        {
            UsuariosService.CerrarSesion();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
