using System.ComponentModel.DataAnnotations;

namespace PolyRushLibrary
{
    public partial class Useritem
    {
        [Key]
        public int IduserItem ;
        public int UserId ;
        public int ItemId ;
        public int Amount ;
    }
}
