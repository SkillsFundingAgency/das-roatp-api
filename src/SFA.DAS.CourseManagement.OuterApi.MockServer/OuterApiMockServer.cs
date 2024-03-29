﻿using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;

namespace SFA.DAS.CourseManagement.OuterApi.MockServer
{
    public static class OuterApiMockServer
    {
        public static void Run()
        {
            var settings = new WireMockServerSettings
            {
                Port = 5334,
                UseSSL = true,
                Logger = new WireMockConsoleLogger()
            };

            var server = StandAloneApp.Start(settings);

            server
                .Given(Request.Create().WithPath(u => u.Contains("registered-providers"))
                .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("registered-providers.json"));

            server
                .Given(Request.Create().WithPath(u => u.Contains("standards"))
                .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("standards-lookup.json"));
        }
    }
}
