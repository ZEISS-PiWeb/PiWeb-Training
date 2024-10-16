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
/// Represents the event that will be published after a characteristic was moved within the same inspection plan.
/// </summary>
public record CharacteristicMovedEvent
{
	#region properties

	/// <summary>
	/// Gets the unique identifier of the moved characteristic.
	/// </summary>
	public Guid CharacteristicUuid { get; init; }

	/// <summary>
	/// Gets the source path where the characteristic was located before movement or
	/// <see cref="string.Empty"/> if the client is not authorized for read access.
	/// </summary>
	public string FromPath { get; init; } = string.Empty;

	/// <summary>
	/// Gets the target path where the characteristic was moved to or
	/// <see cref="string.Empty"/> if the client is not authorized for read access.
	/// </summary>
	public string ToPath { get; init; } = string.Empty;

	#endregion
}