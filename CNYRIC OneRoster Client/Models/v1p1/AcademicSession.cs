using CsvHelper;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CNYRIC_OneRosterClient
{
	public class AcademicSession
	{
		public string SourcedId { get; set; }

		public string Status { get; set; } = "";

		public DateTimeOffset DateLastModified { get; set; } = DateTimeOffset.Now;

		public Dictionary<string, string> Metadata { get; set; }=new Dictionary<string, string>();

		public string Title { get; set; } = "";

		public string SchoolYear { get; set; } = "";

		public DateTimeOffset StartDate { get; set; }

		public DateTimeOffset EndDate { get; set; }

		public List<GuidRef> Children { get; set; } = new List<GuidRef>();

		public GuidRef Parent { get; set; }=new GuidRef();

		public string Type { get; set; } = "";		

		public static void CsvHeader(CsvWriter writer)
		{
			writer.WriteField("sourcedId",true);
			writer.WriteField("status", true);
			writer.WriteField("dateLastModified", true);

			writer.WriteField("title", true);
			writer.WriteField("type", true);
			writer.WriteField("startDate", true);
			writer.WriteField("endDate", true);
			writer.WriteField("parentSourcedId", true);
			writer.WriteField("schoolYear", true);

			writer.NextRecord();
		}

		public void AsCsvRow(CsvWriter writer, bool bulk = true)
		{
			writer.WriteField(SourcedId, true);

			if (bulk)
			{
				writer.WriteField("", true);
				writer.WriteField("", true);
			}
			else
			{
				writer.WriteField(Status, true);
				writer.WriteField(DateLastModified.ToString(), true);
			}

			writer.WriteField(Title, true);
			writer.WriteField(Type,true);
			writer.WriteField(StartDate.ToString("yyyy-MM-dd"),true);
			writer.WriteField(EndDate.ToString("yyyy-MM-dd"),true);
			writer.WriteField(Parent.SourcedId,true);
			writer.WriteField(SchoolYear, true);

			//if (Metadata.Count > 0)
			//{
			//	foreach(string item in Metadata.Values)
			//	{
			//		writer.WriteField(item,true);
			//	}
			//}

			writer.NextRecord();
		}

	}

	public class AcademicSessions
	{
		public List<AcademicSession> academicSessions { get; set; }
	}

	public class SingleAcademicSession
	{
		public AcademicSession academicSession { get; set; }
	}

	public class AcademicSessionsManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public AcademicSessionsManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// get academic sessions for the current school year. Refers to campus terms.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public AcademicSessions GetAllAcademicSessions(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<AcademicSessions>(_request);
		}
		public RestResponse GetAllAcademicSessionsRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<AcademicSessions> GetAllAcademicSessionsAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<AcademicSessions>(_request);
		}

		/// <summary>
		/// To read, get, one academicSession by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleAcademicSession GetAcademicSession(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleAcademicSession>(_request);
		}
		public RestResponse GetAcademicSessionRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleAcademicSession> GetAcademicSessionAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleAcademicSession>(_request);
		}

		//straight csv from APICall
		public string GetAcademicSessionsCsv(ApiParameters? p = null, bool includeHeader = true	)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/";
			_oneRosterApi.AddRequestParameters(_request, p);
			AcademicSessions Records = _oneRosterApi.Execute<AcademicSessions>(_request);

			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) AcademicSession.CsvHeader(csv);
				foreach (AcademicSession Record in Records.academicSessions)
				{
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		//Make csv from object
		public string GetAcademicSessionsCsv(AcademicSessions Records, bool includeHeader = true	)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) AcademicSession.CsvHeader(csv);

				foreach (AcademicSession record in Records.academicSessions)
				{
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}
	}

	public class TermsManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public TermsManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// To read, get, a collection of terms i.e. all terms for the current school year. Refers to campus terms by term ID. status is always active.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public AcademicSessions GetAllTerms(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<AcademicSessions>(_request);
		}
		public RestResponse GetAllTermsRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<AcademicSessions> GetAllTermsAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<AcademicSessions>(_request);
		}

		/// <summary>
		/// To read, get, one term by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleAcademicSession GetTerm(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleAcademicSession>(_request);
		}
		public RestResponse GetTermRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleAcademicSession> GetTermAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleAcademicSession>(_request);
		}

		/// <summary>
		/// To read, get, classes for a term by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public Classes GetClassesForTerm(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Classes>(_request);
		}
		public RestResponse GetClassesForRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Classes> GetClassesForTermAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Classes>(_request);
		}

		/// <summary>
		/// To read, get, grading periods for a term by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public AcademicSessions GetGradingPeriodsForTerm(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}/gradingPeriods";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<AcademicSessions>(_request);
		}
		public RestResponse GetGradingPeriodsForTermRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}/gradingPeriods";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<AcademicSessions> GetGradingPeriodsForTermAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/terms/{sourcedId}/gradingPeriods";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<AcademicSessions>(_request);
		}

	}

	public class GradingPeriodsManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public GradingPeriodsManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// To read, get, a collection of grading periods i.e. all grading periods for the current school year. 
		/// Refers to campus terms and schedule sets by term GUID and structure GUID.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public AcademicSessions GetAllGradingPeriods(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/gradingPeriods/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<AcademicSessions>(_request);
		}
		public RestResponse GetAllGradingPeriodsRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/gradingPeriods/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<AcademicSessions> GetAllGradingPeriodsAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/gradingPeriods/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<AcademicSessions>(_request);
		}

		/// <summary>
		/// To read, get, one grading period by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleAcademicSession GetGradingPeriod(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/gradingPeriods/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleAcademicSession>(_request);
		}
		public RestResponse GetGradingPeriodRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/gradingPeriods/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleAcademicSession> GetGradingPeriodAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/gradingPeriods/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleAcademicSession>(_request);
		}


	}
}
