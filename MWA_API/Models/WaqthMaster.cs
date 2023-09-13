using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("WaqthMaster")]
    public class WaqthMaster
    {
        [Column("waqthId")]
        public int waqthId { get; set; }

        [Column("waqthName")]
        public string waqthName { get; set; }
    }
}
