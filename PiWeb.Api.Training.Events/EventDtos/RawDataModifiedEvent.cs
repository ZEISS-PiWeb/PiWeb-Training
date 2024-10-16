#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2024                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace PiWeb.Api.Training.Events.EventDtos;

/// <summary>
/// Represents the event that will be published after a raw data file was modified.
/// </summary>
public record RawDataModifiedEvent
{
	#region properties

	/// <summary>
	/// Gets the raw data key.
	/// </summary>
	public int Key { get; init; }

	/// <summary>
	/// Gets the unique identifier of the inspection plan entity (part, characteristic, measurement, value) where the raw data was modified.
	/// </summary>
	/// <remarks>Raw data of values use a compound UUID in the form of MeasurementUuid|CharacteristicUuid.</remarks>
	public string TargetUuid { get; init; } = string.Empty;

	/// <summary>
	/// Gets the type of target entity (Part, Characteristic, Measurement, Value).
	/// </summary>
	public string RawDataEntity { get; init; } = string.Empty;

	/// <summary>
	/// Gets the old file name of the raw data before modification.
	/// </summary>
	public string OldFilename { get; init; } = string.Empty;

	/// <summary>
	/// Gets the new file name of the raw data after modification.
	/// </summary>
	public string NewFilename { get; init; } = string.Empty;

	#endregion
}