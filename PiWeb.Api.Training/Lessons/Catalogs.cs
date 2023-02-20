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
	using System.Linq;
	using System.Threading.Tasks;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;
	using Attribute = Zeiss.PiWeb.Api.Core.Attribute;

	#endregion

	public static class Catalogs
	{
		#region members

		private static readonly AbstractAttributeDefinitionDto MachineName = new AttributeDefinitionDto
		{
			Key = 14001,
			Description = "Machine name",
			Type = AttributeTypeDto.AlphaNumeric,
			Length = 255
		};

		private static readonly AbstractAttributeDefinitionDto MachineNumber = new AttributeDefinitionDto
		{
			Key = 14002,
			Description = "Machine number",
			Type = AttributeTypeDto.Integer
		};

		private static readonly AbstractAttributeDefinitionDto MachineVendor = new AttributeDefinitionDto
		{
			Key = 14003,
			Description = "Machine vendor",
			Type = AttributeTypeDto.AlphaNumeric,
			Length = 255
		};

		private static readonly CatalogEntryDto EntryAccura = new CatalogEntryDto
		{
			Key = 1,
			Attributes = new[] { new Attribute( MachineName.Key, "Accura" ), new Attribute( MachineNumber.Key, 1 ) }
		};

		private static readonly CatalogEntryDto EntryContura = new CatalogEntryDto
		{
			Key = 2,
			Attributes = new[] { new Attribute( MachineName.Key, "Contura" ), new Attribute( MachineNumber.Key, 2 ) }
		};

		private static readonly CatalogEntryDto EntryXenos = new CatalogEntryDto
		{
			Key = 3,
			Attributes =
				new[]
				{
					new Attribute( MachineName.Key, "Xenos" ), new Attribute( MachineNumber.Key, 3 ),
					new Attribute( MachineVendor.Key, "Zeiss" )
				}
		};

		private static readonly CatalogDto MachineCatalog = new CatalogDto
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
			await client.CreateAttributeDefinitions( EntityDto.Catalog, new[] { MachineName, MachineNumber, MachineVendor } );

			//Create the catalog
			await client.CreateCatalogs( new[] { MachineCatalog } );

			//Add an attribute
			MachineCatalog.ValidAttributes = MachineCatalog.ValidAttributes.Append( MachineVendor.Key ).ToList();

			await client.UpdateCatalogs( new[] { MachineCatalog } );

			//Add an entry
			await client.CreateCatalogEntries( MachineCatalog.Uuid, new[] { EntryXenos } );

			//Update existing catalog entries
			EntryAccura.Attributes = EntryAccura.Attributes.Append( new Attribute( MachineVendor.Key, "Zeiss" ) ).ToList();
			EntryContura.Attributes = EntryContura.Attributes.Append( new Attribute( MachineVendor.Key, "Zeiss" ) ).ToList();

			await client.UpdateCatalogs( new[] { MachineCatalog } );
		}

		public static async Task UndoLesson( DataServiceRestClient client )
		{
			//Delete the catalogs first, since deleting attributes which are used by catalogs will cause an error
			await client.DeleteCatalogs( new[] { MachineCatalog.Uuid } );
			await client.DeleteAttributeDefinitions( EntityDto.Catalog, new[] { MachineName.Key, MachineNumber.Key, MachineVendor.Key } );
		}

		#endregion
	}
}