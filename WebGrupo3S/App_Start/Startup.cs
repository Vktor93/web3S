using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebGrupo3S.Startup))]
namespace WebGrupo3S
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
