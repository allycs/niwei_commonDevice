using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Dtos
{
    public class GetUploadImageListCmd
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Remark { get; set; }
        public UploadImageType? Type { get; set; }
        public UploadImageStatus Status { get; set; }
        public string RefereeId { get; set; }
        public string CreatedBy { get; set; }
    }
}
