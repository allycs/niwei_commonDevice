using Microsoft.Extensions.Logging;
using Nancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Modules.WebModules
{
    public class RootViewModule: NancyModule
    {
        protected readonly ILogger<IndexViewModule> _logger;

        public RootViewModule(ILogger<IndexViewModule> logger)
        {
            _logger = logger;
            Get("/", _ => View["index"]);
        }
    }
}
