using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class DepartamentosListPage : ContentPage
{
    public DepartamentosListPage()
    {
        InitializeComponent();
        DepartamentosCollection.ItemsSource = DepartamentosService.Departamentos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
    }

    private async void OnNuevoDepartamentoClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//departamento-form");
    }

    private async void OnDepartamentoSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Departamento departamento) return;

        DepartamentosCollection.SelectedItem = null;

        await Shell.Current.GoToAsync($"//departamento-form?id={departamento.Id}");
    }
}
