using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using System.Text;

namespace RS1_2024_25.API.Helper
{
    public static class ImageHelper
    {
        public const string Users = "users";
        
        public const string Roles = "roles";

        public const string Products = "products";



        public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
        {
            Users,
            Roles,
            Products



        };


        public static bool isValid(string? type)=> !string.IsNullOrEmpty(type) && All.Contains(type);   

        public static string[] GetAll()=> All.ToArray();

        public static async Task<bool> IsValidAssociation(ApplicationDbContext db, string type, int id)
        {
            switch (type.Trim().ToLower())
            {
                case "users":
                    return await db.AppUsersAll.AnyAsync(u => u.ID == id);
                case "roles":
                    return await db.Roles.AnyAsync(r => r.ID == id);
                case "products":
                    return await db.Products.AnyAsync(r => r.ID == id);


                default:
                    return false;

            }
        }

        public static IFormFile CreateFakeFormFile(string fileName = "test.jpg", string contentType = "image/jpeg", int byteSize = 1024)
        {
            var content = new byte[byteSize];
            new Random().NextBytes(content);

            var stream = new MemoryStream(content);
            return new FormFile(stream,0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

        }
       


    }
}
