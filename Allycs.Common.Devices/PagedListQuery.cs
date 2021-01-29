namespace Allycs.Common.Devices
{
    public class PagedListQuery
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 25;

        public string PostgresLimitPartialSql()
        {
            var sqlLimit = Limit;
            var sqlOffset = (Page - 1) * Limit;

            var sql = $" LIMIT {sqlLimit}";
            if (sqlOffset > 0)
            {
                sql += $" OFFSET {sqlOffset}";
            }
            return sql;
        }
    }
}