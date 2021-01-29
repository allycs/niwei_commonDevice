using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Entities
{
    public class MemberLoginLog
    {
        [Key]
        public int Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public DateTime CheckOn { get; set; }
        public bool IsPass { get; set; }
        public string Reason { get; set; }
        public string ClientIp { get; set; }
        public DateTime? PassOn { get; set; }
    }
}
