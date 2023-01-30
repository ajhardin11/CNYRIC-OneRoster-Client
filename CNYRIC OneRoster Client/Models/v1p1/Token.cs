using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNYRIC_OneRosterClient
{
	public class Token
	{
		public Token()
		{
			CreatedAt = DateTime.Now;
		}
		public string access_token { get; set; }
		public string error_description { get; set; }
		public string error { get; set; }
		public int expires_in { get; set; }
		public string token_type { get; set; }
		public string scope { get; set; }
		public DateTime CreatedAt { get; }

		public bool NotExpired()
		{
			long elapsedTicks = DateTime.Now.Ticks - CreatedAt.Ticks;
			var elapsedSpan = new TimeSpan(elapsedTicks);
			return elapsedSpan.Seconds < Convert.ToInt32(expires_in);
		}
	}
}
