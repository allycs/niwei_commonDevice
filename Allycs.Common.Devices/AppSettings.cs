

namespace Allycs.Common.Devices
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string DBSchema { get; set; }
        public string UploadImageDirectory { get; set; }
        public int MemberTokenDisableTime { get; set; }
        public int VerificationCodeDisableTime { get; set; }
    }
}
