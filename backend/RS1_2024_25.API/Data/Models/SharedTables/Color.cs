using RS1_2024_25.API.Helper.BaseClasses;

namespace RS1_2024_25.API.Data.Models.SharedTables
{
    public class Color : SharedTableBase
    {
        public string Name { get; set; }  // Naziv boje
        public string Hex_Code { get; set; }
    }
}
