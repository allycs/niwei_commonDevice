using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Entities
{
    public class MemberToken
    {
        /// <summary>
        /// ObjectId
        /// </summary>
        [Key]
        public string Token { get; set; }
        /// <summary>
        /// ObjectId
        /// </summary>
        public string MemberId { get; set; }
        public string ClientIp { get; set; }
        public ClientType ClientType { get; set; }
        public bool IsDisabled { get; set; }
        public string Reason { get; set; }
        public DateTime? DisabledOn { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
