using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models
{
    public class ErrorResponse
    {
        public List<Error> errors { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string title { get; set; }
        public string detail { get; set; }
        public int status { get; set; }
    }
}
