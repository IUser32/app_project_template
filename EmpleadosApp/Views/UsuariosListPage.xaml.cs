using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class UsuariosListPage : ContentPage
{
    public UsuariosListPage()
    {
        InitializeComponent();
        UsuariosCollection.ItemsSource = UsuariosService.Usuarios;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
    }

    private async void OnNuevoUsuarioClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//usuario-form");
    }

    private async void OnUsuarioSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Usuario usuario) return;

        UsuariosCollection.SelectedItem = null;

        await Shell.Current.GoToAsync($"//usuario-form?id={usuario.Id}");
    }
}
