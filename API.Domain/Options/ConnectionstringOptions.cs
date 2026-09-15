using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Domain.Options
{
    public class ConnectionstringOptions
    {
        public const string SectionName = "ConnectionStrings";
        public string ConnectString { get; set; }
    }
}
