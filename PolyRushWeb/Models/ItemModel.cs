using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Models
{
    public class ItemModel
    {
       
        [Required]
        public int Iditem { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [Range(0, 100000, ErrorMessage = "Please provide a value between 0 and 100000")]
        public int Price { get; set; }

    }
 
}