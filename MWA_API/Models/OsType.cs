using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("osType")]
    public class OsType
    {
        [Required]
        [Column("osTypeId")]
        public int osTypeId { get; set; }

        [Required]
        [Column("osTypeName")]
        public string osTypeName { get; set; }
    }
}
