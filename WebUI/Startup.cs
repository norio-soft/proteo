using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Orchestrator.WebUI.Startup))]

namespace Orchestrator.WebUI
{

    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            this.Configuration(app, GlobalConfiguration.Configuration.DependencyResolver);
        }

        public void Configuration(IAppBuilder app, IDependencyResolver dependencyResolver)
        {
            ConfigureAuth(app, dependencyResolver);
        }

    }

}