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
	using System.Xml;
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;

	#endregion

	public static class Measurements
	{
		private static readonly AbstractAttributeDefinitionDto ValueAttributeDefinition = new AttributeDefinitionDto( WellKnownKeys.Value.MeasuredValue, "Value", AttributeTypeDto.Float, 0 );
		private static readonly AbstractAttributeDefinitionDto MeasurementAttributeDefinition = new AttributeDefinitionDto( WellKnownKeys.Measurement.Time, "Time", AttributeTypeDto.DateTime, 0 );

		private static readonly InspectionPlanPartDto Part = new InspectionPlanPartDto { Path = PathHelper.RoundtripString2PathInformation( "P:/Measured part/" ), Uuid = Guid.NewGuid() };
		private static readonly InspectionPlanCharacteristicDto Characteristic = new InspectionPlanCharacteristicDto { Path = PathHelper.RoundtripString2PathInformation( "PC:/Measured part/Measured characteristic/" ), Uuid = Guid.NewGuid() };

		private static readonly DataCharacteristicDto Value = new DataCharacteristicDto
		{
			//Always specify both, path and uuid of the characteristic. All other properties are obsolete
			Path = Characteristic.Path,
			Uuid = Characteristic.Uuid,
			Value = new DataValueDto( 0.0 )
		};

		private static readonly DataMeasurementDto Measurement = new DataMeasurementDto
		{
			Uuid = Guid.NewGuid(),
			PartUuid = Part.Uuid,
			Attributes = new[]
				{
					new AttributeDto( MeasurementAttributeDefinition.Key, DateTime.Now )
				},
			Characteristics = new[]
				{
					Value
				}
		};

		#region methods

		public static async Task Lesson( DataServiceRestClient client )
		{
			//Create the most commonly used attributes for measurement and measurement value

			var configuration = await client.GetConfiguration();

			if( configuration.GetDefinition( EntityDto.Measurement, MeasurementAttributeDefinition.Key ) == null )
				await client.CreateAttributeDefinition( EntityDto.Measurement, MeasurementAttributeDefinition );

			if( configuration.GetDefinition( EntityDto.Value, ValueAttributeDefinition.Key ) == null )
				await client.CreateAttributeDefinition( EntityDto.Value, ValueAttributeDefinition );

			//Every measurement is attached to a part, and every value is attached to a characteristic
			await client.CreateParts( new[] { Part } );
			await client.CreateCharacteristics( new[] { Characteristic } );

			//The function 'CreateMeasurements' will create measurements without any values. You'll much more likely use the 'CreateMeasurementValues' function
			await client.CreateMeasurementValues( new[] { Measurement } );

			Value.Value = new DataValueDto( 0.0 ) //This will result in an unmeasured characteristic, because the attribute array doesn't contain K1
			{
				Attributes = new AttributeDto[] { }
			};

			await client.UpdateMeasurementValues( new[] { Measurement } );

			Value.Value = new DataValueDto //this will work!
			{
				Attributes = new[] { new AttributeDto( ValueAttributeDefinition.Key, 0.5 ) }
			};

			await client.UpdateMeasurementValues( new[] { Measurement } );

			var result = await client.GetMeasurementValues(
				PathInformationDto.Root,                                           // Part where to search measurements 
				new MeasurementValueFilterAttributesDto
				{
					AggregationMeasurements = AggregationMeasurementSelectionDto.All, // Decide how to include aggregated measurements in your query
					CharacteristicsUuidList = null,                                // Use characteristic uuids to fetch single measurement values
					Deep = true,                                                   // A deep search will find measurements recursively below the start path
					FromModificationDate = null,                                   // Will only search measurements with a modification date (LastModified) newer than the specified one
					ToModificationDate = null,                                     // Will only search measurements with a modification date (LastModified) older than the specified one
					LimitResult = 10,                                              // Will limit the number of returned measurements
					MeasurementUuids = null,                                       // Use measurement uuids to search for specific measurements
					OrderBy = new[]                                                // Order the returned measurements by specific attributes
					{
						new OrderDto( WellKnownKeys.Measurement.Time, OrderDirectionDto.Asc, EntityDto.Measurement )
					},
					RequestedMeasurementAttributes = null,                         // Specify, which measurement attributes should be returned (default: all)
					RequestedValueAttributes = null,                               // Specify, which value attributes should be returned (default: all)
					SearchCondition = new GenericSearchAttributeConditionDto       // You can create more complex attribute conditions using the GenericSearchAnd, GenericSearchOr and GenericSearchNot class
					{
						Attribute = WellKnownKeys.Measurement.Time,                //Only measurement attributes are supported
						Operation = OperationDto.GreaterThan,
						Value = XmlConvert.ToString( DateTime.UtcNow - TimeSpan.FromDays( 2 ), XmlDateTimeSerializationMode.Utc )
					}
				} );

			foreach( var measurement in result )
				Console.WriteLine( measurement.ToString() );
		}

		public static async Task UndoLesson( DataServiceRestClient client )
		{
			await client.DeleteMeasurementsByUuid( new[] { Measurement.Uuid } );
			//The server can be configured to allow or disallow deleting of parts with measurements

			await client.DeleteCharacteristics( new[] { Characteristic.Uuid } );
			await client.DeleteParts( new[] { Part.Uuid } );

			await client.DeleteAttributeDefinitions( EntityDto.Measurement, new[] { MeasurementAttributeDefinition.Key } );
			await client.DeleteAttributeDefinitions( EntityDto.Value, new[] { ValueAttributeDefinition.Key } );

		}

		#endregion
	}
}