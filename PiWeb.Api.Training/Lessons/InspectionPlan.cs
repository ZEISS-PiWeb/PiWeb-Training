#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2017                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace PiWeb.Api.Training.Lessons
{
	#region usings

	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Zeiss.IMT.PiWeb.Api.Common.Data;
	using Zeiss.IMT.PiWeb.Api.DataService.Rest;

	#endregion

	public static class InspectionPlan
	{
		#region members

		//The path structure consists of capital 'P' and 'C' characters, one for each path element. 'P' masks a part, while 'C' masks a characteristic. 

		private static readonly InspectionPlanPart Part = new InspectionPlanPart { Path = PathHelper.RoundtripString2PathInformation( "P:/PartName/" ), Uuid = Guid.NewGuid() };
		private static readonly InspectionPlanPart SubPart = new InspectionPlanPart { Path = PathHelper.RoundtripString2PathInformation( "PP:/PartName/SubPart/" ), Uuid = Guid.NewGuid() };
		private static readonly InspectionPlanCharacteristic SubChild = new InspectionPlanCharacteristic { Path = PathHelper.RoundtripString2PathInformation( "PPC:/PartName/SubPart/Child/" ), Uuid = Guid.NewGuid() };

		private static readonly InspectionPlanCharacteristic Characteristic = new InspectionPlanCharacteristic { Path = PathHelper.RoundtripString2PathInformation( "PC:/PartName/CharName/"), Uuid = Guid.NewGuid() };
		private static readonly InspectionPlanCharacteristic Child = new InspectionPlanCharacteristic { Path = PathHelper.RoundtripString2PathInformation( "PCC:/PartName/CharName/Child/" ), Uuid = Guid.NewGuid() };

		#endregion

		#region methods

		public static async Task Lesson( DataServiceRestClient client )
		{

			await client.CreateParts( new[] { Part, SubPart } );
			await client.CreateCharacteristics( new[] { Characteristic, Child, SubChild } );

			//Depth null will result in a recursive search
			var result = await client.GetCharacteristics( Part.Path, null );
			Console.WriteLine($"Depth null: {result.Count()} characteristics");

			//Depth 0 will return an empty list
			result = await client.GetCharacteristics( Part.Path, 0 );
			Console.WriteLine( $"Depth 0: {result.Count()} characteristics" );

			//Depth 1 will return the direct child characteristics
			result = await client.GetCharacteristics( Part.Path, 1 );
			Console.WriteLine( $"Depth 1: {result.Count()} characteristics" );

			//Depth 2 will return the direct children and their children, but not the children of subparts
			result = await client.GetCharacteristics( Part.Path, 2 );
			Console.WriteLine( $"Depth 2: {result.Count()} characteristics" );
			
			//Use UpdateParts and UpdateCharacteristics to rename entities, and modify their attributes
			Part.Path = PathHelper.RoundtripString2PathInformation( "P:/PartName2/" );
			await client.UpdateParts( new[] { Part }  );
		}

		public static async Task UndoLesson( DataServiceRestClient client )
		{
			await client.DeleteParts( new[] { Part.Uuid } );
			await client.DeleteCharacteristics( new[] { Characteristic.Uuid, Child.Uuid } );
		}

		#endregion
	}
}