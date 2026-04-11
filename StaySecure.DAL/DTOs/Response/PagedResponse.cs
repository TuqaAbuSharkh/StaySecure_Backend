using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}
