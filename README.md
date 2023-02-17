# PiWeb-Training

Samples and lessons for our APIs

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

## PiWeb.API - OIDC Authentication Training

A training application showing how to authenticate a request when PiWeb server is using OpenID connect authentication.
PiWeb cloud is a PiWeb server using OpenID connect authentication. The procedure for using OpenID connect authentication
is the same for PiWeb server and PiWeb cloud.

The training application uses [WebView2](https://www.nuget.org/packages/Microsoft.Web.WebView2/) as embedded browser. 
Please note that in order for this to work you might need to install the
[WebView2 Runtime](https://docs.microsoft.com/de-de/microsoft-edge/webview2/concepts/distribution).
