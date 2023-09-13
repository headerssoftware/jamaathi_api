using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MWA_API.Models
{
    [Table("Vw_MasjidWaqth")]
    public class ViewMasjidWaqth
    {
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

        [Column("waqthId")]
        public int waqthId { get; set; }

        [Column("waqthName")]
        public string waqthName { get; set; }

        [Column("masjidWaqthId")]
        public int masjidWaqthId { get; set; }

        [Column("azanTime")]
        public TimeSpan? azanTime { get; set; }

        [Column("iqaamathTime")]
        public TimeSpan? iqaamathTime { get; set; }

        [Column("startTime")]
        public TimeSpan? startTime { get; set; }

        [Column("endTime")]
        public TimeSpan? endTime { get; set; }


    }

    public class ViewMasjidWaqthMaster
    {
        public int masjidId { get; set; }
        public int masjidWaqthId { get; set; }
        public int waqthId { get; set; }
        public string waqthName { get; set; }
        public TimeSpan? azanTime { get; set; }
        public TimeSpan? iqaamathTime { get; set; }
        public TimeSpan? startTime { get; set; }
        public TimeSpan? endTime { get; set; }
    }

}
