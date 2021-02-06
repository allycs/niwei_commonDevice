namespace Allycs.Common.Devices.Dtos
{
    public class CheckCodeCmd
    {
        public string MemberId { get; set; }
        public string CheckCode { get; set; }
        public string ClientIp { get; set; }
        public ClientType ClientType { get; set; } = ClientType.Mobile;
    }
}