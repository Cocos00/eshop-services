namespace Catalog.API.Models.Products.UpdateProduct
{
    public record UpdateProductRequest(
        string Description,
        List<string> Category,
        string ImageFiles,
        decimal Price);

    public record UpdateProductResponse(bool IsSuccess);

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products/{name}", async (
                string name,
                UpdateProductRequest request,
                ISender sender) =>
            {
                var command = new UpdateProductCommand(
                    name,
                    request.Description,
                    request.Category,
                    request.ImageFiles,
                    request.Price);

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Adapt<UpdateProductResponse>())
                    : Results.NotFound(new { Message = $"Product '{name}' was not found." });
            })
            .WithName("UpdateProductByName")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Actualizar producto por nombre")
            .WithDescription("Actualiza un producto existente usando su nombre.");
        }
    }
}
