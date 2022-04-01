﻿using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        var rc = new MessageRouterConfiguration();

        // TODO: Discuss if rc.AddTransport() would be better and align with https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageRouter.html
        // .AddMessagingSystem() would also work and align with https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageChannel.html

        // SQL -> SQL
        // ASB.Namespace -> ASB.Namespace
        // ASB.Topics -> ASB.Topics 

        rc.AddTransport(new MsmqTransport())
            .HasEndpoint("Sales") //.AtMachine("ServerA")
            .HasEndpoint("Finance") //.AtMachine("ServerB");
            .RegisterPublisher("MyNamespace.MyEvent", "Shipping");

        // Note to Kyle & Travis, the above code doesn't work yet. The `AtMachine` I just made up.
        // Would it be possible to only have AtMachine available when you're on MsqmTransport?

        rc.AddTransport(new LearningTransport())
            .HasEndpoint("Shipping")
            .HasEndpoint("Marketing");

        rc.AddTransport(new SqlServerTransport("Data Source=;Initial Catalog=;Integrated Security=True"))
            .HasEndpoint("Billing")
            .HasEndpoint("Support");

        var runningRouter = await rc.Start().ConfigureAwait(false);

        Console.ReadKey();

        await runningRouter.Stop().ConfigureAwait(false);
    }
}