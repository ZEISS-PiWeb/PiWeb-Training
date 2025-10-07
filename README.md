# PiWeb-Training

Samples and lessons for using the PiWeb API

## PiWeb.API - Training

A training application including the most common use cases to read and write data with our .NET SDK from and to the database.

All aspects of our .NET SDK [Zeiss.PiWeb.Api.Rest](https://www.nuget.org/packages/Zeiss.PiWeb.Api.Rest/) with examples structured in different lessons.
A comprehensive documentation can be found on [here](http://zeiss-piweb.github.io/PiWeb-Api/sdk/v7.2/).

#### Lessons:
- ServiceInformation: Get information about your PiWeb Server
- Configuration: Configure your PiWeb Server (attributes etc.)
- Catalogs: Create, update or delete catalogs
- InspectionPlan: Create, update or delete parts and characteristics
- Measurements:  Create, update or delete measurements, values and measurement attributes
- RawData: Create, update or delete raw data

## PiWeb.API - OIDC Authentication

### Client Configuration
If PiWeb Server is secured by OpenID Connect authentication you have to obtain an access token and pass it in the HTTP authorization header. The access token can be obtained from an OpenID identity provider. For doing this manually, 
please refer to the [security section](https://zeiss-piweb.github.io/PiWeb-Api/general#gi-security) of the API documentation to find out PiWeb specific settings and a short summary of necessary steps, and the official [OpenID Connect specification](https://openid.net/specs/openid-connect-core-1_0.html) for detailed information.

### Training application 

A training application showing how to authenticate a request when PiWeb Server is using OpenID Connect authentication.
The procedure for using OpenID Connect authentication is the same for PiWeb Server and PiWeb Cloud. PiWeb Cloud is accepting OpenID Connect as the only way of authentication. 

The training application is utilizing OpenID Connect helper features of our [.NET API SDK](https://www.nuget.org/packages/Zeiss.PiWeb.Api.Rest/) to deal with PiWeb specifics, as well as the authentication process. The application also uses [WebView2](https://www.nuget.org/packages/Microsoft.Web.WebView2/) as embedded browser. 
Please note, in order to make it work you might need to install the
[WebView2 Runtime](https://docs.microsoft.com/de-de/microsoft-edge/webview2/concepts/distribution).

## PiWeb.API - Events

A training application showing how to subscribe to PiWeb Server events sent for changes done via [DataServiceRest](https://zeiss-piweb.github.io/PiWeb-Api/dataservice/v1.11/#ds-events) or [RawDataServiceRest](https://zeiss-piweb.github.io/PiWeb-Api/rawdataservice/v1.8/#rs-events), using the [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-8.0) C# client.

The training application will guide you through the process of subscribing to different events and allows you to get a more in-depth look on how it works using C#.
The application is a simple console application, writing out received events. It supports most of the authentication methods of PiWeb Server.

### Getting started

- open the training project in the IDE of your choice
- build and run the project PiWeb.Api.Training.Events
- enter the URL of your PiWeb Server, including port and if applicable the instance name
- choose the authentication method you want to use and provide the login data

After a successful connection is established, the console will print information about received events.

Try it out! Create a part using PiWeb Planner or the API, and the training application will inform you about this.