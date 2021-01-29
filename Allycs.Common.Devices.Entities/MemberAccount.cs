namespace Allycs.Common.Devices.Entities
{
    using Allycs.Core;
    using Dapper;

    public class MemberAccount
    {
        [Key]
        public string Account { get; set; }

        public string MemberId { get; set; }
        public bool IsLockout { get; set; }
        public string Password { get; set; }
        public PasswordFormatType PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
    }
}