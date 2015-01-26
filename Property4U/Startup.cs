using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

namespace IdentitySample
{
    [assembly: OwinStartup(typeof(IdentitySample.Startup))]
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}
