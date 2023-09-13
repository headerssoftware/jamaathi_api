﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Models
{
    [Table("Vw_MasjidMaster")]
    public class ViewMasjidMaster
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
    }
}
