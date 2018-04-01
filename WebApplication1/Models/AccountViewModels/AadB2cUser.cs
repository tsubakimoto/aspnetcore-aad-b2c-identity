namespace WebApplication1.Models.AccountViewModels
{
    public class AadB2cUser
    {
        public bool accountEnabled { get; set; } = true;
        public Signinname[] signInNames { get; set; }
        public string creationType { get; set; } = "LocalAccount";
        public string displayName { get; set; } = "";
        public string mailNickname { get; set; } = "";
        public Passwordprofile passwordProfile { get; set; }
        public string passwordPolicies { get; set; } = "DisablePasswordExpiration";
        public string city { get; set; } = "";
        public object country { get; set; } = null;
        public object facsimileTelephoneNumber { get; set; } = null;
        public string givenName { get; set; } = "";
        public object mail { get; set; } = null;
        public object mobile { get; set; } = null;
        public object[] otherMails { get; set; } = new object[] { };
        public string postalCode { get; set; } = "";
        public object preferredLanguage { get; set; } = null;
        public string state { get; set; } = "";
        public object streetAddress { get; set; } = null;
        public string surname { get; set; } = "";
        public object telephoneNumber { get; set; } = null;
    }

    public class Passwordprofile
    {
        public string password { get; set; } = "";
        public bool forceChangePasswordNextLogin { get; set; } = false;
    }

    public class Signinname
    {
        public string type { get; set; } = "emailAddress";
        public string value { get; set; } = "";
    }
}
