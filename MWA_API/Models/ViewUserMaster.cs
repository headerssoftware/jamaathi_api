using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("Vw_UserMaster")]
    public class ViewUserMaster
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

        [Column("osTypeName")]
        public string? osTypeName { get; set; }

        [Column("osVersion")]
        public string? osVersion { get; set; }


    }
}
