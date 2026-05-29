using EmpleadosApp.Services;
using EmpleadosApp.ViewModels;

namespace EmpleadosApp.Views;

public partial class DepartamentoFormPage : ContentPage
{
    public DepartamentoFormPage()
    {
        InitializeComponent();
        BindingContext = new DepartamentoFormViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
    }
}
