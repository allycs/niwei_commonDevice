namespace Allycs.Common.Devices.Services
{
    using Dapper;

    public class SqlAndDps
    {
        public string Sql { get; set; }
        public DynamicParameters Dps { get; set; }
    }
}