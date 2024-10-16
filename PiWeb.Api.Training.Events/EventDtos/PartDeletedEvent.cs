﻿#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2024                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

using System;

namespace PiWeb.Api.Training.Events.EventDtos;

/// <summary>
/// Represents the event that will be published after a part was deleted.
/// </summary>
public record PartDeletedEvent
{
	#region properties

	/// <summary>
	/// Gets the unique identifier of the parent part where the part was removed from.
	/// </summary>
	public Guid ParentPartUuid { get; init; }

	/// <summary>
	/// Gets the unique identifier of the deleted part.
	/// </summary>
	public Guid PartUuid { get; init; }

	#endregion
}