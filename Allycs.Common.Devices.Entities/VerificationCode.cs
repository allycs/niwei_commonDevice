using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Entities
{
    using Dapper;
    using System;

    public class VerificationCode
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// ObjectId
        /// </summary>
        public string MemberId { get; set; }
        public string Code { get; set; }
        public CodeType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDisabled { get; set; } = false;
        public string Reason { get; set; }
        public DateTime? DisabledOn { get; set; }
    }
}
