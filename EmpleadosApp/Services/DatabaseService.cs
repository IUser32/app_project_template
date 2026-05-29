using EmpleadosApp.Models;
using SQLite;

namespace EmpleadosApp.Services;

public static class DatabaseService
{
    private static SQLiteAsyncConnection? _connection;
    private static bool _inicializado;

    public static SQLiteAsyncConnection Connection => _connection
        ?? throw new InvalidOperationException("Llama a DatabaseService.InicializarAsync antes de usar la base de datos.");

    public static async Task InicializarAsync()
    {
        if (_inicializado) return;

        var ruta = Path.Combine(FileSystem.AppDataDirectory, "empleados.db3");
        _connection = new SQLiteAsyncConnection(ruta);

        await _connection.CreateTableAsync<Departamento>();
        await _connection.CreateTableAsync<Cargo>();
        await _connection.CreateTableAsync<Empleado>();
        await _connection.CreateTableAsync<Usuario>();

        await SembrarDatosBaseAsync();

        _inicializado = true;
    }

    private static async Task SembrarDatosBaseAsync()
    {
        var hayUsuarios = await _connection!.Table<Usuario>().CountAsync() > 0;
        if (!hayUsuarios)
        {
            await _connection.InsertAsync(new Usuario
            {
                Username = "admin",
                PasswordHash = UsuariosService.HashPassword("1234"),
                NombreCompleto = "Administrador",
                Estado = "Activo"
            });
        }

        var hayDepartamentos = await _connection.Table<Departamento>().CountAsync() > 0;
        if (hayDepartamentos) return;

        var sistemas = new Departamento { Nombre = "Sistemas", Descripcion = "Tecnología e infraestructura" };
        var rrhh = new Departamento { Nombre = "Recursos Humanos", Descripcion = "Gestión del talento" };
        var contabilidad = new Departamento { Nombre = "Contabilidad", Descripcion = "Finanzas y presupuesto" };
        var ventas = new Departamento { Nombre = "Ventas", Descripcion = "Atención comercial" };

        await _connection.InsertAllAsync(new[] { sistemas, rrhh, contabilidad, ventas });

        await _connection.InsertAllAsync(new[]
        {
            new Cargo { Nombre = "Desarrollador", SalarioBase = 1200, DepartamentoId = sistemas.Id },
            new Cargo { Nombre = "Soporte técnico", SalarioBase = 900, DepartamentoId = sistemas.Id },
            new Cargo { Nombre = "Analista de RRHH", SalarioBase = 1000, DepartamentoId = rrhh.Id },
            new Cargo { Nombre = "Contador", SalarioBase = 1100, DepartamentoId = contabilidad.Id },
            new Cargo { Nombre = "Asesor de ventas", SalarioBase = 850, DepartamentoId = ventas.Id },
        });
    }
}
