using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("Vw_UserMasjid")]
    public class ViewUserMasjid 
    {
        [Column("userId")]
        public int userId { get; set; }

        [Column("userName")]
        public string userName { get; set; }

        [Column("userFcmId")]
        public string userFcmId { get; set; }

        [Column("userLastLogin")]
        public DateTime? userLastLogin { get; set; }

        [Column("osVersion")]
        public string? osVersion { get; set; }

        [Column("osTypeId")]
        public int osTypeId { get; set; }

        [Column("osTypeName")]
        public string osTypeName { get; set; }

        [Column("userMasjidId")]
        public int userMasjidId { get; set; }

        [Column("deviceId")]
        public int deviceId { get; set; }

        [Column("deviceName")]
        public string deviceName { get; set; }

        [Column("masjidDeviceId")]
        public int masjidDeviceId { get; set; }

        [Column("masjidId")]
        public int masjidId { get; set; }

        [Column("masjidName")]
        public string masjidName { get; set; }

        [Column("masjidLocation")]
        public string? masjidLocation { get; set; }

        [Column("masjidAddress")]
        public string? masjidAddress { get; set; }

        [Column("masjidPincode")]
        public string? masjidPincode { get; set; }

        [Column("masjidMadhab")]
        public string? masjidMadhab { get; set; }

        [Column("masjidLastUpdatedTime")]
        public DateTime? masjidLastUpdatedTime { get; set; }

        //[JsonIgnore]
        [Column("masjidImagePath")]
        public string? masjidImagePath { get; set; }

        [NotMapped]
        public string? masjidImageURL { get; set; }
    }

   

}
