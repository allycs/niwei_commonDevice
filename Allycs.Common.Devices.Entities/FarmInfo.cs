namespace Allycs.Common.Devices.Entities
{
    using Dapper;
    using System;

    public class FarmInfo
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public FarmType Type { get; set; }
        public string PersonLiable { get; set; }
        public string MainImg { get; set; }
        public FarmStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}