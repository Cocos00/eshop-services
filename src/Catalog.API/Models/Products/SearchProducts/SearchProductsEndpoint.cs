namespace Catalog.API.Models.Products.SearchProducts
{
    public record SearchProductsResponse(
        IEnumerable<Product> Products,
        int PageNumber,
        int PageSize,
        int TotalCount);

    public class SearchProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/search", async (
                string? name,
                int? pageNumber,
                int? pageSize,
                ISender sender) =>
            {
                var query = new SearchProductsQuery(name, pageNumber ?? 1, pageSize ?? 10);
                var result = await sender.Send(query);
                var response = result.Adapt<SearchProductsResponse>();

                return Results.Ok(response);
            })
            .WithName("SearchProducts")
            .Produces<SearchProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Buscar productos por nombre")
            .WithDescription("Busca productos por nombre y retorna resultados paginados.");
        }
    }
}
