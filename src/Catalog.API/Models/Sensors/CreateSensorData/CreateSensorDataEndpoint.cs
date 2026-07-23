using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace Catalog.API.Models.Sensors.CreateSensorData
{
    public record CreateSensorDataRequest(double Distance);

    public record CreateSensorDataResponse(Guid Id);

    public class CreateSensorDataEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/sensor", async (CreateSensorDataRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateSensorDataCommand>();

                var result = await sender.Send(command);
                var response = result.Adapt<CreateSensorDataResponse>();
                return Results.Created($"/api/sensor/{response.Id}", response);
            })
            .WithName("CreateSensorData")
            .Produces<CreateSensorDataResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Registrar nueva lectura de sensor")
            .WithDescription("Crea un nuevo registro con la distancia medida por el sensor y retorna el identificador");
        }
    }
}
