using RS1_2024_25.API.Helper.BaseClasses;

namespace RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic
{
    public class Image : TenantSpecificTable
    {

        public string Name { get; set; } = null!;

        public int ImageableId { get; set; }
        
        public string ImageableType { get; set; }

        public string? FilePath { get; set; }

        public string? Url { get; set; }

        public bool isExternal => !string.IsNullOrEmpty(Url);
    }
}
