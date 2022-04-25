using System.ComponentModel.DataAnnotations;

namespace PolyRushLibrary
{
    public partial class Itemtype
    {
        [Key]
        public int IditemType ;
        public string Name = null!;
    }
}
