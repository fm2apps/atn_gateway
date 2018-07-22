using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ATNTest.Startup))]
namespace ATNTest
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
