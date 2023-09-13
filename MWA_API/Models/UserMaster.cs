using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("UserMaster")]
    public class UserMaster
    {
        [Column("userId")]
        public int userId { get; set; }

        [Column("userName")]
        public string userName { get; set; }

        [Column("userFcmId")]
        public string userFcmId { get; set; }

        [Column("userLastLogin")]
        public DateTime? userLastLogin { get; set; }

        [Column("osTypeId")]
        public int osTypeId { get; set; }

        [Column("osVersion")]
        public string? osVersion { get; set; }
    }
}
