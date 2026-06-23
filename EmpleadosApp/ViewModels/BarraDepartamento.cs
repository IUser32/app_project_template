namespace EmpleadosApp.ViewModels;

public class BarraDepartamento
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }

    // Pesos para columnas star: la barra mas larga llena todo el ancho.
    public double Peso { get; set; }
    public double PesoResto { get; set; }
}
