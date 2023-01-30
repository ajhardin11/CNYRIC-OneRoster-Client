using CsvHelper;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNYRIC_OneRosterClient
{
	public class Enrollment
	{
		public string? SourcedId { get; set; }

		public string Status { get; set; } = "";

		public DateTimeOffset DateLastModified { get; set; }=DateTimeOffset.Now;

		public Dictionary<string, string> Metadata { get; set; }=new Dictionary<string, string>();

		public GuidRef User { get; set; } = new GuidRef();

		public GuidRef Class { get; set; } = new GuidRef();

		public GuidRef School { get; set; }= new GuidRef();
		public string Role { get; set; } = "";

		public string Primary { get; set; } = "false";					

		public DateTimeOffset BeginDate { get; set; }= DateTimeOffset.Now;

		public DateTimeOffset EndDate { get; set; } = DateTimeOffset.Now;		

		public static void CsvHeader(CsvWriter writer)
		{
			writer.WriteField("sourcedId", true);
			writer.WriteField("status", true);
			writer.WriteField("dateLastModified", true);

			writer.WriteField("classSourcedId", true);
			writer.WriteField("schoolSourcedId", true);
			writer.WriteField("userSourcedId", true);
			writer.WriteField("role", true);
			writer.WriteField("primary", true);
			writer.WriteField("beginDate", true);
			writer.WriteField("endDate", true);

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

			writer.WriteField(Class.SourcedId, true);
			writer.WriteField(School.SourcedId, true);
			writer.WriteField(User.SourcedId, true);
			writer.WriteField(Role, true);
			writer.WriteField(Primary.ToString().ToLower(), true);
			writer.WriteField(BeginDate.ToString("yyyy-MM-dd"), true);
			writer.WriteField(EndDate.ToString("yyyy-MM-dd"), true);

			//if (Metadata.Count > 0)
			//{
			//	foreach (string item in Metadata.Values)
			//	{
			//		writer.WriteField(item, true);
			//	}
			//}

			writer.NextRecord();
		}

	}

	public class Enrollments
	{
		public List<Enrollment> enrollments { get; set; }

	}

	public class SingleEnrollment
	{
		public Enrollment enrollment { get; set; }
	}

	public class EnrollmentsManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public EnrollmentsManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// To read, get, a collection of enrollment data. An enrollment is the name given to an individual taking part in a class. 
		/// Users will be students learning or teachers teaching in a class. sourcedId refers to roster ID for students and history ID for 
		/// teachers. role only supports students and teachers.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Enrollments GetAllEnrollments(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/enrollments/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Enrollments>(_request);
		}
		public RestResponse GetAllEnrollmentsRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/enrollments/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Enrollments> GetAllEnrollmentsAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/enrollments/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Enrollments>(_request);
		}

		/// <summary>
		/// To read, get, one enrollment by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleEnrollment GetEnrollment(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/enrollments/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleEnrollment>(_request);
		}
		public RestResponse GetEnrollmentRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/enrollments/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleEnrollment> GetEnrollmentAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/enrollments/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleEnrollment>(_request);
		}

		//straight csv from APICall
		public string GetEnrollmentsCsv(ApiParameters? p = null, bool includeHeader=true)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/enrollments/";
			_oneRosterApi.AddRequestParameters(_request, p);
			Enrollments Records = _oneRosterApi.Execute<Enrollments>(_request);

			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if(includeHeader)Enrollment.CsvHeader(csv);
				foreach (Enrollment Record in Records.enrollments)
				{
					//set primary to false if role is not a teacher
					if(Record.Role!="teacher")
					{
						Record.Primary = "false";
					}
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		//Make csv from object
		public string GetEnrollmentsCsv(Enrollments Records, bool includeHeader=true)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if(includeHeader)Enrollment.CsvHeader(csv);

				foreach (Enrollment record in Records.enrollments)
				{
					//set primary to false if role is not a teacher
					if (record.Role != "teacher")
					{
						record.Primary = "false";
					}
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}


	}
}
