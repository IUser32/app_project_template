using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmpleadosApp.Models;
using EmpleadosApp.Services;
using SQLite;

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
    [ObservableProperty] private Departamento? departamentoSeleccionado;
    [ObservableProperty] private Cargo? cargoSeleccionado;
    [ObservableProperty] private DateTime fechaIngreso = DateTime.Today;
    [ObservableProperty] private string salarioTexto = string.Empty;
    [ObservableProperty] private string? estadoSeleccionado;

    [ObservableProperty] private string nombreError = string.Empty;
    [ObservableProperty] private string apellidoError = string.Empty;
    [ObservableProperty] private string cedulaError = string.Empty;
    [ObservableProperty] private string fechaNacimientoError = string.Empty;
    [ObservableProperty] private string telefonoError = string.Empty;
    [ObservableProperty] private string correoError = string.Empty;
    [ObservableProperty] private string departamentoError = string.Empty;
    [ObservableProperty] private string cargoError = string.Empty;
    [ObservableProperty] private string fechaIngresoError = string.Empty;
    [ObservableProperty] private string salarioError = string.Empty;
    [ObservableProperty] private string estadoError = string.Empty;

    public ObservableCollection<Departamento> DepartamentosDisponibles => DepartamentosService.Departamentos;
    public ObservableCollection<Cargo> CargosDisponibles { get; } = new();
    public ObservableCollection<string> EstadosDisponibles { get; } = new() { "Activo", "Inactivo" };

    partial void OnDepartamentoSeleccionadoChanged(Departamento? value)
    {
        ActualizarCargosDisponibles(value);

        if (CargoSeleccionado is not null && CargoSeleccionado.DepartamentoId != value?.Id)
        {
            CargoSeleccionado = null;
        }
    }

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
        DepartamentoSeleccionado = DepartamentosService.ObtenerPorId(empleado.DepartamentoId);
        CargoSeleccionado = CargosService.ObtenerPorId(empleado.CargoId);
        FechaIngreso = empleado.FechaIngreso;
        SalarioTexto = empleado.Salario.ToString(CultureInfo.InvariantCulture);
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

        try
        {
            if (ModoEditar)
            {
                empleado.Id = _idEditando;
                await EmpleadosService.ActualizarAsync(empleado);
                await Shell.Current.DisplayAlertAsync(
                    "Empleado actualizado",
                    $"{empleado.NombreCompleto} fue actualizado correctamente.",
                    "Aceptar");
            }
            else
            {
                await EmpleadosService.AgregarAsync(empleado);
                await Shell.Current.DisplayAlertAsync(
                    "Empleado registrado",
                    $"{empleado.NombreCompleto} fue agregado correctamente.",
                    "Aceptar");
            }
        }
        catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
        {
            CedulaError = "Ya existe un empleado con esta cédula.";
            return;
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

        await EmpleadosService.EliminarAsync(_idEditando);

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
        DepartamentoId = DepartamentoSeleccionado!.Id,
        CargoId = CargoSeleccionado!.Id,
        FechaIngreso = FechaIngreso,
        Salario = decimal.Parse(SalarioTexto, CultureInfo.InvariantCulture),
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
        ValidarDepartamento();
        ValidarCargo();
        ValidarFechaIngreso();
        ValidarSalario();
        ValidarEstado();

        return new[]
        {
            NombreError, ApellidoError, CedulaError, FechaNacimientoError,
            TelefonoError, CorreoError, DepartamentoError, CargoError,
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

    private void ValidarDepartamento()
    {
        DepartamentoError = DepartamentoSeleccionado is null
            ? "Selecciona un departamento."
            : string.Empty;
    }

    private void ValidarCargo()
    {
        CargoError = CargoSeleccionado is null
            ? "Selecciona un cargo."
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
        var ok = decimal.TryParse(SalarioTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out var salario)
            && salario > 0;
        SalarioError = ok ? string.Empty : "El salario debe ser un número mayor a 0 (usa punto como separador decimal).";
    }

    private void ValidarEstado()
    {
        EstadoError = string.IsNullOrWhiteSpace(EstadoSeleccionado)
            ? "Selecciona un estado."
            : string.Empty;
    }

    private void ActualizarCargosDisponibles(Departamento? departamento)
    {
        CargosDisponibles.Clear();

        if (departamento is null) return;

        foreach (var cargo in CargosService.PorDepartamento(departamento.Id))
        {
            CargosDisponibles.Add(cargo);
        }
    }

    private void Limpiar()
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        Cedula = string.Empty;
        FechaNacimiento = DateTime.Today.AddYears(-25);
        Telefono = string.Empty;
        Correo = string.Empty;
        DepartamentoSeleccionado = null;
        CargoSeleccionado = null;
        FechaIngreso = DateTime.Today;
        SalarioTexto = string.Empty;
        EstadoSeleccionado = null;

        CargosDisponibles.Clear();
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
        DepartamentoError = string.Empty;
        CargoError = string.Empty;
        FechaIngresoError = string.Empty;
        SalarioError = string.Empty;
        EstadoError = string.Empty;
    }
}
