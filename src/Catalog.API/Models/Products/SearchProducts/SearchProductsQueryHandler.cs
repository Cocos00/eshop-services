namespace Catalog.API.Models.Products.SearchProducts
{
    public record SearchProductsQuery(string? Name, int PageNumber, int PageSize)
        : IQuery<SearchProductsResult>;

    public record SearchProductsResult(
        IEnumerable<Product> Products,
        int PageNumber,
        int PageSize,
        int TotalCount);

    internal class SearchProductsQueryHandler(IDocumentSession session)
        : IQueryHandler<SearchProductsQuery, SearchProductsResult>
    {
        public async Task<SearchProductsResult> Handle(
            SearchProductsQuery query,
            CancellationToken cancellationToken)
        {
            var pageNumber = Math.Max(query.PageNumber, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);
            var name = query.Name?.Trim();

            var products = await session.Query<Product>().ToListAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(name))
            {
                products = products
                    .Where(product => product.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalCount = products.Count;
            var pagedProducts = products
                .OrderBy(product => product.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new SearchProductsResult(pagedProducts, pageNumber, pageSize, totalCount);
        }
    }
}
