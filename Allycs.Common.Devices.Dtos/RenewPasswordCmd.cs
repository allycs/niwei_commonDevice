using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Dtos
{
    public class RenewPasswordCmd
    {
        public string OldPassword { get; set; }
        public string CheckCode { get; set; }
        public CodeType CodeType { get; set; }
        public string Password { get; set; }
    }
}
