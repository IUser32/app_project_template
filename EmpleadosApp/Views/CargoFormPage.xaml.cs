using EmpleadosApp.Services;
using EmpleadosApp.ViewModels;

namespace EmpleadosApp.Views;

public partial class CargoFormPage : ContentPage
{
    public CargoFormPage()
    {
        InitializeComponent();
        BindingContext = new CargoFormViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
    }
}
