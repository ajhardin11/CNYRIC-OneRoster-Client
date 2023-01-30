using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CNYRIC_OneRosterClient
{
	internal class Oauth2
	{
		private readonly RestClient _client;
		private readonly RestRequest _request;
		

		internal Oauth2(string tokenUrl)
		{
			_client = new RestClient(tokenUrl);
			_request = new RestRequest();
		}

		internal Token GetToken(string clientId, string clientSecret)
		{

			_client.Authenticator= new HttpBasicAuthenticator(clientId, clientSecret);
			_request.Method = Method.Post;
			_request.AddParameter("grant_type", "client_credentials");
				var response = _client.Execute<Token>(_request);

			if (response == null)
			{
				return new Token() { error = "Could Not Complete Token Request.The Response was empty." + _request.Resource };
			}

			var token = new Token();
			if (response.Data != null)
			{
				token = response.Data;
			}
			else if(response.Content!=null)
			{
				token= JsonSerializer.Deserialize<Token>(response.Content);
			}

			if (response.ErrorException != null)
			{
				return new Token() { error = "Error Obtaining Token", error_description = response.ErrorException.ToString() };
			}

			switch (response.StatusCode)
			{
				case HttpStatusCode.OK:
					break;
				case HttpStatusCode.NotFound:
				case 0:
					token.error = "Could Not Contact Token URL";
					break;
				case HttpStatusCode.BadRequest:
					token.error = "Bad Request";
					token.error_description = response.Content;
					break;
				default:
					token.error_description = "Unknown Error Obtaining Token";
					break;
			}

			return token;
		}
	}
}
