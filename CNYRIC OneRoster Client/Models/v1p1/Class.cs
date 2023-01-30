using CsvHelper;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace CNYRIC_OneRosterClient
{
    /// <summary>
    /// A class is an instance of a course, onto which students and teachers are enrolled. A class is typically held within a term.
    /// </summary>
    public class Class
    {
			public string SourcedId { get; set; }
			public string Status { get; set; } = "";
			public DateTimeOffset DateLastModified { get; set; }= DateTimeOffset.Now;
			public Dictionary<string, string> Metadata { get; set; }=new Dictionary<string, string>();
			public string Title { get; set; } = "";
			public string ClassType { get; set; } = "";
			public string ClassCode { get; set; } = "";
			public string Location { get; set; } = "";
			public List<string> Grades { get; set; } = new List<string>();
			public List<string> Subjects { get; set; }= new List<string>();
			public GuidRef Course { get; set; }=new GuidRef();
			public GuidRef School { get; set; } = new GuidRef();
			public List<GuidRef> Terms { get; set; } = new List<GuidRef>();
			public List<string> SubjectCodes { get; set; }=	new List<string>();
			public List<string> Periods { get; set; }=new List<string>();

		public static void CsvHeader(CsvWriter writer)
		{
			writer.WriteField("sourcedId",true);
			writer.WriteField("status",true);
			writer.WriteField("dateLastModified", true);

			writer.WriteField("title", true);
			writer.WriteField("grades", true);
			writer.WriteField("courseSourcedId", true);
			writer.WriteField("classCode", true);
			writer.WriteField("classType", true);
			writer.WriteField("location", true);
			writer.WriteField("schoolSourcedId", true);
			writer.WriteField("termSourcedIds", true);
			writer.WriteField("subjects", true);
			writer.WriteField("subjectCodes", true);
			writer.WriteField("periods", true);
			
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
					writer.WriteField(Status,true);
					writer.WriteField(DateLastModified.ToString(), true);
				}

				writer.WriteField(Title, true);
				writer.WriteField(String.Join(",", Grades),true);
				writer.WriteField(Course.SourcedId, true);
				writer.WriteField(ClassCode, true);
				writer.WriteField(ClassType, true);
				writer.WriteField(Location, true);
				writer.WriteField(School.SourcedId, true);
				writer.WriteField(String.Join(",", Terms.Select(kas => kas.SourcedId)),true);
				writer.WriteField(String.Join(",", Subjects),true);
				writer.WriteField(String.Join(",", SubjectCodes),true);
				writer.WriteField(String.Join(",", Periods),true);

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

    public class Classes
    {
      public List<Class> classes { get; set; }
    }

    public class SingleClass
    {
      public Class Class { get; set; }
    }



	public class ClassesManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public ClassesManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// Get classes for the current school year. 
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Classes GetAllClasses(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Classes>(_request);
		}
		public RestResponse GetAllClassesRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Classes> GetAllClassesAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Classes>(_request);
		}
		/// <summary>
		/// To read, get, one class by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleClass GetClass(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleClass>(_request);
		}
		public RestResponse GetClassRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleClass> GetClassAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleClass>(_request);
		}

		/// <summary>
		/// Gets classes and return csv string
		/// </summary>
		/// <param name="p"></param>
		/// <returns>string</returns>
		public string GetClassesCsv(ApiParameters? p = null, bool includeHeader = true	)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/";
			_oneRosterApi.AddRequestParameters(_request, p);
			Classes Records = _oneRosterApi.Execute<Classes>(_request);

			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) Class.CsvHeader(csv);
				foreach (Class Record in Records.classes)
				{
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets classes and return csv string
		/// required: Classes object
		/// </summary>
		/// <param name="Records"></param>
		/// <returns>string</returns>
		public string GetClassesCsv(Classes Records, bool includeHeader = true)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) Class.CsvHeader(csv);

				foreach (Class record in Records.classes)
				{
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// To read, get, a collection of students i.e. all students for the current school year for a class.
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public Users GetStudentsForClass(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}/students";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Users>(_request);
		}
		public RestResponse GetStudentsForClassRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}/students";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Users> GetStudentsForClassAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}/students";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Users>(_request);
		}

		/// <summary>
		/// To read, get, a collection of teachers i.e. all teachers for the current school year for a class.
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public Users GetTeachersForClass(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}/teachers";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Users>(_request);
		}
		public RestResponse GetTeachersForClassRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}/teachers";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Users> GetTeachersForClassAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/classes/{sourcedId}/teachers";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Users>(_request);
		}


	}
}