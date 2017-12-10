using System.Collections.Generic;
using System.Security.Claims;

namespace SAM.Models.Auth
{
    public class User
    {
        public User(IEnumerable<Claim> claims)
        {
            Roles = new List<string>();

            foreach (var c in claims)
            {
                switch (c.Type)
                {
                    case "given_name":
                        FirstName = c.Value;
                        break;
                    case "family_name":
                        LastName = c.Value;
                        break;
                    case "nickname":
                        Nickname = c.Value;
                        break;
                    case "name":
                        Name = c.Value;
                        break;
                    case "email":
                        Email = c.Value;
                        break;
                    case "http://llc.idla.us/claims/roles":
                        Roles.Add(c.Value);
                        break;
                }
            }
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Nickname { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public bool ContainsRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}
