using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNYRIC_OneRosterClient
{ 
	public class V1p1 : IDisposable
	{
		private bool disposed;
		public Token Token;
		private RestClient _restClient;
		private string _tokenUrl;

		public bool TokenIsValid()
		{
			return Token.NotExpired();
		}

		public V1p1(string baseUrl, string clientId, string clientSecret)
		{
			//https://SomeDistrict.schooltool.cnyric.org/STAPIService/ims/oneroster/v1p1/token
			_tokenUrl = string.Concat(baseUrl != null ? baseUrl.TrimEnd('/') : baseUrl, "/Token");
			Token = new Oauth2(_tokenUrl).GetToken(clientId, clientSecret);
			_restClient = new RestClient(baseUrl != null ? baseUrl.TrimEnd('/') : baseUrl);
		}

		public OrgsManagement OrgsManagement => new OrgsManagement(this);
		public UsersManagement UsersManagement => new UsersManagement(this);
		public TeachersManagement TeachersManagement => new TeachersManagement(this);
		public StudentsManagement StudentsManagement => new StudentsManagement(this);
		public EnrollmentsManagement EnrollmentsManagement => new EnrollmentsManagement(this);
		public AcademicSessionsManagement AcademicSessionsManagement => new AcademicSessionsManagement(this);
		public TermsManagement TermsManagement => new TermsManagement(this);
		public GradingPeriodsManagement GradingPeriodsManagement => new GradingPeriodsManagement(this);
		public ClassesManagement ClassesManagement => new ClassesManagement(this);
		public CoursesManagement CoursesManagement => new CoursesManagement(this);

		internal T Execute<T>(RestRequest request) where T : new()
		{
			request.AddHeader("Authorization", "Bearer " + Token.access_token);
			var response = _restClient.Execute<T>(request);
			

			if (response.ErrorException != null)
			{
				const string message = "Error retrieving response.  Check inner details for more info.";
				var exception = new Exception(message, response.ErrorException);
				throw exception;
			}
			return response.Data;
		}

		internal async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
		{
			request.AddHeader("Authorization", "Bearer " + Token.access_token);
				var response = await _restClient.ExecuteAsync<T>(request);

			if (response.ErrorException != null)
			{
				const string message = "Error retrieving response.  Check inner details for more info.";
				var exception = new Exception(message, response.ErrorException);
				throw exception;
			}
			return response.Data;
		}

		internal RestResponse GetResponse(RestRequest request)
		{
			request.AddHeader("Authorization", "Bearer " + Token.access_token);
			var response = _restClient.Execute(request);
			return response;
		}


		internal RestRequest AddRequestParameters(RestRequest r, ApiParameters p)
		{
			if (p == null) return r;

			if (p.Filter != null)
			{
				r.AddParameter("filter", p.Filter);
			}
			if (p.Sort != null)
			{
				r.AddParameter("sort", p.Sort);
			}
			if (p.OrderBy != null)
			{
				r.AddParameter("orderBy", p.OrderBy);
			}
			if (p.Limit != null)
			{
				r.AddParameter("limit", p.Limit.ToString());
			}
			if (p.Offset != null)
			{
				r.AddParameter("offset", p.Offset.ToString());
			}
			if (p.Fields != null)
			{
				r.AddParameter("fields", p.Fields);
			}
			if (p.ExtBasic != null)
			{
				r.AddParameter("ext_basic", p.ExtBasic.Value.ToString());
			}

			return r;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					_restClient = null;
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
