using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace Orchestrator.WebUI
{

    /// <summary>
    /// Configure DI container for constructor dependency injection for Web API calls within WebUI.
    /// </summary>
    /// <remarks>Note that web services written from here on in should be part of the separate Orchestrator.WebUI project, not part of WebUI, so this may be redundant.</remarks>
    public static class DIConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // Repositories container is loaded by default but we have to add the Token container config
            DIContainer.AddConfiguration("Token");

            // Add Unity DependencyResolver
            config.DependencyResolver = new UnityDependencyResolver();
        }

    }

    public class UnityDependencyResolver : IDependencyResolver
    {

        public object GetService(Type serviceType)
        {
            try
            {
                return DIContainer.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return DIContainer.ResolveAll(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        { }

    }

}