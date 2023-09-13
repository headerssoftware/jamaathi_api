using System.ComponentModel.DataAnnotations.Schema;

namespace MWA_API.Filters
{
    public class CommonFilters
    {
        public string? SearchText { get; set; }       
    }

    public class ViewUserMasterFilters: CommonFilters
    {
        public int? osTypeId { get; set; }
        public int? userId { get; set; }
        public string? userName { get; set; }
    }

    public class ViewMasjidMasterFilters : CommonFilters
    {
        public int deviceId { get; set; }
        public string? deviceName { get; set; }
        public int? masjidId { get; set; }
        public string? masjidName { get; set; }
        public string? masjidAddress { get; set; }
        public string? masjidPincode { get; set; }
        public string? masjidMadhab { get; set; }
    }

    public class ViewMasjidWaqthFilters: ViewMasjidMasterFilters
    {
        public int waqthId { get; set; }
        public string? waqthName { get; set; }
    }

    public class ViewUserMasjidFilters: ViewMasjidMasterFilters
    {
         public int? osTypeId { get; set; }
        public int? userId { get; set; }
        public string? userName { get; set; }
    }

    public class ViewUserMasjidWaqthFilters : ViewMasjidWaqthFilters
    {
        public int? osTypeId { get; set; }
        public int? userId { get; set; }
        public string? userName { get; set; }
    }
}
