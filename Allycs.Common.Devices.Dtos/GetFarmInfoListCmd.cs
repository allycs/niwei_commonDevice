namespace Allycs.Common.Devices.Dtos
{
    public class GetFarmInfoListCmd
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public FarmType? Type { get; set; }
    }
}