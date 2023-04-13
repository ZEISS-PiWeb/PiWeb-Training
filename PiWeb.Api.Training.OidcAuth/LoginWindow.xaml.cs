using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using Zeiss.PiWeb.Api.Rest.Common.Utilities;

namespace PiWeb.Api.Training.OidcAuth
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		private readonly string _ExtractInputsScriptTemplate;

		public LoginWindow()
		{
			InitializeComponent();

			var resourceName = typeof(LoginWindow).Namespace + ".Resources.extract_cloud_login_inputs.js";
			using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
				?? throw new NullReferenceException("missing script resource");
			using (var streamReader = new StreamReader(stream, Encoding.UTF8))
				_ExtractInputsScriptTemplate = streamReader.ReadToEnd();
		}

		public OAuthRequest? OAuthRequest { get; set; }
		public OAuthResponse? OAuthResponse { get; set; }

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			OAuthRequest = (OAuthRequest)e.NewValue;

			if (OAuthRequest != null)
				WebView.Source = new Uri(OAuthRequest.StartUrl);
		}

		private void WebViewOnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
		{
			if (OAuthRequest == null)
				return;

			if( OAuthResponse != null )
			{
				// close embedded browser successfully
				DialogResult = true;
				Close();
			}
			else if( e.WebErrorStatus != CoreWebView2WebErrorStatus.ConnectionAborted &&
			         e.WebErrorStatus != CoreWebView2WebErrorStatus.OperationCanceled &&
			         ( e.IsSuccess == false || e.HttpStatusCode >= 400 ) )
			{
				Console.Error.WriteLine( $"WebView2: Server responded with error status: {e.WebErrorStatus}, HTTP status code: {e.HttpStatusCode}. Check server configuration!" );
				// do something with the error
			}
		}
		
		private async void WebViewOnNavigationStarting( object sender, CoreWebView2NavigationStartingEventArgs e )
		{
			if( OAuthRequest == null )
				return;

			var navigationTargetUrl = e.Uri;
			if( string.IsNullOrEmpty( navigationTargetUrl ) )
				return;

			if( navigationTargetUrl.StartsWith( OAuthRequest.CallbackUrl, StringComparison.Ordinal ) )
				e.Cancel = true;

			// monitor the embedded browser until a form is displayed trying to post a response
			// to the callback url
			// extract all inputs from the form constructing a URL with all inputs as URL parameters
			var extractInputsScript = _ExtractInputsScriptTemplate.Replace("%%CALLBACKURL%%", OAuthRequest.CallbackUrl);
			var response = await WebView.CoreWebView2.ExecuteScriptAsync(extractInputsScript);
			if (response == "null")
				return;

			// create the OAuthResponse using the URL from above
			OAuthResponse = new OAuthResponse(response);
		}
	}
}
