using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Setting:BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
