namespace Allycs.Common.Devices.Entities
{
    using System;

    public class UploadImages
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public UploadImageType Type { get; set; }
        public UploadImageStatus Status { get; set; }
        public string RefereeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}