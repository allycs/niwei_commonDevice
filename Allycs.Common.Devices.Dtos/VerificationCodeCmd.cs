using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Dtos
{
    public class VerificationCodeCmd
    {
        public string MemberId { get; set; }
        public CodeType CodeType { get; set; } = CodeType.All;
        public string Code { get; set; }
    }
}
