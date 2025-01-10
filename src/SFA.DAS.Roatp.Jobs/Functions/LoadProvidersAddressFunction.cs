﻿using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class LoadProvidersAddressFunction
    {
        private readonly ILoadUkrlpAddressesService _loadUkrlpAddressesService;

        public LoadProvidersAddressFunction(ILoadUkrlpAddressesService loadUkrlpAddressesService)
        {
            _loadUkrlpAddressesService = loadUkrlpAddressesService;
        }

        [Function(nameof(LoadProvidersAddressFunction))]
        public async Task Run([TimerTrigger("%UpdateUkrlpDataSchedule%",RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("LoadProvidersAddressFunction function started");
            var result = await _loadUkrlpAddressesService.LoadProvidersAddresses();

            if (result)
                log.LogInformation("Ukrlp Addresses updated from last update date");
            else
                log.LogWarning("Ukrlp addresses not updated from last update date");
        }
    }
}
