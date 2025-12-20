using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.Auth.GoogleExchangeService;

namespace Domain.Models.Auth
{
    public class Callback
    {
        public string Token { get; set; }
        public GooglePayload User { get; set; }
    }
}
