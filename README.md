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

### OIDC Client Configuration
PiWeb uses the Hybrid Flow with PKCE (proof key for code exchange).

We recommend using the following OIDC client configuration for authentication:

**Authorize request:**

| Property       | Value                                             |
|----------------|---------------------------------------------------|
| client_id      | f1ddf74a-7ed1-4963-ab60-a1138a089791              |
| response_type  | id_token code                                     |
| redirect_uri   | urn:ietf:wg:oauth:2.0:oob                         |
| response_mode  | form_post                                         |
| scope          | openid%20profile%20email%20offline_access%20piweb |

**Token exchange request:**

| Property     | Value                                              |
|--------------|----------------------------------------------------|
| client_id    | f1ddf74a-7ed1-4963-ab60-a1138a089791               |
| redirect_uri | urn:ietf:wg:oauth:2.0:oob                          |
| grant_type   | authorization_code                                 |

### Training application 

A training application showing how to authenticate a request when PiWeb server is using OpenID connect authentication.
PiWeb cloud is a PiWeb server using OpenID connect authentication. The procedure for using OpenID connect authentication
is the same for PiWeb server and PiWeb cloud.

The training application uses [WebView2](https://www.nuget.org/packages/Microsoft.Web.WebView2/) as embedded browser. 
Please note that in order for this to work you might need to install the
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