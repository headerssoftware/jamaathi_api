using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("GetMasjidWithUserSubscribeFlag")]
    public class SpGetMasjidWithUserSubscribeFlag
    {

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

        [Column("userId")]
        public int? userId { get; set; }

        [Column("subscribedFlag")]
        public string? subscribedFlag { get; set; }

        [NotMapped]
        public List<ViewMasjidWaqthMaster>? waqthDetails { get; set; }
    }
}
