using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmpleadosApp.Models;
using EmpleadosApp.Services;

namespace EmpleadosApp.ViewModels;

public partial class CargoFormViewModel : ObservableObject, IQueryAttributable
{
    private int _idEditando;

    [ObservableProperty] private bool modoEditar;
    [ObservableProperty] private string tituloPagina = "Nuevo cargo";

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string salarioBaseTexto = string.Empty;
    [ObservableProperty] private Departamento? departamentoSeleccionado;
    [ObservableProperty] private string? estadoSeleccionado = "Activo";

    [ObservableProperty] private string nombreError = string.Empty;
    [ObservableProperty] private string salarioBaseError = string.Empty;
    [ObservableProperty] private string departamentoError = string.Empty;
    [ObservableProperty] private string estadoError = string.Empty;

    public ObservableCollection<Departamento> DepartamentosDisponibles => DepartamentosService.Departamentos;
    public ObservableCollection<string> EstadosDisponibles { get; } = new() { "Activo", "Inactivo" };

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out var id) && id > 0)
        {
            var cargo = CargosService.ObtenerPorId(id);
            if (cargo is not null)
            {
                CargarDesde(cargo);
                return;
            }
        }

        ResetearParaNuevo();
    }

    private void CargarDesde(Cargo cargo)
    {
        _idEditando = cargo.Id;
        ModoEditar = true;
        TituloPagina = "Editar cargo";

        Nombre = cargo.Nombre;
        SalarioBaseTexto = cargo.SalarioBase.ToString(CultureInfo.InvariantCulture);
        DepartamentoSeleccionado = DepartamentosService.ObtenerPorId(cargo.DepartamentoId);
        EstadoSeleccionado = cargo.Estado;

        LimpiarErrores();
    }

    private void ResetearParaNuevo()
    {
        _idEditando = 0;
        ModoEditar = false;
        TituloPagina = "Nuevo cargo";

        Nombre = string.Empty;
        SalarioBaseTexto = string.Empty;
        DepartamentoSeleccionado = null;
        EstadoSeleccionado = "Activo";

        LimpiarErrores();
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar()) return;

        var cargo = new Cargo
        {
            Nombre = Nombre.Trim(),
            SalarioBase = decimal.Parse(SalarioBaseTexto, CultureInfo.InvariantCulture),
            DepartamentoId = DepartamentoSeleccionado!.Id,
            Estado = EstadoSeleccionado!
        };

        if (ModoEditar)
        {
            cargo.Id = _idEditando;
            await CargosService.ActualizarAsync(cargo);
            EmpleadosService.RefrescarNombresRelacionados();
            await Shell.Current.DisplayAlertAsync(
                "Cargo actualizado",
                $"{cargo.Nombre} fue actualizado correctamente.",
                "Aceptar");
        }
        else
        {
            await CargosService.AgregarAsync(cargo);
            await Shell.Current.DisplayAlertAsync(
                "Cargo registrado",
                $"{cargo.Nombre} fue agregado correctamente.",
                "Aceptar");
        }

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//cargos");
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (!ModoEditar) return;

        var tieneEmpleados = EmpleadosService.Empleados.Any(e => e.CargoId == _idEditando);
        if (tieneEmpleados)
        {
            await Shell.Current.DisplayAlertAsync(
                "No se puede eliminar",
                "Este cargo tiene empleados asignados. Reasigna esos empleados primero.",
                "Entendido");
            return;
        }

        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar cargo",
            $"¿Seguro que quieres eliminar el cargo {Nombre}?",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar) return;

        await CargosService.EliminarAsync(_idEditando);

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//cargos");
    }

    private bool Validar()
    {
        NombreError = string.IsNullOrWhiteSpace(Nombre) || Nombre.Trim().Length is < 2 or > 50
            ? "El nombre es obligatorio y debe tener entre 2 y 50 caracteres."
            : string.Empty;

        var salarioOk = decimal.TryParse(SalarioBaseTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out var salario)
            && salario > 0;
        SalarioBaseError = salarioOk ? string.Empty : "El salario base debe ser un número mayor a 0 (usa punto como separador decimal).";

        DepartamentoError = DepartamentoSeleccionado is null
            ? "Selecciona un departamento."
            : string.Empty;

        EstadoError = string.IsNullOrWhiteSpace(EstadoSeleccionado)
            ? "Selecciona un estado."
            : string.Empty;

        return string.IsNullOrEmpty(NombreError)
            && string.IsNullOrEmpty(SalarioBaseError)
            && string.IsNullOrEmpty(DepartamentoError)
            && string.IsNullOrEmpty(EstadoError);
    }

    private void LimpiarErrores()
    {
        NombreError = string.Empty;
        SalarioBaseError = string.Empty;
        DepartamentoError = string.Empty;
        EstadoError = string.Empty;
    }
}
