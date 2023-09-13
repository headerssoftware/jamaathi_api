using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MWA_API.Models;

namespace MWA_API.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        //Tables
        public DbSet<OsType> osTypes { get; set; }
        public DbSet<UserMaster> userMasters { get; set; }
        public DbSet<DeviceMaster> deviceMasters { get; set; }
        public DbSet<MasjidMaster> masjidMasters { get; set; }
        public DbSet<MasjidDevice> masjidDevices { get; set; }
        public DbSet<UserMasjid> userMasjids { get; set; }
        public DbSet<WaqthMaster> waqthMasters { get; set; }
        public DbSet<MasjidWaqth> masjidWaqths { get; set; }

        //Views
        public DbSet<ViewUserMaster> viewUserMasters { get; set; }
        public DbSet<ViewMasjidMaster> ViewMasjidMasters { get; set; }
        public DbSet<ViewMasjidWaqth> viewMasjidWaqths { get; set; }
        public DbSet<ViewUserMasjid> viewUserMasjids { get; set; }
        public DbSet<ViewUserMasjidWaqth> viewUserMasjidWaqths { get; set; }
        
        // Stored Procedure
        public DbSet<SpGetMasjidWithUserSubscribeFlag> spGetMasjidWithUserSubscribeFlags { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OsType>().HasKey(e => new { e.osTypeId });

            modelBuilder.Entity<UserMaster>().HasKey(e => new { e.userId });

            modelBuilder.Entity<DeviceMaster>().HasKey(e => new { e.deviceId });

            modelBuilder.Entity<MasjidMaster>().HasKey(e => new { e.masjidId });

            modelBuilder.Entity<MasjidDevice>().HasKey(e => new { e.masjidDeviceId });
            modelBuilder.Entity<MasjidDevice>().HasIndex(new String[] { "deviceId", "masjidId"}).IsUnique(true);

            modelBuilder.Entity<UserMasjid>().HasKey(e => new { e.userMasjidId });
            modelBuilder.Entity<UserMasjid>().HasIndex(new String[] { "userId", "masjidId" }).IsUnique(true);

            modelBuilder.Entity<WaqthMaster>().HasKey(e => new { e.waqthId });

            modelBuilder.Entity<MasjidWaqth>().HasKey(e => new { e.masjidWaqthId });
            modelBuilder.Entity<MasjidWaqth>().HasIndex(new String[] { "masjidId", "waqthId" }).IsUnique(true);


            //Views
            modelBuilder.Entity<ViewUserMaster>().HasNoKey();
            modelBuilder.Entity<ViewMasjidMaster>().HasNoKey();
            modelBuilder.Entity<ViewMasjidWaqth>().HasNoKey();
            modelBuilder.Entity<ViewUserMasjid>().HasNoKey();
            modelBuilder.Entity<ViewUserMasjidWaqth>().HasNoKey();
            
            // Stored Procedure
            modelBuilder.Entity<SpGetMasjidWithUserSubscribeFlag>().HasNoKey();
        }

    }
}
