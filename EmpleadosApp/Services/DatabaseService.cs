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

        var desarrollador = new Cargo { Nombre = "Desarrollador", SalarioBase = 1200, DepartamentoId = sistemas.Id };
        var soporte = new Cargo { Nombre = "Soporte técnico", SalarioBase = 900, DepartamentoId = sistemas.Id };
        var analistaRrhh = new Cargo { Nombre = "Analista de RRHH", SalarioBase = 1000, DepartamentoId = rrhh.Id };
        var contador = new Cargo { Nombre = "Contador", SalarioBase = 1100, DepartamentoId = contabilidad.Id };
        var asesor = new Cargo { Nombre = "Asesor de ventas", SalarioBase = 850, DepartamentoId = ventas.Id };

        await _connection.InsertAllAsync(new[] { desarrollador, soporte, analistaRrhh, contador, asesor });

        await _connection.InsertAllAsync(new[]
        {
            new Empleado { Nombre = "Ana", Apellido = "García", Cedula = "0102030405", FechaNacimiento = new DateTime(1992, 4, 12), Telefono = "0991112233", Correo = "ana.garcia@empresa.com", DepartamentoId = sistemas.Id, CargoId = desarrollador.Id, FechaIngreso = new DateTime(2021, 3, 1), Salario = 1400, Estado = "Activo" },
            new Empleado { Nombre = "Luis", Apellido = "Pérez", Cedula = "0203040506", FechaNacimiento = new DateTime(1995, 9, 23), Telefono = "0992223344", Correo = "luis.perez@empresa.com", DepartamentoId = sistemas.Id, CargoId = soporte.Id, FechaIngreso = new DateTime(2022, 7, 15), Salario = 950, Estado = "Activo" },
            new Empleado { Nombre = "María", Apellido = "López", Cedula = "0304050607", FechaNacimiento = new DateTime(1990, 1, 5), Telefono = "0993334455", Correo = "maria.lopez@empresa.com", DepartamentoId = rrhh.Id, CargoId = analistaRrhh.Id, FechaIngreso = new DateTime(2020, 11, 2), Salario = 1100, Estado = "Activo" },
            new Empleado { Nombre = "Carlos", Apellido = "Ruiz", Cedula = "0405060708", FechaNacimiento = new DateTime(1988, 6, 30), Telefono = "0994445566", Correo = "carlos.ruiz@empresa.com", DepartamentoId = contabilidad.Id, CargoId = contador.Id, FechaIngreso = new DateTime(2019, 5, 20), Salario = 1200, Estado = "Inactivo" },
            new Empleado { Nombre = "Sofía", Apellido = "Torres", Cedula = "0506070809", FechaNacimiento = new DateTime(1997, 12, 8), Telefono = "0995556677", Correo = "sofia.torres@empresa.com", DepartamentoId = ventas.Id, CargoId = asesor.Id, FechaIngreso = new DateTime(2023, 2, 10), Salario = 900, Estado = "Activo" },
            new Empleado { Nombre = "Diego", Apellido = "Mora", Cedula = "0607080910", FechaNacimiento = new DateTime(1993, 8, 17), Telefono = "0996667788", Correo = "diego.mora@empresa.com", DepartamentoId = ventas.Id, CargoId = asesor.Id, FechaIngreso = new DateTime(2021, 9, 5), Salario = 880, Estado = "Inactivo" },
        });
    }
}
