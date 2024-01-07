using IoTFeeder.Common.Helpers;
using System;

namespace IoTFeeder.Common.Models
{
    public class DropDownBindViewModel
    {
        public Int64 value { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        /// <summary>
        /// Encrypted value
        /// </summary>
        public string EncValue { get { return Encryption.Encrypt(value.ToString()); } }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public decimal? AverageRating { get; set; }
    }
}