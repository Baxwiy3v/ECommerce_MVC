using Microsoft.AspNetCore.Identity;

namespace Malefashion.Models
{
	public class AppUser:IdentityUser
	{
        public string Name { get; set; }

		public string Surname { get; set; }
	}
}
