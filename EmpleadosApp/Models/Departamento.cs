using SQLite;

namespace EmpleadosApp.Models;

[Table("Departamentos")]
public class Departamento
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
}
