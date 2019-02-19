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
	using PiWeb.Api.Training.Helpers;
	using Zeiss.IMT.PiWeb.Api.DataService.Rest;
	using Attribute = Zeiss.IMT.PiWeb.Api.DataService.Rest.Attribute;

	#endregion

	public static class Catalogs
	{
		#region members

		private static readonly AbstractAttributeDefinition MachineName = new AttributeDefinition
		{
			Key = 14001,
			Description = "Machine name",
			Type = AttributeType.AlphaNumeric,
			Length = 255
		};

		private static readonly AbstractAttributeDefinition MachineNumber = new AttributeDefinition
		{
			Key = 14002,
			Description = "Machine number",
			Type = AttributeType.Integer
		};

		private static readonly AbstractAttributeDefinition MachineVendor = new AttributeDefinition
		{
			Key = 14003,
			Description = "Machine vendor",
			Type = AttributeType.AlphaNumeric,
			Length = 255
		};

		private static readonly CatalogEntry EntryAccura = new CatalogEntry
		{
			Key = 1,
			Attributes = new[] { new Attribute( MachineName.Key, "Accura" ), new Attribute( MachineNumber.Key, 1 ) }
		};

		private static readonly CatalogEntry EntryContura = new CatalogEntry
		{
			Key = 2,
			Attributes = new[] { new Attribute( MachineName.Key, "Contura" ), new Attribute( MachineNumber.Key, 2 ) }
		};

		private static readonly CatalogEntry EntryXenos = new CatalogEntry
		{
			Key = 3,
			Attributes =
				new[]
				{
					new Attribute( MachineName.Key, "Xenos" ), new Attribute( MachineNumber.Key, 3 ),
					new Attribute( MachineVendor.Key, "Zeiss" )
				}
		};

		private static readonly Catalog MachineCatalog = new Catalog
		{
			ValidAttributes = new[] { MachineName.Key, MachineNumber.Key },
			Name = "Machine catalog",
			Uuid = Guid.NewGuid(),
			CatalogEntries = new[]
			{
				EntryAccura,
				EntryContura
			}
		};

		#endregion

		#region methods

		public static async Task Lesson( DataServiceRestClient client )
		{
			//A catalog must have at least one attribute.
			//Attributes with entity 'Catalog' must not be of type 'CatalogAttribute'
			await client.CreateAttributeDefinitions( Entity.Catalog, new[] { MachineName, MachineNumber, MachineVendor } );

			//Create the catalog
			await client.CreateCatalogs( new[] { MachineCatalog } );

			//Add an attribute
			MachineCatalog.ValidAttributes = MachineCatalog.ValidAttributes.Append( MachineVendor.Key );

			await client.UpdateCatalogs( new[] { MachineCatalog } );

			//Add an entry
			await client.CreateCatalogEntries( MachineCatalog.Uuid, new[] { EntryXenos } );

			//Update existing catalog entries
			EntryAccura.Attributes = EntryAccura.Attributes.Append( new Attribute( MachineVendor.Key, "Zeiss" ) );
			EntryContura.Attributes = EntryContura.Attributes.Append( new Attribute( MachineVendor.Key, "Zeiss" ) );

			await client.UpdateCatalogs( new[] { MachineCatalog } );
		}

		public static async Task UndoLesson( DataServiceRestClient client )
		{
			//Delete the catalogs first, since deleting attributes which are used by catalogs will cause an error
			await client.DeleteCatalogs( new[] { MachineCatalog.Uuid } );
			await
				client.DeleteAttributeDefinitions( Entity.Catalog, new[] { MachineName.Key, MachineNumber.Key, MachineVendor.Key } );
		}

		#endregion
	}
}