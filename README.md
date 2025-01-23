# Introduction 
MicroServices - shared library with common functions like GenericRepository, Audit logger, Identity service, Base64 file helper, Authentication helpers, Notification and OTP and many more

# Getting Started
To deploy the nuget package, run this on CMD from the project folder

nuget.exe push -Source "Libraries" -ApiKey az <PROJECT_PATH>\MicroService.Shared\bin\Release\MicroService.Shared.0.0.1.nupkg

# To Include Package in Project
PM> Install-Package MicroService.Shared -version 1.0.0

Make sure you point the download source to https://nuget.pkg.github.com/First-Ally-Capital/index.json