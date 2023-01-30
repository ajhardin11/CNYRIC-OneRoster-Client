
//using static CNYRIC_OneRosterClient.Models.SharedVocabulary;

namespace CNYRIC_OneRosterClient
{
    public class UserRole
    {
			public string Role { get; set; } = "";

      public GuidRef Org { get; set; }=new GuidRef();

			public string RoleType { get; set; } = "";
    }

   
}