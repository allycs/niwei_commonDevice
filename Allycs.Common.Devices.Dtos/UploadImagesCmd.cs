namespace Allycs.Common.Devices.Dtos
{
    using Nancy;
    using System.Collections.Generic;

    public class UploadImagesCmd
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public UploadImageType Type { get; set; }
        public UploadImageStatus Status { get; set; }
        public string RefereeId { get; set; }
        public List<HttpFile> Images { get; set; }
    }
}