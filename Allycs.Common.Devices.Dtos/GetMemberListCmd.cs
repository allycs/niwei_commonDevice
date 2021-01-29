using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Dtos
{
    public class GetMemberListCmd
    {
        public MemberType? Type { get; set; }
        public MemberLevel? Level { get; set; }
        public string RefereeId { get; set; }
        public string Realname { get; set; }
        public string MobilePhone { get; set; }
    }
}
