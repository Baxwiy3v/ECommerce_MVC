using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Department:BaseNameableEntity
    {
        public List<Team>? Teams { get; set; }
    }
}
