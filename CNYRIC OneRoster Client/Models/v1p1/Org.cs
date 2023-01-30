
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
	public class Org
	{
		public string SourcedId { get; set; }

		public string Status { get; set; } = "";

		public DateTimeOffset DateLastModified { get; set; } = DateTimeOffset.Now;

		public Dictionary<string, string> Metadata { get; set; } =new Dictionary<string, string>();

		public string Name { get; set; } = "";

		public string Identifier { get; set; } = "";

		public List<GuidRef> Children { get; set; } = new List<GuidRef>();

		public GuidRef Parent { get; set; } = new GuidRef();

		public string Type { get; set; } = "";

		public static void CsvHeader(CsvWriter writer)
		{
			
			writer.WriteField("sourcedId", true);
			writer.WriteField("status", true);
			writer.WriteField("dateLastModified", true);

			writer.WriteField("name", true);
			writer.WriteField("type", true);
			writer.WriteField("identifier", true);
			writer.WriteField("parentSourcedId", true);

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


			writer.WriteField(Name, true);
			writer.WriteField(Type, true);
			writer.WriteField(Identifier, true);
			writer.WriteField(Parent.SourcedId, true);

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

	public class Orgs
	{
		public List<Org> orgs { get; set; }
	}

	public class SingleOrg
	{
		public Org org { get; set; }
	}

	public class OrgsManagement
	{
		private readonly V1p1 _oneRosterApi;
		private RestRequest _request;

		public OrgsManagement(V1p1 oneRosterApi)
		{
			_request = new RestRequest();
			_oneRosterApi = oneRosterApi;
		}

		/// <summary>
		/// To read, get, a collection of orgs, district and schools. Refers to campus districts and schools. status is always active.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public Orgs GetAllOrgs(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/orgs/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<Orgs>(_request);
		}

		public RestResponse GetAllOrgsRaw(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/orgs/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<Orgs> GetAllOrgsAsync(ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/orgs/";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<Orgs>(_request);
		}

		/// <summary>
		/// To read, get, one organization, district or school by sourcedId
		/// </summary>
		/// <param name="sourcedId"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public SingleOrg GetOrg(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/orgs/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.Execute<SingleOrg>(_request);
		}

		//will need to change the URL ahead of calling
		public SingleOrg GetOrg(string fullUrl)
		{
			_request.Method = Method.Get;
			_oneRosterApi.AddRequestParameters(_request, null);
			return _oneRosterApi.Execute<SingleOrg>(_request);
		}
		public RestResponse GetOrgRaw(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/orgs/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return _oneRosterApi.GetResponse(_request);
		}
		public async Task<SingleOrg> GetOrgAsync(string sourcedId, ApiParameters? p = null)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/orgs/{sourcedId}";
			_oneRosterApi.AddRequestParameters(_request, p);
			return await _oneRosterApi.ExecuteAsync<SingleOrg>(_request);
		}

		//straight csv from APICall
		public string GetOrgsCsv(ApiParameters? p = null, bool includeHeader=true)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/orgs/";
			_oneRosterApi.AddRequestParameters(_request, p);
			Orgs Records= _oneRosterApi.Execute<Orgs>(_request);

			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if(includeHeader)Org.CsvHeader(csv);
				foreach (Org Record in Records.orgs)
				{
					Record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}

		//Make csv from object
		public string GetOrgsCsv(Orgs Records, bool includeHeader=true)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if(includeHeader)Org.CsvHeader(csv);

				foreach (Org record in Records.orgs)
				{
					record.AsCsvRow(csv);
				}
				return sb.ToString();
			}
		}


	}

}
