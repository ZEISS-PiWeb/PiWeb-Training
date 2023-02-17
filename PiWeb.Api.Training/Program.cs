#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2017                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace PiWeb.Api.Training
{
	#region usings
	
	using Lessons;
	using Zeiss.PiWeb.Api.Rest.HttpClient.Data;
	using Zeiss.PiWeb.Api.Rest.HttpClient.RawData;

	#endregion

	class Program
	{
		#region methods

		static void Main( )
		{
			var client = new DataServiceRestClient(new System.Uri("http://127.0.0.1:8080/"));
			var rawDataClient = new RawDataServiceRestClient( new System.Uri( "http://127.0.0.1:8080/" ) );

			ServiceInformation.Lesson( client ).Wait();

			Configuration.Lesson( client ).Wait();
			Configuration.UndoLesson( client ).Wait();

			Catalogs.Lesson( client ).Wait();
			Catalogs.UndoLesson( client ).Wait();

			InspectionPlan.Lesson( client ).Wait();
			InspectionPlan.UndoLesson( client ).Wait();

			Measurements.Lesson( client ).Wait();
			Measurements.UndoLesson( client ).Wait();

			RawData.Lesson( client, rawDataClient ).Wait();
			RawData.UndoLesson( client, rawDataClient ).Wait();
		}

		#endregion
	}
}