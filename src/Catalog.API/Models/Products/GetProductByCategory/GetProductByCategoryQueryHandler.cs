namespace Catalog.API.Models.Products.GetProductByCategory
{
    public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;

    public record GetProductByCategoryResult(IEnumerable<Product> Products);

    internal class GetProductByCategoryQueryHandler(IDocumentSession session)
        : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(
            GetProductByCategoryQuery query,
            CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>().ToListAsync(cancellationToken);

            var filteredProducts = products
                .Where(product => product.Category.Any(category =>
                    category.Equals(query.Category, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(product => product.Name)
                .ToList();

            return new GetProductByCategoryResult(filteredProducts);
        }
    }
}
