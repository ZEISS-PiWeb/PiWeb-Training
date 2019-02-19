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
	
	using PiWeb.Api.Training.Lessons;
	using Zeiss.IMT.PiWeb.Api.DataService.Rest;
	using Zeiss.IMT.PiWeb.Api.RawDataService.Rest;
	using Configuration = PiWeb.Api.Training.Lessons.Configuration;
	using RawData = PiWeb.Api.Training.Lessons.RawData;
	using ServiceInformation = PiWeb.Api.Training.Lessons.ServiceInformation;

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