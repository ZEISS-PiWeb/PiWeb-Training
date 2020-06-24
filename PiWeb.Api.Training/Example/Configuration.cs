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
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;

	public static class Configuration
	{
		#region methods

		public static void CheckAttribute( DataServiceRestClient client, EntityDto entity, ushort key, string description,
			AttributeTypeDto type )
		{
			var configuration = client.GetConfiguration().Result;
			var definition = configuration.GetDefinition( key );

			if( definition == null )
			{
				client.CreateAttributeDefinition( entity, new AttributeDefinitionDto
				{
					Key = key,
					Description = description,
					Type = type
				} );
			}
			else if( configuration.GetTypeForKey( key ) != entity || definition.Description != description ||
			         ( definition as AttributeDefinitionDto )?.Type != type )
			{
				//Don't do this, in case the PiWeb database is already in use. Changing the configuration will cause data-loss!!!
				client.UpdateAttributeDefinitions( entity, new AbstractAttributeDefinitionDto[]
				{
					new AttributeDefinitionDto
					{
						Key = key,
						Description = description,
						Type = type
					}
				} );
			}
		}

		public static void CheckCatalogAttribute( DataServiceRestClient client, EntityDto entity, ushort key, string description,
			Guid catalog )
		{
			var configuration = client.GetConfiguration().Result;
			var definition = configuration.GetDefinition( key );

			if( definition == null )
			{
				client.CreateAttributeDefinition( entity, new CatalogAttributeDefinitionDto
				{
					Key = key,
					Description = description,
					Catalog = catalog
				} );
			}
			else if( configuration.GetTypeForKey( key ) != entity || definition.Description != description ||
			         ( definition as CatalogAttributeDefinitionDto )?.Catalog != catalog )
			{
				client.UpdateAttributeDefinitions( entity, new AbstractAttributeDefinitionDto[]
				{
					new CatalogAttributeDefinitionDto
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