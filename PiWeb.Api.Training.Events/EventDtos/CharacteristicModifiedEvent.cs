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
/// Represents the event that will be published after a characteristic was modified.
/// </summary>
public record CharacteristicModifiedEvent
{
	#region properties

	/// <summary>
	/// Gets the unique identifier of the parent part where the characteristic was modified.
	/// </summary>
	/// <remarks>
	/// The parent part is possibly not the direct parent in the inspection plan tree.
	/// It is possible, that the direct parent is a characteristic.
	/// </remarks>
	public Guid ParentPartUuid { get; init; }

	/// <summary>
	/// Gets the unique identifier of the modified characteristic.
	/// </summary>
	public Guid CharacteristicUuid { get; init; }

	#endregion
}