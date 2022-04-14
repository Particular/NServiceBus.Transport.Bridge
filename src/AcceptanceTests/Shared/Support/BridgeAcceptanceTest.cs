﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.AcceptanceTesting;
using NServiceBus.Transport;
using NUnit.Framework;

public class BridgeAcceptanceTest
{
    [SetUp]
    public void SetUp()
    {
        NServiceBus.AcceptanceTesting.Customization.Conventions.EndpointNamingConvention = t =>
        {
            if (string.IsNullOrWhiteSpace(t.FullName))
            {
                throw new InvalidOperationException($"The type {nameof(t)} has no fullname to work with.");
            }

            var classAndEndpoint = t.FullName.Split('.').Last();

            var testName = classAndEndpoint.Split('+').First();

            var endpointBuilder = classAndEndpoint.Split('+').Last();

            testName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(testName);

            testName = testName.Replace("_", "");

            return testName + "." + endpointBuilder;
        };

        var transportConfig = TestSuiteConfiguration.Current.CreateTransportConfiguration();
        bridgeTransportDefinition = transportConfig.GetBridgeTransport();
    }

    [TearDown]
    public Task TearDown()
    {
        return bridgeTransportDefinition.Cleanup(CancellationToken.None);
    }

    protected void AddTestEndpoint<T>(BridgeTransportConfiguration bridgeTransportConfiguration)
        where T : EndpointConfigurationBuilder
    {
        var endpointName = NServiceBus.AcceptanceTesting.Customization.Conventions.EndpointNamingConvention(typeof(T));
        var endpointAddress = bridgeTransportDefinition.GetEndpointAddress(endpointName);
        bridgeTransportConfiguration.HasEndpoint(endpointName, endpointAddress);
    }

    protected TransportDefinition TransportBeingTested => bridgeTransportDefinition.TransportDefinition;
    protected TransportDefinition TestTransport;

    BridgeTransportDefinition bridgeTransportDefinition;
}