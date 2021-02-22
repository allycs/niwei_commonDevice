using Nancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Dtos
{
    public class UpdateFarmInfoCmd
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public FarmType? Type { get; set; }
        public string PersonLiable { get; set; }
        public HttpFile MainImg { get; set; }
        public FarmStatus? Status { get; set; }
    }
}
