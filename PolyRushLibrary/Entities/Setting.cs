using System.ComponentModel.DataAnnotations;

namespace PolyRushLibrary
{
    public partial class Setting
    {
        [Key]
        public int Idsetting ;
        public string Name = null!;
        public string Description = null!;
    }
}
