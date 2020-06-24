#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2017                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace PiWeb.Api.Training.Example
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Zeiss.PiWeb.Api.Rest.Dtos;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;

	public static class InspectionPlan
	{
		#region methods

		public static InspectionPlanPartDto GetOrCreatePart( DataServiceRestClient client, string name )
		{
			var parts = client.GetParts( PathInformationDto.Root, null, 1 ).Result;
			var existingPart = parts.FirstOrDefault( p => string.Equals( p.Path.Name, name ) );
			if( existingPart == null )
			{
				existingPart = new InspectionPlanPartDto
				{
					Path = PathHelper.String2PartPathInformation( name ),
					Uuid = Guid.NewGuid()
				};

				client.CreateParts( new[]
				{
					existingPart
				} ).Wait();
			}

			return existingPart;
		}

		public static InspectionPlanCharacteristicDto GetOrCreateCharacteristic( DataServiceRestClient client, string partName, string characteristicName, Dictionary<string, ushort> mapping, Dictionary<string, object> values )
		{
			var characteristics = client.GetCharacteristics( PathHelper.String2PartPathInformation( partName ), 1 ).Result;
			var attributes = values.Select( pair => new AttributeDto( mapping[ pair.Key ], pair.Value ) ).ToArray();

			var existingCharacteristic = characteristics.FirstOrDefault( p => string.Equals( p.Path.Name, characteristicName ) );
			if( existingCharacteristic == null )
			{
				existingCharacteristic = new InspectionPlanCharacteristicDto
				{
					Path = PathHelper.RoundtripString2PathInformation( "PC:/" + partName + " / " + characteristicName+ "/" ),
					Uuid = Guid.NewGuid(),
					Attributes = attributes
				};

				client.CreateCharacteristics( new[]
				{
					existingCharacteristic
				} ).Wait();
			}
			else
			{
				existingCharacteristic.Attributes = attributes;
				//maybe update the existing characteristic, so the attributes are up-to-date
				client.UpdateCharacteristics( new[]
				{
					existingCharacteristic
				} ).Wait();
			}

			return existingCharacteristic;
		}

		#endregion
	}
}