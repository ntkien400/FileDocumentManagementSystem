namespace FileDocumentManagementSystem.Helpers
{
    public static class PaginationHelper
    {
        private const int PageSize = 5;
        public static PaginationResult<T> Paginate<T>(IEnumerable<T> items, int? pageIndex)
        {
            int currentPage = pageIndex ?? 1;
            var totalItems = items.Count();
            
            var pagedItems = items
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var paginationResult = new PaginationResult<T>
            {
                TotalItems = totalItems,
                CurrentPage = currentPage,
                PageSize = PageSize,
                Items = pagedItems
            };

            return paginationResult;
        }
    }

    public class PaginationResult<T>
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
    }

}
