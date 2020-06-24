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
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;
	using AttributeList = System.Collections.Generic.Dictionary<string, object>;

	public static class ExampleExport
	{
		#region methods

		public static void Run()
		{
			//Define your available characteristics
			var characteristics = new List<Characteristic>
			{
				new Characteristic
				{
					Name = "Rz",
					Value = 0.16,
					Attributes = new AttributeList
					{
						{ "LowerTolerance", -0.2 },
						{ "UpperTolerance", 0.2 },
						{ "NominalValue", 0.0 }
					}
				},
				new Characteristic
				{
					Name = "Ra",
					Value = 0.21,
					Attributes = new AttributeList
					{
						{ "LowerTolerance", -0.2 },
						{ "UpperTolerance", 0.2 },
						{ "NominalValue", 0.0 }
					}
				},
				new Characteristic
				{
					Name = "Rx",
					Value = 0.18,
					Attributes = new AttributeList
					{
						{ "LowerTolerance", -0.2 },
						{ "UpperTolerance", 0.2 },
						{ "NominalValue", 0.0 }
					}
				}
			};

			//Define a mapping between your internal entites and PiWeb-attributes
			var mapping = new Dictionary<string, ushort>
			{
				{ "LowerTolerance", 2110 },
				{ "UpperTolerance", 2111 },
				{ "NominalValue", 2101 }
			};

			ExportToPiWeb( "http://127.0.0.1:8080/", "MyInspection", characteristics, mapping );
		}

		private static void ExportToPiWeb( string serverUri, string inspectionName, IEnumerable<Characteristic> data,
			Dictionary<string, ushort> attributeMapping )
		{
			//1. Create the client
			var client = new DataServiceRestClient( new Uri(serverUri) );

			//2. Check the server configuration
			foreach( var entry in attributeMapping )
			{
				Configuration.CheckAttribute( client, EntityDto.Characteristic, entry.Value, entry.Key, AttributeTypeDto.Float );
			}

			//2.1 Make sure the essential time and value attributes are present
			Configuration.CheckAttribute( client, EntityDto.Measurement, WellKnownKeys.Measurement.Time, "Time",
				AttributeTypeDto.DateTime );
			Configuration.CheckAttribute( client, EntityDto.Value, WellKnownKeys.Value.MeasuredValue, "Value", AttributeTypeDto.Float );

			//3. Check the inspection plan

			//3.1 Check the inspection plan part
			var part = InspectionPlan.GetOrCreatePart( client, inspectionName );
			var characteristicMapping = new Dictionary<Characteristic, InspectionPlanCharacteristicDto>();

			//3.2 Check the inspection plan characteristics
			foreach( var characteristic in data )
			{
				var inspectionPlanCharacteristic = InspectionPlan.GetOrCreateCharacteristic( client, part.Path.Name,
					characteristic.Name, attributeMapping, characteristic.Attributes );
				characteristicMapping.Add( characteristic, inspectionPlanCharacteristic );
			}

			//4. Create the measurement
			var dataCharacteristics = characteristicMapping.Select( pair => new DataCharacteristicDto
			{
				Uuid = pair.Value.Uuid,
				Path = pair.Value.Path,
				Value = new DataValueDto
				{
					Attributes = new[] { new AttributeDto( 1, pair.Key.Value ) }
				}
			} ).ToArray();

			var measurement = new DataMeasurementDto
			{
				Uuid = Guid.NewGuid(),
				PartUuid = part.Uuid,
				Time = DateTime.UtcNow,
				Attributes = new[] { new AttributeDto( WellKnownKeys.Measurement.Time, DateTime.UtcNow ) },
				Characteristics = dataCharacteristics
			};

			//4.1 Write the measurement to the database
			client.CreateMeasurementValues( new[] { measurement } ).Wait();


		}

		#endregion

		#region class Characteristic

		private class Characteristic
		{
			#region properties

			public string Name { get; set; }
			public double Value { get; set; }
			public AttributeList Attributes { get; set; }

			#endregion
		}

		#endregion
	}
}