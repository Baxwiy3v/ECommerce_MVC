using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Team:BaseNameableEntity
    {

        public string ImageUrl { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
