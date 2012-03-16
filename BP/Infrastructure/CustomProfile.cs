using System.Web.Profile;
using System;

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

        public Guid OrganizationId
        {
            get
            {
                return ((Guid)base["OrganizationId"]);
            }
            set { base["OrganizationId"] = value; Save(); }
        }

        public Guid BikePlanApplicationId
        {
            get
            {
                return ((Guid)base["BikePlanApplicationId"]);
            }
            set { base["BikePlanApplicationId"] = value; Save(); }
        }

        public string Company
        {
            get
            {
                return ((string)base["Company"]);
            }
            set { base["Company"] = value; Save(); }
        }

        public string[] Permissions
        {
            get
            {
                return ((string[])base["Permissions"]);
            }
            set { base["Permissions"] = value; Save(); }
        }

        public static CustomProfile GetProfile(string username)
        {
            return Create(username) as CustomProfile;
        }
    }
}