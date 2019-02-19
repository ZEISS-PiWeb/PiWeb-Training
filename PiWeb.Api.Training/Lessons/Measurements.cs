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
	using Zeiss.IMT.PiWeb.Api.Common.Data;
	using Zeiss.IMT.PiWeb.Api.DataService.Rest;

	#endregion

	public static class Measurements
	{
		private static readonly AbstractAttributeDefinition ValueAttributeDefinition = new AttributeDefinition( WellKnownKeys.Value.MeasuredValue, "Value", AttributeType.Float, 0 );
		private static readonly AbstractAttributeDefinition MeasurementAttributeDefinition = new AttributeDefinition( WellKnownKeys.Measurement.Time, "Time", AttributeType.DateTime, 0 );

		private static readonly InspectionPlanPart Part = new InspectionPlanPart { Path = PathHelper.RoundtripString2PathInformation( "P:/Measured part/" ), Uuid = Guid.NewGuid() };
		private static readonly InspectionPlanCharacteristic Characteristic = new InspectionPlanCharacteristic { Path = PathHelper.RoundtripString2PathInformation( "PC:/Measured part/Measured characteristic/" ), Uuid = Guid.NewGuid() };

		private static readonly DataCharacteristic Value = new DataCharacteristic
		{
			//Always specify both, path and uuid of the characteristic. All other properties are obsolete
			Path = Characteristic.Path,
			Uuid = Characteristic.Uuid,
			Value = new DataValue( 0.0 )
		};

		private static readonly DataMeasurement Measurement = new DataMeasurement
		{
			Uuid = Guid.NewGuid(),
			PartUuid = Part.Uuid,
			Attributes = new[]
				{
					new Zeiss.IMT.PiWeb.Api.DataService.Rest.Attribute( MeasurementAttributeDefinition.Key, DateTime.Now )
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

			if( configuration.GetDefinition( Entity.Measurement, MeasurementAttributeDefinition.Key ) == null )
				await client.CreateAttributeDefinition( Entity.Measurement, MeasurementAttributeDefinition );

			if( configuration.GetDefinition( Entity.Value, ValueAttributeDefinition.Key ) == null )
				await client.CreateAttributeDefinition( Entity.Value, ValueAttributeDefinition );

			//Every measurement is attached to a part, and every value is attached to a characteristic
			await client.CreateParts( new[] { Part } );
			await client.CreateCharacteristics( new[] { Characteristic } );

			//The function 'CreateMeasurements' will create measurements without any values. You'll much more likely use the 'CreateMeasurementValues' function
			await client.CreateMeasurementValues( new[] { Measurement } );

			Value.Value = new DataValue( 0.0 ) //This will result in an unmeasured characteristic, because the attribute array doesn't contain K1
			{
				Attributes = new Zeiss.IMT.PiWeb.Api.DataService.Rest.Attribute[] { }
			};

			await client.UpdateMeasurementValues( new[] { Measurement } );

			Value.Value = new DataValue //this will work!
			{
				Attributes = new[] { new Zeiss.IMT.PiWeb.Api.DataService.Rest.Attribute( ValueAttributeDefinition.Key, 0.5 ) }
			};

			await client.UpdateMeasurementValues( new[] { Measurement } );

			var result = await client.GetMeasurementValues(
				PathInformation.Root,                                              // Part where to search measurements 
				new MeasurementValueFilterAttributes
				{
					AggregationMeasurements = AggregationMeasurementSelection.All, // Decide how to include aggregated measurements in your query
					CharacteristicsUuidList = null,                                // Use characteristic uuids to fetch single measurement values
					Deep = true,                                                   // A deep search will find measurements recursively below the start path
					FromModificationDate = null,                                   // Will only search measurements with a modification date (LastModified) newer than the specified one
					ToModificationDate = null,                                     // Will only search measurements with a modification date (LastModified) older than the specified one
					LimitResult = 10,                                              // Will limit the number of returned measurements
					MeasurementUuids = null,                                       // Use measurement uuids to search for specific measurements
					OrderBy = new[]                                                // Order the returned measurements by specific attributes
					{
						new Order( WellKnownKeys.Measurement.Time, OrderDirection.Asc, Entity.Measurement )
					},
					RequestedMeasurementAttributes = null,                         // Specify, which measurement attributes should be returned (default: all)
					RequestedValueAttributes = null,                               // Specify, which value attributes should be returned (default: all)
					SearchCondition = new GenericSearchAttributeCondition          // You can create more complex attribute conditions using the GenericSearchAnd, GenericSearchOr and GenericSearchNot class
					{
						Attribute = WellKnownKeys.Measurement.Time,                //Only measurement attributes are supported
						Operation = Operation.GreaterThan,
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

			await client.DeleteAttributeDefinitions( Entity.Measurement, new[] { MeasurementAttributeDefinition.Key } );
			await client.DeleteAttributeDefinitions( Entity.Value, new[] { ValueAttributeDefinition.Key } );

		}

		#endregion
	}
}