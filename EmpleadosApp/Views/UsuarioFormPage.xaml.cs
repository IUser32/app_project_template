using EmpleadosApp.Services;
using EmpleadosApp.ViewModels;

namespace EmpleadosApp.Views;

public partial class UsuarioFormPage : ContentPage
{
    public UsuarioFormPage()
    {
        InitializeComponent();
        BindingContext = new UsuarioFormViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EmpleadosService.InicializarAsync();
    }
}
