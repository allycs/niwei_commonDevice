namespace Allycs.Common.Devices.Dtos
{
    using Nancy;
    using System;

    public class NewFarmInfoCmd
    {
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public FarmType? Type { get; set; }
        public string PersonLiable { get; set; }
        public HttpFile MainImg { get; set; }
        public FarmStatus Status { get; set; }
    }
}