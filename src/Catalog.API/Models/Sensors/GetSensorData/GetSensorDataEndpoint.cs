using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace Catalog.API.Models.Sensors.GetSensorData
{
    // The response has the latest sensor data or an empty object if no data
    public record GetSensorDataResponse(Sensor? SensorData);

    public class GetSensorDataEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/sensor/latest", async (ISender sender) =>
            {
                var result = await sender.Send(new GetSensorDataQuery());
                var response = result.Adapt<GetSensorDataResponse>();
                return Results.Ok(response);
            })
            .WithName("GetSensorDataLatest")
            .Produces<GetSensorDataResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Obtener la última lectura del sensor")
            .WithDescription("Retorna el último valor de distancia medido por el sensor ultrasónico");
        }
    }
}
