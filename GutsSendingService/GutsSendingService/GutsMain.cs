using log4net.Config;
using RabbitMQ.Client;
using Sportradar.SDK.Common.Interfaces;
using Sportradar.SDK.Services.Sdk;
using Sportradar.SDK.Services.SdkConfiguration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Guts.SendingService
{
    public class GutsService
    {
        private static Sdk m_sdk = Sdk.Instance;
        public static void Main()
        {
            XmlConfigurator.Configure();
            m_sdk.Initialize();
            m_sdk.Start();

            var cfm = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sdk_section = (SdkConfigurationSection)cfm.GetSection("Sdk");

            HostFactory.Run(x =>
            {
                x.Service<LiveScoutModule>(s =>
                {
                    s.ConstructUsing(name => new LiveScoutModule(m_sdk.LiveScout, "LiveScout", sdk_section.LiveScout.Test));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop()); 
                });
                x.RunAsLocalSystem();

                x.SetDescription("Guts Topshelf Host");
                x.SetDisplayName("Guts");
                x.SetServiceName("Guts");
            });
            
        }
    }

}
