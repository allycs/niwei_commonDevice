using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Services
{
    using Microsoft.Extensions.Options;
    using Npgsql;

    public class PostgresService
    {
        private readonly AppSettings _settings;

        public PostgresService(IOptionsSnapshot<AppSettings> option)
        {
            _settings = option.Value;
        }

        internal NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_settings.ConnectionString);
        }
    }
}
