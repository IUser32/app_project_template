using SQLite;

namespace EmpleadosApp.Models;

[Table("Cargos")]
public class Cargo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public decimal SalarioBase { get; set; }
    public int DepartamentoId { get; set; }
    public string Estado { get; set; } = "Activo";
}
