using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MWA_API.Models
{
    [Table("MasjidMaster")]
    public class MasjidMaster
    {
        [Required]
        [Column("masjidId")]
        public int masjidId { get; set; }

        [Required]
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

    public class FilePost
    {
        public IFormFile? file { get; set; }
    }
}
