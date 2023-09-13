using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("UserMasjid")]
    public class UserMasjid
    {
        
        [Column("userMasjidId")]
        public int userMasjidId { get; set; }

        [Column("userId")]
        public int userId { get; set; }

        [Column("masjidId")]
        public int masjidId { get; set; }
    }
}
