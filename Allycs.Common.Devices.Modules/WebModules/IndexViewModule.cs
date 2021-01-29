using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Modules.WebModules
{
    public class IndexViewModule: BaseNancyModule
    {
        protected readonly ILogger<IndexViewModule> _logger;

        public IndexViewModule(ILogger<IndexViewModule> logger) : base("web", "view", "")
        {
            _logger = logger;
            Get("/", _ => View["index"]);
        }
    }
}
