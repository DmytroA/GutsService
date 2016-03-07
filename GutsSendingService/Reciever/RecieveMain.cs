using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Guts.Core.Entities;
using Topshelf;
using Reciever;
using log4net.Config;

public class RecieveMain 
{
    public static void Main()
    {
        XmlConfigurator.Configure();

        HostFactory.Run(hostConfigurator =>
        {
            hostConfigurator.Service<RecieveQueueJson>(serviceConfigurator =>
            {
                serviceConfigurator.ConstructUsing(() => new RecieveQueueJson());
                serviceConfigurator.WhenStarted(recieveService => recieveService.Start());
                serviceConfigurator.WhenStopped(recieveService => recieveService.Stop());
            });

            hostConfigurator.RunAsLocalSystem();

            hostConfigurator.SetDescription("RecieveQueue using Topshelf");
            hostConfigurator.SetDisplayName("GutsRecievingQueue");
            hostConfigurator.SetServiceName("GutsRecievingQueue");
        });

    }

}