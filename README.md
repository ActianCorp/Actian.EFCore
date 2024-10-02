# Actian.EFCore

[Entity Framework Core](https://github.com/dotnet/efcore) provider for Actian X, Ingres, and Vector.

   * [Overview](#overview)
   * [Installing Actian.EFCore](#installing-actianefcore)
      + [Before installing](#before-installing)
      + [Install using the .NET Core CLI](#install-using-the-net-core-cli)
      + [Install using the NuGet Package Manager Console in Visual Studio](#install-using-the-nuget-package-manager-console-in-visual-studio)
   * [Testing](#testing)
      + [Configuration for tests](#configuration-for-tests)
      + [Preparing to test](#preparing-to-test)
      + [Running tests](#running-tests)
   * [History](#history)


## Overview

Actian.EFCore is a database provider for Entity Framework Core that allows access to Actian Ingres and Vector from EFCore. Further references:

  * https://en.wikipedia.org/wiki/Entity_Framework
  * https://learn.microsoft.com/en-us/ef/core/
  * https://github.com/dotnet/efcore

## Obtaining Actian.EFCore

Source code and issue tracker is located at https://github.com/ActianCorp/Actian.EFCore/

Pre-built binaries are available from https://esd.actian.com/

## Support

Issues may be opened at [ActianCorp GitHub Project Page for EFCore](https://github.com/ActianCorp/Actian.EFCore/issues).
Alternatively contact support.

## Installing Actian.EFCore

### Before installing

Actian.EFCore 8.0 relies on Actian.Client (Actian .NET Data Provider) version 8.0.0 and above to work. Actian.Client has not been published to [NuGet.org], so it will have to be added to a local or private NuGet feed, which the project has access to.

Actian.Client (Actian .NET Data Provider) can be downloaded from [Customer Downloads Provider].

For help setting up a local NuGet feed see: [Setting up Local NuGet Feeds | Microsoft Docs].

### Install using the .NET Core CLI

```
dotnet add package Actian.EFCore
```

### Install using the NuGet Package Manager Console in Visual Studio

```powershell
Install-Package Actian.EFCore
```

## Testing

The Actian.EFCore solution contains a number of automated tests.

### Configuration for tests

When running tests the database server to be used is specified by the environment variable `ACTIAN_TEST_CONNECTION_STRING`. This variable should contain an Actian client connection string specifying:

- The actian server
- The port
- The dabase user id
- The password for the dabase user id
- Persist Security Info=true

Example:
```
Server=actian-client-test;Port=II7;User ID=efcore_test;Password=xxxxxxxxxx;Persist Security Info=true
```

The connection string should _not_ specify the database.

The database user specified in the connection string should:
- have permission to create new database users
- have permission to impersonate other database users

### Preparing to test

A number of databases, owned by the `"dbo"` user, need to be created before running tests.

These databases can be created on the machine that hosts the database server by running the following script:

```
scripts\setup-test-databases.cmd
```

The environment variable `ACTIAN_TEST_CONNECTION_STRING` must be have a valid value for this to work.

The user that runs this script should:

- have permission to create new database users
- have permission to create new databases
- have permission to impersonate other database users

When running `scripts\setup-test-databases.cmd` the following users will be created:
- `"dbo"`
- `"db2"`
- `"db.2"`

### Running tests

These tests can be run in Visual Studio or from the command line:

```
dotnet test
```

The environment variable `ACTIAN_TEST_CONNECTION_STRING` must be have a valid value for this to work.

## History

 * EFCore 8 - for Ingres and Vector - Authored by https://github.com/mianculovici
 * EFCore 3 - for Actian X, Ingres and Vector. Based on https://github.com/2PS-Consulting/Actian.EFCore Authored by https://github.com/MortenHoustonLudvigsen and https://github.com/mianculovici
 * https://github.com/ActianCorp/EntityFramework6.Ingres/ Authored by https://github.com/thoda04

[Customer Downloads Provider]: https://esd.actian.com/product/drivers/.Net_Data_Provider/Windows_64-Bit/.Net_Data_Provider_GA
[Setting up Local NuGet Feeds | Microsoft Docs]: https://docs.microsoft.com/en-us/nuget/hosting-packages/local-feeds
[NuGet.org]: https://www.nuget.org/
