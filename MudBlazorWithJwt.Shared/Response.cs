using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazorWithJwt.Shared
{
    public class Response
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Error { get; set; }
        public string Token { get; set; }
    }
}
