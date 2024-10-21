#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2024                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

using System;

namespace PiWeb.Api.Training.Events.EventDtos;

/// <summary>
/// Represents the event that will be published after a characteristic was created.
/// </summary>
public record CharacteristicCreatedEvent
{
	#region properties

	/// <summary>
	/// Gets the unique identifier of the parent inspection plan item (either a part or a characteristic) where the new characteristic was added to.
	/// </summary>
	public Guid ParentUuid { get; init; }

	/// <summary>
	/// Gets the unique identifier of the newly created characteristic.
	/// </summary>
	public Guid CharacteristicUuid { get; init; }

	#endregion
}