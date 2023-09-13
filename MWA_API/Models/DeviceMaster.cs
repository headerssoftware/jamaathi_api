using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("DeviceMaster")]
    public class DeviceMaster
    {
        [Column("deviceId")]
        public int deviceId { get; set; }

        [Column("deviceName")]
        public string deviceName { get; set; }
    }
}
