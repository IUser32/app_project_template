using EmpleadosApp.ViewModels;

namespace EmpleadosApp.Views;

public partial class EmpleadoFormPage : ContentPage
{
    public EmpleadoFormPage()
    {
        InitializeComponent();
        BindingContext = new EmpleadoFormViewModel();
    }
}
