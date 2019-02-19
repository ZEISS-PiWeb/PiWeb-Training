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
	using System.Security.Cryptography;
	using System.Text;
	using System.Threading.Tasks;
	using Zeiss.IMT.PiWeb.Api.Common.Data;
	using Zeiss.IMT.PiWeb.Api.DataService.Rest;
	using Zeiss.IMT.PiWeb.Api.RawDataService.Rest;

	#endregion

	public static class RawData
	{
		private static readonly InspectionPlanPart Part = new InspectionPlanPart { Path = PathHelper.String2PartPathInformation( "/RawDataTarget" ), Uuid = Guid.NewGuid() };

		#region methods

		public static async Task Lesson( DataServiceRestClient client, RawDataServiceRestClient rawClient )
		{
			await client.CreateParts( new[] { Part } );

			//PiWeb only accepts binary data
			var data = Encoding.UTF8.GetBytes( "Hello RawDataService!" );
			var target = RawDataTargetEntity.CreateForPart( Part.Uuid );

			//Notes:	- see e.g. http://wiki.selfhtml.org/wiki/Referenz:MIME-Typen for a complete list of mime types
			//			- When using Key = -1, the server will generate a new key

			await rawClient.CreateRawData( new RawDataInformation
			{
				FileName = "Hello.txt",
				MimeType = "text/plain",
				Key = -1,
				Created = DateTime.Now,
				LastModified = DateTime.Now,
				MD5 = new Guid( MD5.Create().ComputeHash( data ) ),
				Size = data.Length,
				Target = target
			}, data );

			var rawDataInformation = await rawClient.ListRawData( new[] { target } );

			foreach( var information in rawDataInformation )
			{
				Console.WriteLine( $"Fetchin {information.FileName}: {information.Size} bytes" );

				//Fetch the data by providing the correct RawDataInformation
				data = await rawClient.GetRawData( information );

				Console.WriteLine( $"Content: {Encoding.UTF8.GetString( data )}" );

				//We can use the key we found with the ListRawData function to delete a single file
				await rawClient.DeleteRawDataForPart( Part.Uuid, information.Key );
			}
			
			//Or we simply delete all raw data for a certain entity
			await rawClient.DeleteRawDataForPart( Part.Uuid );
		}

		public static async Task UndoLesson( DataServiceRestClient client, RawDataServiceRestClient rawClient )
		{
			await client.DeleteParts( new[] { Part.Uuid } );
		}
		#endregion
	}


}