using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Models
{
    public class DiscountModel
    {
        [Range(1, int.MaxValue)]
        public int ItemId { get; set; }
        [Range(1,100, ErrorMessage ="Please provide a value between 1 and 100")]
        public int DiscountPercentage { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Startdate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Enddate { get; set; }

    }
 
}
