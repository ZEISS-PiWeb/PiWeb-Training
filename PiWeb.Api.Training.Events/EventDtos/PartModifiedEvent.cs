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
/// Represents the event that will be published after a part was modified.
/// </summary>
public record PartModifiedEvent
{
	#region properties

	/// <summary>
	/// Gets the unique identifier of the modified part.
	/// </summary>
	public Guid PartUuid { get; init; }

	#endregion
}