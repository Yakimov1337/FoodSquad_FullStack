using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodSquad_API.Utilities
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; private set; }
        public int TotalCount { get; private set; }
        public int PageSize { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PaginatedList(List<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        public static PaginatedList<T> Create(IEnumerable<T> source, int page, int pageSize)
        {
            var totalCount = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, totalCount, page, pageSize);
        }
    }
}
