#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2017                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace PiWeb.Api.Training.Helpers
{
	#region usings

	using System.Collections.Generic;

	#endregion

	public static class CollectionExtensions
	{
		#region methods

		public static T[] Append<T>( this ICollection<T> array, T item )
		{
			var result = new List<T>( array );
			result.Add( item );

			return result.ToArray();
		}

		#endregion
	}
}