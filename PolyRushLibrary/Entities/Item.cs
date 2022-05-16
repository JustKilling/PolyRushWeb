using System.ComponentModel.DataAnnotations;

namespace PolyRushLibrary
{
    public partial class Item
    {
        [Key]
        public int Iditem ;
        public int ItemTypeId ;
        public string Name = null!;
        public int Price ;
    }
}
