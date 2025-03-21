using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Functions2025.Models.School;
using Microsoft.EntityFrameworkCore;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Read connection string from configuration
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        
        // Configure DbContext with SQL Server
        services.AddDbContext<SchoolContext>(options =>
            options.UseSqlServer(connectionString));
    })
    .Build();

host.Run();