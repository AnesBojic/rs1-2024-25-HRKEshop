using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using static RS1_2024_25.API.Endpoints.CartEndpoints.CartGetEndpoint;

namespace RS1_2024_25.API.Endpoints.CartEndpoints
{
    public static class CartQueryHelper
    {
        public static async Task<List<CartItemDto>> GetCartItemsAsync(
            ApplicationDbContext db,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await db.CartItems
                .Where(c => c.AppUserId == userId)
                .Include(c => c.ProductSize)
                    .ThenInclude(ps => ps.Product)
                .Include(c => c.ProductSize)
                    .ThenInclude(ps => ps.Size)
                .OrderByDescending(c => c.UpdatedAt)
                .Select(c => new CartItemDto
                {
                    Id = c.ID,
                    ProductId = c.ProductSize.ProductId,
                    ProductSizeId = c.ProductSizeId,
                    ProductName = c.ProductSize.Product.Name,
                    SizeName = c.ProductSize.Size.Value,
                    Quantity = c.Quantity,
                    UnitPrice = c.ProductSize.Price ?? (decimal)c.ProductSize.Product.Price,
                    LineTotal = c.Quantity * (c.ProductSize.Price ?? (decimal)c.ProductSize.Product.Price),
                    Stock = c.ProductSize.Stock,
                    ImageUrl = db.ImagesAll
                        .Where(img => img.ImageableId == c.ProductSize.ProductId && img.ImageableType.ToLower() == "products")
                        .OrderByDescending(img => img.UpdatedAt)
                        .Select(img => img.Url)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }

        public static CartResponse ToCartResponse(List<CartItemDto> items)
        {
            return new CartResponse
            {
                Items = items,
                TotalQuantity = items.Sum(i => i.Quantity),
                TotalAmount = items.Sum(i => i.LineTotal)
            };
        }
    }
}
