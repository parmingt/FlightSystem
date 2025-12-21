using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models
{
    public class ErrorResponse
    {
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public int StatusCode { get; set; }
    }
}
