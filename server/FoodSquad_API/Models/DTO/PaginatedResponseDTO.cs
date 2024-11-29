using System.Collections.Generic;

namespace FoodSquad_API.Models.DTO
{
    public class PaginatedResponseDTO<T>
    {
        public List<T> Items { get; set; }
        public long TotalCount { get; set; }

        public PaginatedResponseDTO(List<T> items, long totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
