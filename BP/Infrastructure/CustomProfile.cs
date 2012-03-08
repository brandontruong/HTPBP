using System.Web.Profile;

namespace BP.Infrastructure
{
    public class CustomProfile: ProfileBase
    {
        public string Role
        {
            get { 
                return ((string) base["Role"]);
            }
            set { base["Role"] = value; Save(); }
        }

        public string FamilyName
        {
            get
            {
                return ((string)base["FamilyName"]);
            }
            set { base["FamilyName"] = value; Save(); }
        }

        public string GivenName
        {
            get
            {
                return ((string)base["GivenName"]);
            }
            set { base["GivenName"] = value; Save();}
        }

        public string Organization
        {
            get
            {
                return ((string)base["Organization"]);
            }
            set { base["Organization"] = value; Save();}
        }

        public static CustomProfile GetProfile(string username)
        {
            return Create(username) as CustomProfile;
        }
    }
}