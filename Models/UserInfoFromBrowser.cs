#nullable enable
using System;
namespace AMS.Models
{
    public class UserInfoFromBrowser : EntityBase
    {
        public Int64 Id { get; set; }
        public string? BrowserUniqueID { get; set; }
        public string? Lat { get; set; }
        public string? Long { get; set; }
        public string TimeZone { get; set; } = string.Empty;
        public string BrowserMajor { get; set; } = string.Empty;
        public string BrowserName { get; set; } = string.Empty;
        public string BrowserVersion { get; set; } = string.Empty;
        public string? CPUArchitecture { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceVendor { get; set; }
        public string? EngineName { get; set; }
        public string? EngineVersion { get; set; }
        public string? OSName { get; set; }
        public string? OSVersion { get; set; }
        public string? UA { get; set; }
    }
}
