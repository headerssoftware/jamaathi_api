using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("MasjidDevice")]
    public class MasjidDevice
    {        
        [Column("masjidDeviceId")]
        public int masjidDeviceId { get; set; }

        [Column("deviceId")]
        public int deviceId { get; set; }

        [Column("masjidId")]
        public int masjidId { get; set; }
    }
}
