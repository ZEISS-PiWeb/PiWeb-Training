using Newtonsoft.Json;
using System;
using System.Windows;
using Zeiss.PiWeb.Api.Rest.Common.Client;
using Zeiss.PiWeb.Api.Rest.Common.Utilities;
using Zeiss.PiWeb.Api.Rest.HttpClient.Data;

namespace PiWeb.Api.Training.OidcAuth
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			// get the database id from the text box
			bool isDatabaseIdValid = Guid.TryParse(DatabaseId.Text, out var databaseGuid);
			if (!isDatabaseIdValid)
			{
				Output.Text = "invalid database id - please input the database id using the database url in PiWeb cloud portal";
				return;
			}

			// combine database id with cloud base URI to get the server URI
			var cloudBaseUri = new Uri("https://piwebcloud-service.metrology.zeiss.com/");
			var serverUri = new Uri(cloudBaseUri, databaseGuid.ToString("D"));

			// use OAuthHelper to get the OIDC access token for the service
			var oAuthTokenCredential = OAuthHelper.GetAuthenticationInformationForDatabaseUrl(serverUri.AbsoluteUri, requestCallback: OAuthRequestCallback);

			// create the REST client class and use the OIDC access token for authentication
			var dataServiceRestClient = new DataServiceRestClient(serverUri);
			dataServiceRestClient.AuthenticationContainer = new AuthenticationContainer(oAuthTokenCredential.AccessToken);

			// invoke the service method querying data from PiWeb cloud database
			var serviceInformation = await dataServiceRestClient.GetServiceInformation();

			// display queried data
			var serializedServiceInformation = JsonConvert.SerializeObject(serviceInformation, Formatting.Indented);
			Output.Text = serializedServiceInformation;
		}

		private OAuthResponse? OAuthRequestCallback(OAuthRequest request)
		{
			// the OAuthRequest contains the start URL and callback URL
			// display an embedded browser at the start URL
			var loginWindow = new LoginWindow()
			{
				DataContext = request
			};

			var dialogResult = loginWindow.ShowDialog();

			// if login was successful return the response
			if (dialogResult == true)
				return loginWindow.OAuthResponse;
			else
				return null;
		}
	}
}
