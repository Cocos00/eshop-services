namespace Catalog.API.Models.Products.DeleteProduct
{
    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{name}", async (string name, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductCommand(name));

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(new { Message = $"Product '{name}' was not found." });
            })
            .WithName("DeleteProductByName")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Eliminar producto por nombre")
            .WithDescription("Elimina un producto existente usando su nombre.");
        }
    }
}
