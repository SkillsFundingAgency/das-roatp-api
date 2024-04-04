using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System.IO;

namespace SFA.DAS.Roatp.Jobs.Extensions;
public static class AddConfigurationExtension
{
    public static void AddConfiguration(this IConfigurationBuilder builder)
    {
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true);

        var configuration = builder.Build();

        builder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = new[] { "SFA.DAS.Roatp.Jobs", "SFA.DAS.Roatp.CourseManagement.Web" };
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            })
            .Build();
    }
}
