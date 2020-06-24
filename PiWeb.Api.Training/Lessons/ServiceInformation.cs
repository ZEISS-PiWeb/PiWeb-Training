#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2017                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace PiWeb.Api.Training.Lessons
{
	#region usings

	using System;
	using System.Threading.Tasks;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;

	#endregion

	public static class ServiceInformation
	{
		#region methods

		public static async Task Lesson( DataServiceRestClient client )
		{
			//The statistics in serviceInformation are calculated periodically by the server, and are not guaranteed to be always exact.

			var info = await client.GetServiceInformation();
			Console.WriteLine( info.ServerName );
		}

		#endregion
	}
}