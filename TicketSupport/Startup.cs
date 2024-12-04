using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TicketSupport.Startup))]
namespace TicketSupport
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
