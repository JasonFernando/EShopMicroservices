using Marten.Schema;

namespace Catalog.API.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();

            if (await session.Query<Product>().AnyAsync())
                return;

            session.Store<Product>(GetPreconfiguredProducts());
            await session.SaveChangesAsync();
        }

        private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
        {
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "iPhone 15 Pro",
                Description = "Titanium design, A17 Pro chip, versatile 48MP Main camera.",
                ImageUrl = "iphone-15-pro.png",
                Price = 999.99M,
                Category = new List<string> { "Smart Phone" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Samsung Galaxy S24 Ultra",
                Description = "AI-powered smartphone with 200MP camera and S Pen.",
                ImageUrl = "s24-ultra.png",
                Price = 1299.00M,
                Category = new List<string> { "Smart Phone" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Google Pixel 8",
                Description = "The most advanced Pixel yet, with a focus on AI photography.",
                ImageUrl = "pixel-8.png",
                Price = 699.00M,
                Category = new List<string> { "Smart Phone" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Xiaomi 14",
                Description = "Leica optics and Snapdragon 8 Gen 3 for flagship performance.",
                ImageUrl = "xiaomi-14.png",
                Price = 799.50M,
                Category = new List<string> { "Smart Phone" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "OnePlus 12",
                Description = "Smooth Beyond Belief with 120Hz ProXDR display.",
                ImageUrl = "oneplus-12.png",
                Price = 899.00M,
                Category = new List<string> { "Smart Phone" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Asus Zenfone 10",
                Description = "Compact size, big performance with flagship specs.",
                ImageUrl = "zenfone-10.png",
                Price = 649.99M,
                Category = new List<string> { "Smart Phone" }
            },

            // --- LAPTOPS ---
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "MacBook Air M3",
                Description = "Supercharged by M3, strikingly thin and fast.",
                ImageUrl = "macbook-air.png",
                Price = 1099.00M,
                Category = new List<string> { "Laptops" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Dell XPS 13",
                Description = "Iconic design, thin and light for premium portability.",
                ImageUrl = "dell-xps.png",
                Price = 950.00M,
                Category = new List<string> { "Laptops" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "HP Spectre x360",
                Description = "Versatile 2-in-1 laptop with stunning OLED display.",
                ImageUrl = "hp-spectre.png",
                Price = 1350.00M,
                Category = new List<string> { "Laptops" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Lenovo ThinkPad X1 Carbon",
                Description = "The gold standard for business productivity and durability.",
                ImageUrl = "thinkpad-x1.png",
                Price = 1420.00M,
                Category = new List<string> { "Laptops" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "ASUS ROG Zephyrus G14",
                Description = "Powerful gaming laptop in a compact 14-inch chassis.",
                ImageUrl = "rog-g14.png",
                Price = 1599.99M,
                Category = new List<string> { "Laptops" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Razer Blade 15",
                Description = "Ultra-fast 240Hz display and NVIDIA RTX graphics.",
                ImageUrl = "razer-blade.png",
                Price = 2199.00M,
                Category = new List<string> { "Laptops" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Microsoft Surface Laptop 5",
                Description = "Sleek and elegant with a vibrant touchscreen.",
                ImageUrl = "surface-laptop.png",
                Price = 899.00M,
                Category = new List<string> { "Laptops" }
            },

            // --- ACCESSORIES ---
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "AirPods Pro (2nd Gen)",
                Description = "Active Noise Cancellation and personalized Spatial Audio.",
                ImageUrl = "airpods-pro.png",
                Price = 249.00M,
                Category = new List<string> { "Accessories" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Logitech MX Master 3S",
                Description = "Performance wireless mouse with quiet clicks.",
                ImageUrl = "mx-master.png",
                Price = 99.00M,
                Category = new List<string> { "Accessories" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Keychron K2 Keyboard",
                Description = "Wireless mechanical keyboard with tactile switches.",
                ImageUrl = "keychron-k2.png",
                Price = 79.99M,
                Category = new List<string> { "Accessories" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Sony WH-1000XM5",
                Description = "Industry-leading noise canceling headphones.",
                ImageUrl = "sony-xm5.png",
                Price = 348.00M,
                Category = new List<string> { "Accessories" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Samsung T7 Shield 1TB",
                Description = "Rugged external SSD with high-speed transfers.",
                ImageUrl = "samsung-t7.png",
                Price = 120.00M,
                Category = new List<string> { "Accessories" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Anker 737 Power Bank",
                Description = "Ultra-powerful 140W charging for laptops and phones.",
                ImageUrl = "anker-737.png",
                Price = 149.99M,
                Category = new List<string> { "Accessories" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "SteelSeries Arctis Nova 7",
                Description = "Multi-platform gaming headset with high-fidelity audio.",
                ImageUrl = "arctis-nova.png",
                Price = 179.99M,
                Category = new List<string> { "Accessories" }
            }
        };

    }


}
