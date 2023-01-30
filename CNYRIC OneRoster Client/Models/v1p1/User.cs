using CsvHelper;
using RestSharp;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Globalization;

namespace CNYRIC_OneRosterClient
{
    public class User
    {
      public string SourcedId { get; set; }

			public string Status { get; set; } = "";

      public DateTimeOffset DateLastModified { get; set; }

		public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

      public string UserMasterIdentifier { get; set; } = "";

		public string Identifier { get; set; } = "";

		public string Username { get; set; } = "";

		public string EnabledUser { get; set; } = "true";

      public string GivenName { get; set; } = "";

		public string FamilyName { get; set; } = "";

		public string MiddleName { get; set; } = "";

		public string PreferredFirstName { get; set; } = "";

		public string PreferredLastName { get; set; } = "";

		public string PreferredMiddleName { get; set; } = "";

		public string Email { get; set; } = "";

		public string Phone { get; set; } = "";

		public string Sms { get; set; } = "";

		public List<UserId> UserIds { get; set; }=new List<UserId> { new UserId() };

      /// <summary>
      /// v1p1 only
      /// </summary>
      public string Role { get; set; } = "";

		/// <summary>
		/// v1p2 only
		/// </summary>
		public List<UserRole> Roles { get; set; } = new List<UserRole>();

		public List<GuidRef> Agents { get; set; } = new List<GuidRef>();

		public List<GuidRef> Orgs { get; set; } = new List<GuidRef>();

		public List<string> Grades { get; set; } = new List<string>();

		/// <summary>
		/// v1p2 only
		/// </summary>
		public UserRole RoleTypesForGreatestPrimaryEnrollment { get; set; } = new UserRole();

		/// <summary>
		/// v1p1 only
		/// </summary>
		public string Password { get; set; } = "";

		public static void CsvHeader(CsvWriter writer)
		{
			writer.WriteField("sourcedId", true);
			writer.WriteField("status", true);
			writer.WriteField("dateLastModified", true);

			writer.WriteField("enabledUser", true);
			writer.WriteField("orgSourcedIds", true);
			writer.WriteField("role", true);
			writer.WriteField("username", true);
			writer.WriteField("userIds", true);
			writer.WriteField("givenName", true);
			writer.WriteField("familyName", true);
			writer.WriteField("middleName", true);
			writer.WriteField("identifier", true);
			writer.WriteField("email", true);
			writer.WriteField("sms", true);
			writer.WriteField("phone", true);
			writer.WriteField("agentSourcedIds", true);
			writer.WriteField("grades", true);
			writer.WriteField("password", true);

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

			writer.WriteField(EnabledUser.ToString().ToLower(), true);
			writer.WriteField(String.Join(",", Orgs.Select(uo => uo.SourcedId)),true);
			writer.WriteField(Role, true);
			writer.WriteField(Username, true);
			writer.WriteField(UserIds == null ? "" : String.Join(",", UserIds.FindAll(i=>i.Identifier!=null).Select(ui => $"{{{ui.Type}:{ui.Identifier}}}")),true);
			writer.WriteField(GivenName, true);
			writer.WriteField(FamilyName, true);
			writer.WriteField(MiddleName, true);
			writer.WriteField(Identifier, true);
			writer.WriteField(Email, true);
			writer.WriteField(Sms, true);
			writer.WriteField(Phone, true);
			writer.WriteField(String.Join(",", Agents.FindAll(a=>a.SourcedId!=null).Select(ua => ua.SourcedId)),true);
			writer.WriteField(String.Join(",", Grades),true);
			writer.WriteField(Password, true);

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

    public class Users
    {
      public List<User> users { get; set; }
    }

    public class SingleUser
    {
			public List<User> user { get; set; }
    }

	public class StudentsManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public StudentsManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// get all students enrolled for the current school year.
		/// ApiParameters p optional
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Users GetAllStudents(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Users>(_request);
		}
		public RestResponse GetAllStudentsRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Users> GetAllStudentsAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Users>(_request);
		}

