using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Dtos
{
    public class RegistCmd
    {
        public string Realname { get; set; }
        public SexType Sex { get; set; } = SexType.Unknown;
        public string Alias { get; set; }
        public string Avatar { get; set; }
        public string Account { get; set; }
        public string Telephone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; }
        public string RegistCode { get; set; }
        public string CheckCode { get; set; }
        public CodeType CodeType { get; set; }
        public string Password { get; set; }
    }
}
