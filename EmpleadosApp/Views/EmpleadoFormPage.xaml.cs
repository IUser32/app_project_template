using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.Views;

public partial class EmpleadoFormPage : ContentPage
{
    public EmpleadoFormPage()
    {
        InitializeComponent();
        InicializarFechas();
    }

    private void InicializarFechas()
    {
        FechaNacimientoPicker.Date = DateTime.Today.AddYears(-25);
        FechaIngresoPicker.Date = DateTime.Today;
    }

    private async void OnGuardarClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(ApellidoEntry.Text) ||
            string.IsNullOrWhiteSpace(CedulaEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoEntry.Text) ||
            string.IsNullOrWhiteSpace(CorreoEntry.Text) ||
            string.IsNullOrWhiteSpace(CargoEntry.Text) ||
            string.IsNullOrWhiteSpace(DepartamentoEntry.Text) ||
            string.IsNullOrWhiteSpace(SalarioEntry.Text) ||
            EstadoPicker.SelectedIndex < 0)
        {
            await DisplayAlertAsync(
                "Datos incompletos",
                "Por favor completa todos los campos.",
                "Aceptar");
            return;
        }

        if (!decimal.TryParse(SalarioEntry.Text, out var salario) || salario <= 0)
        {
            await DisplayAlertAsync(
                "Salario inválido",
                "El salario debe ser un número mayor a 0.",
                "Aceptar");
            return;
        }

        var empleado = new Empleado
        {
            Nombre = NombreEntry.Text.Trim(),
            Apellido = ApellidoEntry.Text.Trim(),
            Cedula = CedulaEntry.Text.Trim(),
            FechaNacimiento = FechaNacimientoPicker.Date ?? DateTime.Today,
            Telefono = TelefonoEntry.Text.Trim(),
            Correo = CorreoEntry.Text.Trim(),
            Cargo = CargoEntry.Text.Trim(),
            Departamento = DepartamentoEntry.Text.Trim(),
            FechaIngreso = FechaIngresoPicker.Date ?? DateTime.Today,
            Salario = salario,
            Estado = (string)EstadoPicker.SelectedItem
        };

        EmpleadosService.Agregar(empleado);

        await DisplayAlertAsync(
            "Empleado registrado",
            $"{empleado.NombreCompleto} fue agregado correctamente.",
            "Aceptar");

        LimpiarFormulario();
        await Shell.Current.GoToAsync("//empleados");
    }

    private void LimpiarFormulario()
    {
        NombreEntry.Text = string.Empty;
        ApellidoEntry.Text = string.Empty;
        CedulaEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        CorreoEntry.Text = string.Empty;
        CargoEntry.Text = string.Empty;
        DepartamentoEntry.Text = string.Empty;
        SalarioEntry.Text = string.Empty;
        EstadoPicker.SelectedIndex = -1;
        InicializarFechas();
    }
}
