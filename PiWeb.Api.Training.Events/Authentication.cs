#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2024                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections.Client;

namespace PiWeb.Api.Training.Events;

public static class Authentication
{
    internal static async Task<AuthenticationMode> GetAuthenticationMode(Uri serverUrl)
    {
        var modes = await GetAuthenticationModes(serverUrl);

        // Display found authentication modes to the user and offer a selection
        return modes.Count > 0
            ? ConsoleUI.SelectOption("Enabled authentication modes", modes)
            : AuthenticationMode.None;
    }
    
    internal static void SetConnectionOptions(AuthenticationMode mode, HttpConnectionOptions options)
    {
        switch (mode)
        {
            case AuthenticationMode.None:
                // nothing to do
                break;
            case AuthenticationMode.Basic:
                SetConnectionOptionsForBasic(options);
                break;
            case AuthenticationMode.Windows:
                SetConnectionOptionsForWindows(options);
                break;
            case AuthenticationMode.Certificate:
                SetConnectionOptionsForCertificate(options);
                break;
            case AuthenticationMode.OpenIDConnect:
                throw new ArgumentOutOfRangeException(nameof(mode),
                    "Authentication mode is not supported by this client implementation.");
            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
        }
    }

    private static void SetConnectionOptionsForBasic(HttpConnectionOptions options)
    {
        var userName = ConsoleUI.InputText("User name");
        var password = ConsoleUI.InputText("Password");

        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
        options.Headers.Add("Authorization", $"Basic {credentials}");
    }

    private static void SetConnectionOptionsForWindows(HttpConnectionOptions options)
    {
        options.UseDefaultCredentials = true;
    }

    private static void SetConnectionOptionsForCertificate(HttpConnectionOptions options)
    {
        using var certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        certificateStore.Open(OpenFlags.ReadOnly);

        var certificates = certificateStore.Certificates
            .Find(X509FindType.FindByApplicationPolicy, "1.3.6.1.5.5.7.3.2", true).ToList();

        if (certificates.Count == 0)
            throw new InvalidOperationException("No suitable certificate found.");

        var selectedCertificate = ConsoleUI.SelectOption("Found client certificates", certificates,
            certificate =>
                $"Subject: {certificate.Subject}; Issuer: {certificate.Issuer}; Thumbprint: {certificate.Thumbprint}");

        options.ClientCertificates!.Add(selectedCertificate);
    }

    private static async Task<IList<AuthenticationMode>> GetAuthenticationModes(Uri serverUrl)
    {
        try
        {
            using var httpClient = new HttpClient();
            var jsonResult = await httpClient.GetStringAsync(new Uri(serverUrl, ".well-known/ServerConfiguration"));

            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            var serverConfiguration =
                JsonSerializer.Deserialize<ServerConfiguration>(jsonResult, jsonSerializerOptions);

            if (serverConfiguration != null)
                return serverConfiguration.AuthenticationModes;

            ConsoleUI.WriteWarning("Supported authentication modes could not be determined.");
            return [];
        }
        catch (Exception exception)
        {
            ConsoleUI.WriteWarning(exception.Message);
            return [];
        }
    }
    
    /// <summary>
    /// Available authentication modes
    /// </summary>
    internal enum AuthenticationMode
    {
        None,
        Basic,
        Windows,
        Certificate,
        OpenIDConnect
    }

    /// <summary>
    /// Configuration of a PiWeb Server
    /// </summary>
    private record ServerConfiguration
    {
        /// <summary>
        /// Gets the supported authentication modes.
        /// </summary>
        [JsonPropertyName("authentication_types_supported")]
        public AuthenticationMode[] AuthenticationModes { get; init; } = [];
    }
}