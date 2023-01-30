using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNYRIC_OneRosterClient
{
	public class ApiParameters
	{
		/// <summary>
		/// The number of results to return
		/// </summary>
		public int? Limit { get; set; }

		/// <summary>
		/// Data fields to return
		/// </summary>
		public string? Fields { get; set; }

		/// <summary>
		/// The index of the first record to return
		/// </summary>
		public int? Offset { get; set; }

		/// <summary>
		/// Data field to sort on
		/// </summary>
		public string? Sort { get; set; }

		/// <summary>
		/// asc or desc
		/// </summary>
		public string? OrderBy { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string? Filter { get; set; }

		/// <summary>
		/// Set to False to omit loading child data
		/// </summary>
		public bool? ExtBasic { get; set; }

	}
}