		/// <summary>
		/// To read, get, one student by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleUser GetStudent(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleUser>(_request);
		}
		public RestResponse GetStudentRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleUser> GetStudentAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleUser>(_request);
		}

		/// <summary>
		/// To read, get, a collection of classes i.e. all classes for the current school year for a student. 
		/// Properties that are not supported: resources, grades. Status is always active, classCode is section teacher display name.
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public Classes GetClassesForStudent(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Classes>(_request);
		}
		public RestResponse GetClassesForStudentRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Classes> GetClassesForStudentAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/students/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Classes>(_request);
		}

		//straight csv from APICall
		public string GetStudentsCsv(ApiParameters? p = null, bool includeHeader = true)
		{
			Users Records = GetAllStudents(p);
			var sb = new StringBuilder();

			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if(includeHeader)User.CsvHeader(csv);

				foreach (User Record in Records.users)
				{
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		//Make csv from object
		public string GetStudentsCsv(Users Records, bool includeHeader = true)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) User.CsvHeader(csv);

				foreach (User record in Records.users)
				{
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}


	}

	public class TeachersManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public TeachersManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// To read, get, a collection of teachers i.e. all teachers registered to teach for the current school year. 
		/// Properties that are not supported: userProfiles, userIds, identifier, grades, username is not valid for login.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Users GetAllTeachers(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Users>(_request);
		}
		public RestResponse GetAllTeachersRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Users> GetAllTeachersAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Users>(_request);
		}

		/// <summary>
		/// To read, get, one teacher by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleUser GetTeacher(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleUser>(_request);
		}
		public RestResponse GetTeacherRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleUser> GetTeacherAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleUser>(_request);
		}

		/// <summary>
		/// To read, get, a collection of classes i.e. all classes for the current school year for a teacher. 
		/// Properties that are not supported: resources, grades. Status is always active, classCode is section teacher display name.
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public Classes GetClassesForTeacher(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Classes>(_request);
		}
		public RestResponse GetClassesForTeacherRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Classes> GetClassesForTeacherAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/teachers/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Classes>(_request);
		}

		//straight csv from APICall
		public string GetTeachersCsv(ApiParameters? p = null, bool includeHeader = true)
		{
			Users Records = GetAllTeachers(p);
			var sb = new StringBuilder();

			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if(includeHeader)User.CsvHeader(csv);
				foreach (User Record in Records.users)
				{
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		//Make csv from object
		public string GetTeachersCsv(Users Records, bool includeHeader = true	)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) User.CsvHeader(csv);

				foreach (User record in Records.users)
				{
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

	}

	public class UsersManagement
	{
		private readonly V1p1 _oneRosterApi;
		private readonly RestRequest _request;

		public UsersManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// To read, get, a collection of users i.e. all user for the current school year. 
		/// Properties that are not supported: userProfiles, userIds, identifier, username is not valid for login. 
		/// Filtering by grades of students is not supported.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Users GetAllUsers(ApiParameters? p = null)
		{
		

			_request.Method = Method.Get;
			_request.Resource = $"/users/";
			_oneRosterApi.AddRequestParameters(_request, p);

			Users users = _oneRosterApi.Execute<Users>(_request);
			return users;
		}
		public RestResponse GetAllUsersRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/users/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}

		/// <summary>
		/// To read, get, one user by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public User GetUser(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/users/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			SingleUser theseusers = _oneRosterApi.Execute<SingleUser>(_request);
			User thisuser = theseusers.user.FirstOrDefault();
			return thisuser;
		}
		public RestResponse GetUserRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/users/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}

		/// <summary>
		/// To read, get, a collection of classes i.e. all classes for the current school year for a student or teacher. 
		/// Properties that are not supported: resources, grades. Status is always active, classCode is section teacher display name.
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public Classes GetClassesForUser(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/users/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Classes>(_request);
		}
		public RestResponse GetClassesForUserRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/users/{sourcedId}/classes";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}

		//straight csv from APICall
		public string GetUsersCsv(ApiParameters? p = null, bool includeHeader = true)
		{
			//_request.Method = Method.Get;
			//_request.Resource = $"/users/";
			//_oneRosterApi.AddRequestParameters(_request, p);
			Users Records = GetAllUsers(p);

			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) User.CsvHeader(csv);
				foreach (User Record in Records.users)
				{
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		//Make csv from object
		public string GetUsersCsv(Users Records, bool includeHeader = true)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) User.CsvHeader(csv);

				foreach (User record in Records.users)
				{
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}



	}
}