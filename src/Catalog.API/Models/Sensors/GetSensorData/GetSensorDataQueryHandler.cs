using BuildingBlocks.CQRS;
using Marten;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.API.Models.Sensors.GetSensorData
{
    public record GetSensorDataQuery() : IQuery<GetSensorDataResult>;
    
    public record GetSensorDataResult(Sensor? SensorData);

    internal class GetSensorDataQueryHandler
        (IDocumentSession session, ILogger<GetSensorDataQueryHandler> logger)
        : IQueryHandler<GetSensorDataQuery, GetSensorDataResult>
    {
        public async Task<GetSensorDataResult> Handle(GetSensorDataQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetSensorDataQueryHandler.Handle llamado");
            
            // Get the most recent sensor reading
            var latestSensor = await session.Query<Sensor>()
                                            .OrderByDescending(x => x.Timestamp)
                                            .FirstOrDefaultAsync(cancellationToken);
                                            
            return new GetSensorDataResult(latestSensor);
        }
    }
}
