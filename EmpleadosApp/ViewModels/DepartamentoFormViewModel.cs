using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmpleadosApp.Models;
using EmpleadosApp.Services;
using SQLite;

namespace EmpleadosApp.ViewModels;

public partial class DepartamentoFormViewModel : ObservableObject, IQueryAttributable
{
    private int _idEditando;

    [ObservableProperty] private bool modoEditar;
    [ObservableProperty] private string tituloPagina = "Nuevo departamento";

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string descripcion = string.Empty;
    [ObservableProperty] private string? estadoSeleccionado = "Activo";

    [ObservableProperty] private string nombreError = string.Empty;
    [ObservableProperty] private string descripcionError = string.Empty;
    [ObservableProperty] private string estadoError = string.Empty;

    public ObservableCollection<string> EstadosDisponibles { get; } = new() { "Activo", "Inactivo" };

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out var id) && id > 0)
        {
            var departamento = DepartamentosService.ObtenerPorId(id);
            if (departamento is not null)
            {
                CargarDesde(departamento);
                return;
            }
        }

        ResetearParaNuevo();
    }

    private void CargarDesde(Departamento departamento)
    {
        _idEditando = departamento.Id;
        ModoEditar = true;
        TituloPagina = "Editar departamento";

        Nombre = departamento.Nombre;
        Descripcion = departamento.Descripcion;
        EstadoSeleccionado = departamento.Estado;

        LimpiarErrores();
    }

    private void ResetearParaNuevo()
    {
        _idEditando = 0;
        ModoEditar = false;
        TituloPagina = "Nuevo departamento";

        Nombre = string.Empty;
        Descripcion = string.Empty;
        EstadoSeleccionado = "Activo";

        LimpiarErrores();
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar()) return;

        var departamento = new Departamento
        {
            Nombre = Nombre.Trim(),
            Descripcion = Descripcion.Trim(),
            Estado = EstadoSeleccionado!
        };

        try
        {
            if (ModoEditar)
            {
                departamento.Id = _idEditando;
                await DepartamentosService.ActualizarAsync(departamento);
                EmpleadosService.RefrescarNombresRelacionados();
                await Shell.Current.DisplayAlertAsync(
                    "Departamento actualizado",
                    $"{departamento.Nombre} fue actualizado correctamente.",
                    "Aceptar");
            }
            else
            {
                await DepartamentosService.AgregarAsync(departamento);
                await Shell.Current.DisplayAlertAsync(
                    "Departamento registrado",
                    $"{departamento.Nombre} fue agregado correctamente.",
                    "Aceptar");
            }
        }
        catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
        {
            NombreError = "Ya existe un departamento con ese nombre.";
            return;
        }

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//departamentos");
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (!ModoEditar) return;

        var tieneCargos = CargosService.PorDepartamento(_idEditando).Any();
        if (tieneCargos)
        {
            await Shell.Current.DisplayAlertAsync(
                "No se puede eliminar",
                "Este departamento tiene cargos asociados. Elimina o reasigna esos cargos primero.",
                "Entendido");
            return;
        }

        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar departamento",
            $"¿Seguro que quieres eliminar el departamento {Nombre}?",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar) return;

        await DepartamentosService.EliminarAsync(_idEditando);
        EmpleadosService.RefrescarNombresRelacionados();

        ResetearParaNuevo();
        await Shell.Current.GoToAsync("//departamentos");
    }

    private bool Validar()
    {
        NombreError = string.IsNullOrWhiteSpace(Nombre) || Nombre.Trim().Length is < 2 or > 50
            ? "El nombre es obligatorio y debe tener entre 2 y 50 caracteres."
            : string.Empty;

        DescripcionError = Descripcion.Length > 200
            ? "La descripción no puede superar 200 caracteres."
            : string.Empty;

        EstadoError = string.IsNullOrWhiteSpace(EstadoSeleccionado)
            ? "Selecciona un estado."
            : string.Empty;

        return string.IsNullOrEmpty(NombreError)
            && string.IsNullOrEmpty(DescripcionError)
            && string.IsNullOrEmpty(EstadoError);
    }

    private void LimpiarErrores()
    {
        NombreError = string.Empty;
        DescripcionError = string.Empty;
        EstadoError = string.Empty;
    }
}
