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
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;
	using Attribute = Zeiss.PiWeb.Api.Core.Attribute;

	#endregion

	public static class Configuration
	{
		#region members

		private static readonly AbstractAttributeDefinitionDto AttributeDefinition = new AttributeDefinitionDto( 11001, "Description", AttributeTypeDto.AlphaNumeric, 255 );
		private static readonly AbstractAttributeDefinitionDto CatalogColumnAttributeDefinition = new AttributeDefinitionDto( 14001, "Catalog column", AttributeTypeDto.AlphaNumeric, 255 );

		private static readonly CatalogDto Catalog = new CatalogDto
		{
			Name = "TestCatalog",
			Uuid = Guid.NewGuid(),
			ValidAttributes = new[] { CatalogColumnAttributeDefinition.Key },
			CatalogEntries = new[]
			{
				new CatalogEntryDto
				{
					Key = 1,
					Attributes = new[] { new Attribute( CatalogColumnAttributeDefinition.Key, "Value" ) }
				}
			}
		};

		private static readonly AbstractAttributeDefinitionDto CatalogAttributeDefinition = new CatalogAttributeDefinitionDto
		{
			Catalog = Catalog.Uuid,
			Description = "Catalog Attribute",
			Key = 11002
		};

		#endregion

		#region methods

		/// <summary>
		///     Shows how to work with the attribute configuration.
		/// </summary>
		/// <param name="client">The client.</param>
		public static async Task Lesson( DataServiceRestClient client )
		{
			//Create attributes
			await client.CreateAttributeDefinition( EntityDto.Part, AttributeDefinition );

			//This will create an attribute which can be used by catalogs. Don't be confused, this is no catalog attribute
			await client.CreateAttributeDefinition( EntityDto.Catalog, CatalogColumnAttributeDefinition );

			//Create a catalog which we can use for our catalog attribute
			await client.CreateCatalogs( new[] { Catalog } );

			//Create the catalog attribute definition. 
			await client.CreateAttributeDefinition( EntityDto.Part, CatalogAttributeDefinition );

			//Notes: - you can't create catalog attributes for catalogs (Entity.Catalog)
			//Notes: - you must obey the shown order of commands!

			//You can update everything except the key, which is the identifier.
			//To change the key, you must delete and recreate the attribute, but be aware: all data stored for this attribute will be lost! 
			CatalogAttributeDefinition.Description = "Characteristic catalog attribute";
			await client.UpdateAttributeDefinitions( EntityDto.Characteristic, new[] { CatalogAttributeDefinition } );

			//Get all attributes
			var configuration = await client.GetConfiguration();

			//Attributes are assigned to an entity: part, characteristic, measurement, value or catalog.
			Console.WriteLine( $"Attributes for part: {configuration.PartAttributes.Count}" );
			Console.WriteLine( $"Attributes for characteristic: {configuration.CharacteristicAttributes.Count}" );
			Console.WriteLine( $"Attributes for measurement: {configuration.MeasurementAttributes.Count}" );
			Console.WriteLine( $"Attributes for value: {configuration.ValueAttributes.Count}" );
			Console.WriteLine( $"Attributes for catalog: {configuration.CatalogAttributes.Count}" );
		}

		public static async Task UndoLesson( DataServiceRestClient client )
		{
			//Delete attributes
			await client.DeleteAttributeDefinitions( EntityDto.Part, new[] { AttributeDefinition.Key } );
			await client.DeleteAttributeDefinitions( EntityDto.Characteristic, new[] { CatalogAttributeDefinition.Key } );
			await client.DeleteAttributeDefinitions( EntityDto.Catalog, new[] { CatalogColumnAttributeDefinition.Key } );

			//Delete catalogs
			await client.DeleteCatalogs( new[] { Catalog.Uuid } );
		}

		#endregion
	}
}