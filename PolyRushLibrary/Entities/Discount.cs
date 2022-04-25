using System.ComponentModel.DataAnnotations;

namespace PolyRushLibrary
{
    public partial class Discount
    {

        [Key] public int Iddiscount;
        public int ItemId;
        public int DiscountPercentage;
        public DateTime Startdate;
        public DateTime Enddate;
    }
}
