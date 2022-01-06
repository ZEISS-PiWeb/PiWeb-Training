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
	using Zeiss.PiWeb.Api.Rest.Dtos;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Api.Rest.Dtos.RawData;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.RawData;

	#endregion

	public static class RawData
	{
		#region members

		private static readonly InspectionPlanPartDto Part = new InspectionPlanPartDto { Path = PathHelper.RoundtripString2PathInformation( "P:/RawDataTarget/" ), Uuid = Guid.NewGuid() };

		#endregion

		#region methods

		public static async Task Lesson( DataServiceRestClient client, RawDataServiceRestClient rawClient )
		{
			await client.CreateParts( new[] { Part } );

			//PiWeb only accepts binary data
			var data = Encoding.UTF8.GetBytes( "Hello RawDataService!" );
			var target = RawDataTargetEntityDto.CreateForPart( Part.Uuid );

			//Notes:	- see e.g. http://wiki.selfhtml.org/wiki/Referenz:MIME-Typen for a complete list of mime types
			//			- When using Key = -1, the server will generate a new key. The generated key can be read from the result.

			var createdRawDataInfo = await rawClient.CreateRawData( new RawDataInformationDto
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
			
			Console.WriteLine( $"RawData created with key: {createdRawDataInfo.Key}" );

			//We can simply update raw data information like filename or MIME-type
			if ( createdRawDataInfo.Key.HasValue )
			{
				createdRawDataInfo.FileName = "HelloEdit.txt";
				
				Console.WriteLine( $"Renaming raw data file to {createdRawDataInfo.FileName}" );
				
				await rawClient.UpdateRawDataInformation( target, createdRawDataInfo.Key.Value, createdRawDataInfo );
			}

			var rawDataInformation = await rawClient.ListRawData( new[] { target } );

			foreach( var information in rawDataInformation )
			{
				Console.WriteLine( $"Fetching {information.FileName}: {information.Size} bytes" );

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