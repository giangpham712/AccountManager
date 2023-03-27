using System.Collections.Generic;

namespace AccountManager.Application
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int StartIndex { get; set; }
        public int Limit { get; set; }
        public bool HasMore { get; set; }
    }
}