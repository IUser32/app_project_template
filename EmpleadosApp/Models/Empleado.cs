using SQLite;

namespace EmpleadosApp.Models;

[Table("Empleados")]
public class Empleado
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    [Unique]
    public string Cedula { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public decimal Salario { get; set; }
    public string Estado { get; set; } = "Activo";

    [Ignore]
    public string NombreCompleto => $"{Nombre} {Apellido}";
}
