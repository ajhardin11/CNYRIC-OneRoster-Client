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
	public class Course
	{
		public string SourcedId { get; set; }

		public string Status { get; set; } = "";

		public DateTimeOffset DateLastModified { get; set; }=new DateTimeOffset();

		public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

		public string Title { get; set; } = "";

		//TO DO: SCHOOL YEAR: the AcademicSession 'sourcedId'.
		public string SchoolYear { get; set; } = "";
		public string CourseCode { get; set; } = "";

		/// <summary>
		/// v1p1 only
		/// </summary>
		public List<string> Grades { get; set; } = new List<string>();

		public List<string> Subjects { get; set; } = new List<string>();

		public List<string> SubjectCodes { get; set; } = new List<string>();

		public GuidRef Org { get; set; }=new GuidRef();
		public static void CsvHeader(CsvWriter writer)
		{
			writer.WriteField("sourcedId", true);
			writer.WriteField("status", true);
			writer.WriteField("dateLastModified", true);

			writer.WriteField("schoolYearSourcedId", true);
			writer.WriteField("title", true);
			writer.WriteField("courseCode", true);
			writer.WriteField("grades", true);
			writer.WriteField("orgSourcedId", true);
			writer.WriteField("subjects", true);
			writer.WriteField("subjectCodes", true);

			writer.NextRecord();
		}

		public  void AsCsvRow(CsvWriter writer, bool bulk = true)
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

			writer.WriteField(SchoolYear, true);
			writer.WriteField(Title, true);
			writer.WriteField(CourseCode, true);
			writer.WriteField(String.Join(",", Grades),true);
			writer.WriteField(Org.SourcedId, true);
			writer.WriteField(String.Join(",", Subjects),true);
			writer.WriteField(String.Join(",", SubjectCodes),true);

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

	public class Courses
	{
		public List<Course> courses { get; set; }
	}

	public class SingleCourse
	{
		public Course course { get; set; }
	}

	public class CoursesManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public CoursesManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// To read, get, a collection of courses i.e. all courses for the current school year. Refers to campus courses. 
		/// Properties that are not supported: schoolYear, grades, resources. status is always active
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Courses GetAllCourses(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Courses>(_request);
		}
		public RestResponse GetAllCoursesRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Courses> GetAllCoursesAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Courses>(_request);
		}

		/// <summary>
		/// To read, get, one course by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleCourse GetCourse(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleCourse>(_request);
		}
		public RestResponse GetCourseRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleCourse> GetCourseAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleCourse>(_request);
		}

		//straight csv from APICall
		public string GetCoursesCsv(ApiParameters? p = null, bool includeHeader=true)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/";
			_oneRosterApi.AddRequestParameters(_request, p);
			Courses Records = _oneRosterApi.Execute<Courses>(_request);

			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) Course.CsvHeader(csv);
				foreach (Course Record in Records.courses)
				{
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		//Make csv from object
		public string GetCoursesCsv(Courses Records, bool includeHeader = true)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) Course.CsvHeader(csv);

				foreach (Course record in Records.courses)
				{
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// To read, get, classes for a course by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public Classes GetClassesForCourse(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Classes>(_request);
		}
		public RestResponse GetClassesForCourseRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Classes> GetClassesForCourseAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/courses/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Classes>(_request);
		}


	}
}
