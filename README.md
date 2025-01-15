# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

# Register of Apprenticeship Training Providers (RoATP) Course Management API

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2FApprenticeships%20Providers%2Fdas-roatp-api?repoName=SkillsFundingAgency%2Fdas-roatp-api&branchName=refs%2Fpull%2F167%2Fmerge)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2798&repoName=SkillsFundingAgency%2Fdas-roatp-api&branchName=refs%2Fpull%2F167%2Fmerge)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-api&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-roatp-api)

## About
The API encapsulates ROATP course management data which is mainly for managing the courses delivery details for all the providers. This solution has two main projects: 

### API
There are two sets of endpoints: 
* Management: These endpoints are used by the course management web solution (https://github.com/SkillsFundingAgency/das-roatp-coursemanagement-web) which communicates with it via the outer api layer (https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/RoatpCourseManagement). 
* Integration: These endpoint are used by other services for integration purpose

### Jobs
The SFA.DAS.Roatp.Jobs project that contains functions to reload standards, reload provider details, update provider address and coordinates and import provider achievement rates. 

## Developer Setup

### Pre-requisites

You will need following on your local:
* .Net 8.0 SDK
* Azurite or similar local storage emulator
* SQL Database
* Visual studio or similar IDE 
* A clone of this repository

### Dependencies
* Setup `RoatpCourseManagement` outer api solution in `das-apim-endpoints`. This has its own dependencies which will be required to be setup as well, see its [readme](https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/RoatpCourseManagement) for further instructions.

### Configuration

- Create a `Configuration `table in your (Development) local storage account.
- Obtain the [SFA.DAS.Roatp.Api.json](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-api/SFA.DAS.Roatp.Api.json) from the das-employer-config and adjust the SqlConnectionString property to match your local setup.
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.Roatp.Api_1.0
  - Data: {The contents of the `SFA.DAS.Roatp.Api.json` file}
- Obtain the [SFA.DAS.Roatp.Jobs.json](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-api/SFA.DAS.Roatp.Jobs.json) from the das-employer-config for roatp-api repo 
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.Roatp.Jobs_1.0
  - Data: {The contents of the `SFA.DAS.Roatp.Jobs.json` file}
  - 
- In the `SFA.DAS.Roatp.Api` project, add `appSettings.Development.json` file with following content:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.Roatp.Api",
  "Environment": "LOCAL",
  "Version": "1.0",
  "APPINSIGHTS_INSTRUMENTATIONKEY": ""
}
```
- In the `SFA.DAS.Roatp.Jobs` project, add `local.settings.json` file with following content:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true;",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
    "EnvironmentName": "LOCAL",
    "LoggingRedisConnectionString": "localhost",
    "QarTimePeriod": "202223",
    "QarOverallImportFileName": "app-narts-subject-and-level-detailed.csv",
    "QarProviderLevelImportFileName": "app-narts-provider-level-fwk-std.csv",
    "ReloadStandardsCacheSchedule": "0 0 20 * * 1-5",
    "ReloadProviderRegistrationDetailsSchedule": "0 0 21 * * 1-5",
    "UpdateUkrlpDataSchedule": "0 0 23 * * 1-5",
    "UpdateProviderAddressCoordinatesSchedule": "0 30 23 * * 1-5",
    "AzureWebJobs.LoadAllProviderAddressesFunction.Disabled": true,
    "AzureWebJobs.LoadCourseDirectoryDataFunction.Disabled": true,
    "AzureWebJobs.LoadProvidersAddressFunction.Disabled": true,
    "AzureWebJobs.ReloadNationalAcheivementRatesFunction.Disabled": true,
    "AzureWebJobs.ReloadProviderRegistrationDetailsFunction.Disabled": true,
    "AzureWebJobs.ReloadStandardsCacheFunction.Disabled": false,
    "AzureWebJobs.UpdateProviderAddressCoordinatesFunction.Disabled": true,
    "AzureWebJobs.ImportAchievementRatesFunction.Disabled": false
  }
}
```

### Initializing the data
- Publish the database project SFA.DAS.RoATP.Database to your local SQL Server instance.
- Seed following tables via manual scripts or by running respective jobs 
1) __Standard__: run job `ReloadStandardsCacheFunction`
2) __ProviderRegistrationDetail__: run jobs in this order `ReloadProviderRegistrationDetailsFunction`, `LoadAllProviderAddressesFunction`
