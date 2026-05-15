using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.ViewModels;

public partial class EmpleadoFormViewModel : ObservableObject, IQueryAttributable
{
    private static readonly Regex SoloLetras = new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");
    private static readonly Regex DiezDigitos = new(@"^\d{10}$");
    private static readonly Regex Email = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private int _idEditando;

    [ObservableProperty] private bool modoEditar;
    [ObservableProperty] private string tituloPagina = "Nuevo empleado";

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string apellido = string.Empty;
    [ObservableProperty] private string cedula = string.Empty;
    [ObservableProperty] private DateTime fechaNacimiento = DateTime.Today.AddYears(-25);
    [ObservableProperty] private string telefono = string.Empty;
    [ObservableProperty] private string correo = string.Empty;
    [ObservableProperty] private string cargo = string.Empty;
    [ObservableProperty] private string departamento = string.Empty;
    [ObservableProperty] private DateTime fechaIngreso = DateTime.Today;
    [ObservableProperty] private string salarioTexto = string.Empty;
    [ObservableProperty] private string? estadoSeleccionado;

    [ObservableProperty] private string nombreError = string.Empty;
    [ObservableProperty] private string apellidoError = string.Empty;
    [ObservableProperty] private string cedulaError = string.Empty;
    [ObservableProperty] private string fechaNacimientoError = string.Empty;
    [ObservableProperty] private string telefonoError = string.Empty;
    [ObservableProperty] private string correoError = string.Empty;
    [ObservableProperty] private string cargoError = string.Empty;
    [ObservableProperty] private string departamentoError = string.Empty;
    [ObservableProperty] private string fechaIngresoError = string.Empty;
    [ObservableProperty] private string salarioError = string.Empty;
    [ObservableProperty] private string estadoError = string.Empty;

