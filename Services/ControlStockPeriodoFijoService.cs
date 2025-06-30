using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // importante

namespace Proyect_InvOperativa.Services
{
    public class ControlStockPeriodoFijoService : BackgroundService
    {
        private readonly ILogger<ControlStockPeriodoFijoService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _intervalo;

        public ControlStockPeriodoFijoService(
            ILogger<ControlStockPeriodoFijoService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _intervalo = TimeSpan.FromMinutes(30); // cambiá esto si querés
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ControlStockPeriodoFijoService iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var maestroArticuloService = scope.ServiceProvider.GetRequiredService<MaestroArticulosService>();

                    await maestroArticuloService.ControlStockPeriodico(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al ejecutar ControlStock periódicamente");
                }

                await Task.Delay(_intervalo, stoppingToken);
            }

            _logger.LogInformation("ControlStockPeriodoFijoService detenido");
        }
    }
}