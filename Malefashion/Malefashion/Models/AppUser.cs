using Malefashion.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace Malefashion.Models
{
	public class AppUser:IdentityUser
	{
        public string Name { get; set; }

		public string Surname { get; set; }

        public Gender Gender { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<Order> Orders { get; set; }
        public List<WishList> WishLists { get; set; }
	}
}