    public ObservableCollection<string> EstadosDisponibles { get; } = new() { "Activo", "Inactivo" };

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out var id) && id > 0)
        {
            var empleado = EmpleadosService.ObtenerPorId(id);
            if (empleado is not null)
            {
                CargarDesde(empleado);
                return;
            }
        }

        ResetearParaNuevo();
    }

    private void CargarDesde(Empleado empleado)
    {
        _idEditando = empleado.Id;
        ModoEditar = true;
        TituloPagina = "Editar empleado";

        Nombre = empleado.Nombre;
        Apellido = empleado.Apellido;
        Cedula = empleado.Cedula;
        FechaNacimiento = empleado.FechaNacimiento;
        Telefono = empleado.Telefono;
        Correo = empleado.Correo;
        Cargo = empleado.Cargo;
        Departamento = empleado.Departamento;
        FechaIngreso = empleado.FechaIngreso;
        SalarioTexto = empleado.Salario.ToString();
        EstadoSeleccionado = empleado.Estado;

        LimpiarErrores();
    }

    private void ResetearParaNuevo()
    {
        _idEditando = 0;
        ModoEditar = false;
        TituloPagina = "Nuevo empleado";
        Limpiar();
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar()) return;

        var empleado = ConstruirEmpleadoDesdeForm();

        if (ModoEditar)
        {
            empleado.Id = _idEditando;
            EmpleadosService.Actualizar(empleado);
            await Shell.Current.DisplayAlertAsync(
                "Empleado actualizado",
                $"{empleado.NombreCompleto} fue actualizado correctamente.",
                "Aceptar");
        }
        else
        {
            EmpleadosService.Agregar(empleado);
            await Shell.Current.DisplayAlertAsync(
                "Empleado registrado",
                $"{empleado.NombreCompleto} fue agregado correctamente.",
                "Aceptar");
        }

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//empleados");
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (!ModoEditar) return;

        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar empleado",
            $"¿Seguro que quieres eliminar a {Nombre} {Apellido}?",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar) return;

        EmpleadosService.Eliminar(_idEditando);

        await Shell.Current.DisplayAlertAsync(
            "Empleado eliminado",
            "El empleado fue eliminado correctamente.",
            "Aceptar");

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//empleados");
    }

    private Empleado ConstruirEmpleadoDesdeForm() => new()
    {
        Nombre = Nombre.Trim(),
        Apellido = Apellido.Trim(),
        Cedula = Cedula.Trim(),
        FechaNacimiento = FechaNacimiento,
        Telefono = Telefono.Trim(),
        Correo = Correo.Trim(),
        Cargo = Cargo.Trim(),
        Departamento = Departamento.Trim(),
        FechaIngreso = FechaIngreso,
        Salario = decimal.Parse(SalarioTexto),
        Estado = EstadoSeleccionado!
    };

    private bool Validar()
    {
        ValidarNombre();
        ValidarApellido();
        ValidarCedula();
        ValidarFechaNacimiento();
        ValidarTelefono();
        ValidarCorreo();
        ValidarCargo();
        ValidarDepartamento();
        ValidarFechaIngreso();
        ValidarSalario();
        ValidarEstado();

        return new[]
        {
            NombreError, ApellidoError, CedulaError, FechaNacimientoError,
            TelefonoError, CorreoError, CargoError, DepartamentoError,
            FechaIngresoError, SalarioError, EstadoError
        }.All(string.IsNullOrEmpty);
    }

    private void ValidarNombre()
    {
        if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Trim().Length is < 2 or > 50)
            NombreError = "El nombre es obligatorio y debe tener entre 2 y 50 caracteres.";
        else if (!SoloLetras.IsMatch(Nombre.Trim()))
            NombreError = "El nombre solo puede contener letras y espacios.";
        else
            NombreError = string.Empty;
    }

    private void ValidarApellido()
    {
        if (string.IsNullOrWhiteSpace(Apellido) || Apellido.Trim().Length is < 2 or > 50)
            ApellidoError = "El apellido es obligatorio y debe tener entre 2 y 50 caracteres.";
        else if (!SoloLetras.IsMatch(Apellido.Trim()))
            ApellidoError = "El apellido solo puede contener letras y espacios.";
        else
            ApellidoError = string.Empty;
    }

    private void ValidarCedula()
    {
        CedulaError = DiezDigitos.IsMatch(Cedula ?? "")
            ? string.Empty
            : "La cédula debe tener exactamente 10 dígitos.";
    }

    private void ValidarFechaNacimiento()
    {
        if (FechaNacimiento.Date >= DateTime.Today)
        {
            FechaNacimientoError = "La fecha de nacimiento debe ser anterior a hoy.";
            return;
        }

        var edad = DateTime.Today.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

        FechaNacimientoError = edad < 18
            ? "El empleado debe tener al menos 18 años."
            : string.Empty;
    }

    private void ValidarTelefono()
    {
        TelefonoError = DiezDigitos.IsMatch(Telefono ?? "")
            ? string.Empty
            : "El teléfono debe tener exactamente 10 dígitos.";
    }

    private void ValidarCorreo()
    {
        CorreoError = Email.IsMatch(Correo ?? "")
            ? string.Empty
            : "El correo no tiene un formato válido.";
    }

    private void ValidarCargo()
    {
        CargoError = string.IsNullOrWhiteSpace(Cargo) || Cargo.Trim().Length is < 2 or > 50
            ? "El cargo es obligatorio y debe tener entre 2 y 50 caracteres."
            : string.Empty;
    }

    private void ValidarDepartamento()
    {
        DepartamentoError = string.IsNullOrWhiteSpace(Departamento) || Departamento.Trim().Length is < 2 or > 50
            ? "El departamento es obligatorio y debe tener entre 2 y 50 caracteres."
            : string.Empty;
    }

    private void ValidarFechaIngreso()
    {
        FechaIngresoError = FechaIngreso.Date > DateTime.Today
            ? "La fecha de ingreso no puede ser futura."
            : string.Empty;
    }

    private void ValidarSalario()
    {
        SalarioError = decimal.TryParse(SalarioTexto, out var salario) && salario > 0
            ? string.Empty
            : "El salario debe ser un número mayor a 0.";
    }

    private void ValidarEstado()
    {
        EstadoError = string.IsNullOrWhiteSpace(EstadoSeleccionado)
            ? "Selecciona un estado."
            : string.Empty;
    }

    private void Limpiar()
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        Cedula = string.Empty;
        FechaNacimiento = DateTime.Today.AddYears(-25);
        Telefono = string.Empty;
        Correo = string.Empty;
        Cargo = string.Empty;
        Departamento = string.Empty;
        FechaIngreso = DateTime.Today;
        SalarioTexto = string.Empty;
        EstadoSeleccionado = null;

        LimpiarErrores();
    }

    private void LimpiarErrores()
    {
        NombreError = string.Empty;
        ApellidoError = string.Empty;
        CedulaError = string.Empty;
        FechaNacimientoError = string.Empty;
        TelefonoError = string.Empty;
        CorreoError = string.Empty;
        CargoError = string.Empty;
        DepartamentoError = string.Empty;
        FechaIngresoError = string.Empty;
        SalarioError = string.Empty;
        EstadoError = string.Empty;
    }
}
