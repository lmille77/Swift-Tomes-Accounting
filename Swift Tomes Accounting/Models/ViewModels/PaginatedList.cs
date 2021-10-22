using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }
        public PaginatedList(List<T> items1, List<T> items2, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            
            TotalPages = (int)Math.Ceiling((count) / ((double)pageSize));

            this.AddRange(items1);
            this.AddRange(items2);
        }
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source1, IQueryable<T> source2, int pageIndex, int pageSize)
        {
            var count = await source1.CountAsync() + await source2.CountAsync();
            var items = await source1.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            items.AddRange(await source2.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
