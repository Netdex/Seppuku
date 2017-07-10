using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Seppuku.Module.Internal.ModuleWebAPI
{
    public class AuthenticationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.EnableBasicAuthentication(new BasicAuthenticationConfiguration(
                container.Resolve<IUserValidator>(),
                "seppuku-realm"));
        }
    }
}
