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
/// Represents the event that will be published after a measurement was created.
/// </summary>
public record MeasurementCreatedEvent
{
	#region properties

	/// <summary>
	/// Gets the unique identifier of the part where the new measurement was created.
	/// </summary>
	public Guid PartUuid { get; init; }

	/// <summary>
	/// Gets the unique identifier of the newly created measurement.
	/// </summary>
	public Guid MeasurementUuid { get; init; }

	#endregion
}