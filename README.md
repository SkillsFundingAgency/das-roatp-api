# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

# Register of Apprenticeship Training Providers (RoATP) Course Management API

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/SkillsFundingAgency_das-roatp-api?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=SkillsFundingAgency_das-roatp-api&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-api&metric=alert_status)](https://sonarcloud.io/project/overview?id=SkillsFundingAgency_das-roatp-api)

## About

das-roatp-api is the inner api for roatp course management data.  It contains CRUD calls to provider, provider course,  provider locations, various other details about providers and courses, as well as lookup tables for standards and subregions that map to course directory data.  It is used by the course management web solution (https://github.com/SkillsFundingAgency/das-roatp-coursemanagement-web) which communicates with it via the outer api layer (https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/RoatpCourseManagement)

In addition, the solution contains a SFA.DAS.Roatp.Jobs project that contains functions to reload standards, reload provider details, and load course directory data.

> **Note:**  
> The job to load course directory data also uses configuration values from an external library, das-roatp-coursemanagement-web (see https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-course-management-web/SFA.DAS.Roatp.CourseManagement.Web.json), to manage a small list of providers for beta testing. This will be removed once the project proceeds past beta testing


## How It Works

# API

Once the full installation is done (below), you can run the API by running the SFA.DAS.Roatp.Api project and you will see a web interface for all available endpoints (via swagger)


# Jobs

Once the full installation is done (below), you will need to run the SFA.DAS.Roatp.Jobs project

In addition, you will need the following repo set up and running for the jobs to run:
* das-apim-endpoints, CourseManagement solution (see https://github.com/SkillsFundingAgency/das-apim-endpoints/src/RoatpCourseManagement)


1) Reload Standards Cache will run when you first start the application - this will populate/refresh the 'Standard' table
2) Reload Provider Registration Details will run when you first start the application - this will populate/refresh the 'ProviderRegistrationDetail' table
3) Load Course Directory - this has to be run manually, using an API tool such as POSTMAN, using the following details: POST http://localhost:7071/api/load-course-directory This will load all pilot and beta providers only (expect around 14 or 28, depending on configuration used).  Alternatively you can load all data from course directory using http://localhost:7071/api/load-course-directory?betaAndPilotOnly=false, in which case you will get 1400+ providers loaded.  This populates several tables, including provider, provider Course, provider Location etc



### Developer Setup

#### Requirements
- Clone this repository
- Install [Visual Studio 2022](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator]
- Install [Azure Storage Explorer](http://storageexplorer.com/)
- Administrator Access

#### Setup

- Create a Configuration table in your (Development) local storage account.
- Obtain the local config json from the das-employer-config for roatp-api repo (https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-api/SFA.DAS.Roatp.Api.json) and adjust the SqlConnectionString property to match your local setup (eg Data Source=<local database name>;Initial Catalog=SFA.DAS.RoATP.Database;Integrated Security=True;MultipleActiveResultSets=True;")
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.Roatp.Api_1.0
  - Data: {The contents of the local config json file}
  
- Obtain the local config json from the das-employer-config for roatp-api repo (https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-api/SFA.DAS.Roatp.Jobs.json)
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.Roatp.Jobs_1.0
  - Data: {The contents of the local config json file}


##### Open the API project within the solution

- Open Visual studio as an administrator
- Open the solution
- Set SFA.DAS.RoATP.Api as the startup project
- Publish the database project SFA.DAS.RoATP.Database to your local SQL Server instance
- Running the solution will launch the API in your browser

##### Open the Jobs project within the solution

- Open Visual studio as an administrator
- Open the solution
- Set SFA.DAS.RoATP.Jobs as the startup project
- Publish the database project SFA.DAS.RoATP.Database to your local SQL Server instance
- Run an instance of course management outer api with full setup (see https://github.com/SkillsFundingAgency/das-apim-endpoints/ 'Local Running > Course Management')
- Running the solution will launch the reload standards and reload provider registration details, and will make available the load course directory endpoint (see above for details)
