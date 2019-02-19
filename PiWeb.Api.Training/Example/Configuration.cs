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
	using Zeiss.IMT.PiWeb.Api.DataService.Rest;

	public static class Configuration
	{
		#region methods

		public static void CheckAttribute( DataServiceRestClient client, Entity entity, ushort key, string description,
			AttributeType type )
		{
			var configuration = client.GetConfiguration().Result;
			var definition = configuration.GetDefinition( key );

			if( definition == null )
			{
				client.CreateAttributeDefinition( entity, new AttributeDefinition
				{
					Key = key,
					Description = description,
					Type = type
				} );
			}
			else if( configuration.GetTypeForKey( key ) != entity || definition.Description != description ||
			         ( definition as AttributeDefinition )?.Type != type )
			{
				//Don't do this, in case the PiWeb database is already in use. Changing the configuration will cause data-loss!!!
				client.UpdateAttributeDefinitions( entity, new AbstractAttributeDefinition[]
				{
					new AttributeDefinition
					{
						Key = key,
						Description = description,
						Type = type
					}
				} );
			}
		}

		public static void CheckCatalogAttribute( DataServiceRestClient client, Entity entity, ushort key, string description,
			Guid catalog )
		{
			var configuration = client.GetConfiguration().Result;
			var definition = configuration.GetDefinition( key );

			if( definition == null )
			{
				client.CreateAttributeDefinition( entity, new CatalogAttributeDefinition
				{
					Key = key,
					Description = description,
					Catalog = catalog
				} );
			}
			else if( configuration.GetTypeForKey( key ) != entity || definition.Description != description ||
			         ( definition as CatalogAttributeDefinition )?.Catalog != catalog )
			{
				client.UpdateAttributeDefinitions( entity, new AbstractAttributeDefinition[]
				{
					new CatalogAttributeDefinition
					{
						Key = key,
						Description = description,
						Catalog = catalog
					}
				} );
			}
		}

		#endregion
	}
}