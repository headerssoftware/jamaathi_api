using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("MasjidWaqth")]
    public class MasjidWaqth
    {
        [Column("masjidWaqthId")]
        public int masjidWaqthId { get; set; }

        [Column("masjidId")]
        public int masjidId { get; set; }

        [Column("waqthId")]
        public int waqthId { get; set; }

        [Column("azanTime")]
        public TimeSpan? azanTime { get; set; }

        [Column("iqaamathTime")]
        public TimeSpan? iqaamathTime { get; set; }

        [Column("startTime")]
        public TimeSpan? startTime { get; set; }

        [Column("endTime")]
        public TimeSpan? endTime { get; set; }
    }
}
