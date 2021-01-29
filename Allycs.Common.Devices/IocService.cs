using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices
{
    using Microsoft.Extensions.DependencyInjection;

    public class IocService
    {
        public string ServiceType { get; set; }

        public string ImplementationType { get; set; }

        public ServiceLifetime Lifetime { get; set; }
    }
}
