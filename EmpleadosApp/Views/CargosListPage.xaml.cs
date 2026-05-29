using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class CargosListPage : ContentPage
{
    public CargosListPage()
    {
        InitializeComponent();
        CargosCollection.ItemsSource = CargosService.Cargos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
    }

    private async void OnNuevoCargoClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//cargo-form");
    }

    private async void OnCargoSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Cargo cargo) return;

        CargosCollection.SelectedItem = null;

        await Shell.Current.GoToAsync($"//cargo-form?id={cargo.Id}");
    }
}
