namespace Allycs.Common.Devices.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class LoginCmd
    {
        /// <summary>
        /// Account改为PhoneNumber
        /// </summary>
        [Required(ErrorMessage = "账号必填")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "长度1到30")]
        public string Account { get; set; }

        [Required(ErrorMessage = "密码必填")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "长度1到30")]
        public string Password { get; set; }

        public ClientType ClientType { get; set; } = ClientType.PC;
        public string RequestUrl { get; set; }
    }
}