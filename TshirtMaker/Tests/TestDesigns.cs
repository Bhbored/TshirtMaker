using TshirtMaker.Models.Core;
using TshirtMaker.Models.Enums;

namespace TshirtMaker.Tests
{
    public static class TestDesigns
    {

        public static readonly List<Design> Testdesigns = [

    // Design 1: Astronaut T-shirt
    new Design
    {
        Id = Guid.NewGuid(),
        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
        Prompt = "An astronaut floating in space with colorful nebulae, digital art style",
        NegativePrompt = "blurry, low quality, text, watermark",
        Title = "Space Explorer T-Shirt",
        StylePreset = StylePresetType.PixelArt,
        Resolution = Resolution.FourK,
        ClothingType = ClothingType.TShirt,
        Color = "#000000",
        Size = ClothingSize.M,
        Material = Material.HeavyCotton,
        PrintPosition = PrintPosition.Front,
        FinalImageUrl = "https://fbtaluaxtwvxfdnflxoq.supabase.co/storage/v1/object/sign/Design%20images/2b8b3e04-1ccf-490d-8a10-8d806e27f2a9/final/11.png?token=eyJraWQiOiJzdG9yYWdlLXVybC1zaWduaW5nLWtleV8wZjllMzgzNS0yOWYzLTQzNjMtODFjYS0wMGM0YmZiMjVlZjMiLCJhbGciOiJIUzI1NiJ9.eyJ1cmwiOiJEZXNpZ24gaW1hZ2VzLzJiOGIzZTA0LTFjY2YtNDkwZC04YTEwLThkODA2ZTI3ZjJhOS9maW5hbC8xMS5wbmciLCJpYXQiOjE3NjkxNzAwMDIsImV4cCI6MTgwMDcwNjAwMn0.fUG3d3EGIye5jDQYkrhw6am0bHcM61_KHpvVPYXBybc"
    },

    // Design 2: Vintage Band Hoodie
    new Design
    {
        Id = Guid.NewGuid(),
        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
        Prompt = "Retro 80s rock band logo with neon lights and grunge texture",
        NegativePrompt = "modern, clean, minimalist",
        Title = "Neon Rock Hoodie",
        StylePreset = StylePresetType.Vintage,
        Resolution = Resolution.Standard,
        ClothingType = ClothingType.Hoodie,
        Color = "#2C3E50",
        Size = ClothingSize.L,
        Material = Material.Fleece,
        PrintPosition = PrintPosition.Front,
        FinalImageUrl = "https://fbtaluaxtwvxfdnflxoq.supabase.co/storage/v1/object/sign/Design%20images/2b8b3e04-1ccf-490d-8a10-8d806e27f2a9/final/2.png?token=eyJraWQiOiJzdG9yYWdlLXVybC1zaWduaW5nLWtleV8wZjllMzgzNS0yOWYzLTQzNjMtODFjYS0wMGM0YmZiMjVlZjMiLCJhbGciOiJIUzI1NiJ9.eyJ1cmwiOiJEZXNpZ24gaW1hZ2VzLzJiOGIzZTA0LTFjY2YtNDkwZC04YTEwLThkODA2ZTI3ZjJhOS9maW5hbC8yLnBuZyIsImlhdCI6MTc2OTE3MDAxOSwiZXhwIjoxODAwNzA2MDE5fQ.7LF1XuVN_tohVxDnL0ZtMWbl8mFBqfDKkV4Y99g7RWY"
    },

    // Design 3: Nature Tank Top
    new Design
    {
        Id = Guid.NewGuid(),
        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
        Prompt = "Majestic wolf in forest with moonlight, watercolor painting style",
        NegativePrompt = "cartoon, pixelated, dark",
        Title = "Moonlight Wolf Tank",
        StylePreset = StylePresetType.Watercolor,
        Resolution = Resolution.Standard,
        ClothingType = ClothingType.Tank,
        Color = "#1A5276",
        Size = ClothingSize.S,
        Material = Material.Polyester,
        PrintPosition = PrintPosition.Front,
        FinalImageUrl = "https://fbtaluaxtwvxfdnflxoq.supabase.co/storage/v1/object/sign/Design%20images/2b8b3e04-1ccf-490d-8a10-8d806e27f2a9/final/12.png?token=eyJraWQiOiJzdG9yYWdlLXVybC1zaWduaW5nLWtleV8wZjllMzgzNS0yOWYzLTQzNjMtODFjYS0wMGM0YmZiMjVlZjMiLCJhbGciOiJIUzI1NiJ9.eyJ1cmwiOiJEZXNpZ24gaW1hZ2VzLzJiOGIzZTA0LTFjY2YtNDkwZC04YTEwLThkODA2ZTI3ZjJhOS9maW5hbC8xMi5wbmciLCJpYXQiOjE3NjkxNzAyNDYsImV4cCI6MTgwMDcwNjI0Nn0.F1CuifL8mTKp41bmBtxmnueVHh692_wMp9SJvOACCls"
    },

    // Design 4: Geometric Pattern Long Sleeve
    new Design
    {
        Id = Guid.NewGuid(),
        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
        Prompt = "Minimalist geometric patterns with triangles and circles, monochrome",
        NegativePrompt = "complex, colorful, busy",
        Title = "Geometric Long Sleeve",
        StylePreset = StylePresetType.PixelArt,
        Resolution = Resolution.TwoK,
        ClothingType = ClothingType.LongSleeve,
        Color = "#FFFFFF",
        Size = ClothingSize.XL,
        Material = Material.CottonPolyesterBlend,
        PrintPosition = PrintPosition.Front,
        FinalImageUrl = "https://fbtaluaxtwvxfdnflxoq.supabase.co/storage/v1/object/sign/Design%20images/2b8b3e04-1ccf-490d-8a10-8d806e27f2a9/final/13.png?token=eyJraWQiOiJzdG9yYWdlLXVybC1zaWduaW5nLWtleV8wZjllMzgzNS0yOWYzLTQzNjMtODFjYS0wMGM0YmZiMjVlZjMiLCJhbGciOiJIUzI1NiJ9.eyJ1cmwiOiJEZXNpZ24gaW1hZ2VzLzJiOGIzZTA0LTFjY2YtNDkwZC04YTEwLThkODA2ZTI3ZjJhOS9maW5hbC8xMy5wbmciLCJpYXQiOjE3NjkxNzA0NjQsImV4cCI6MTgwMDcwNjQ2NH0.ZilAUyctKoWz62cusfsLuFs1_EKGTEgTbHVYCGQdufg"
    },

    // Design 5: Ocean Theme Sweatshirt
    new Design
    {
        Id = Guid.NewGuid(),
        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
        Prompt = "Underwater scene with sea turtles and coral reefs, vibrant colors",
        NegativePrompt = "dark, murky, sharks",
        Title = "Ocean Life Sweatshirt",
        StylePreset = StylePresetType.Abstract,
        Resolution = Resolution.TwoK,
        ClothingType = ClothingType.Sweat,
        Color = "#5DADE2",
        Size = ClothingSize.M,
        Material = Material.CottonPolyesterBlend,
        PrintPosition = PrintPosition.Front,
        FinalImageUrl = "https://fbtaluaxtwvxfdnflxoq.supabase.co/storage/v1/object/sign/Design%20images/2b8b3e04-1ccf-490d-8a10-8d806e27f2a9/final/14.png?token=eyJraWQiOiJzdG9yYWdlLXVybC1zaWduaW5nLWtleV8wZjllMzgzNS0yOWYzLTQzNjMtODFjYS0wMGM0YmZiMjVlZjMiLCJhbGciOiJIUzI1NiJ9.eyJ1cmwiOiJEZXNpZ24gaW1hZ2VzLzJiOGIzZTA0LTFjY2YtNDkwZC04YTEwLThkODA2ZTI3ZjJhOS9maW5hbC8xNC5wbmciLCJpYXQiOjE3NjkxNzA3MTEsImV4cCI6MTgwMDcwNjcxMX0.Kq6Fw3KJwWbm9HpmkWwZ80-zxhMkwP373v0UOIMJIlQ"
    },


            ];
    }
}



//// Design 6: Cyberpunk Pullover
//new Design
//{
//    Id = Guid.NewGuid(),
//    UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
//    Prompt = "Cyberpunk cityscape with neon signs and flying cars at night",
//    NegativePrompt = "daytime, natural, rustic",
//    Title = "Neon City Pullover",
//    StylePreset = StylePresetType.Cyberpunk,
//    Resolution = Resolution.Ultra,
//    ClothingType = ClothingType.Pullover,
//    Color = "#17202A",
//    Size = ClothingSize.L,
//    Material = Material.Polyester,
//    PrintPosition = PrintPosition.Back,
//    FinalImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80"
//},

//    // Design 7: Cat Lover T-shirt
//    new Design
//    {
//        Id = Guid.NewGuid(),
//        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
//        Prompt = "Cute cartoon cats in various poses with rainbow colors",
//        NegativePrompt = "realistic, scary, dogs",
//        Title = "Rainbow Cats T-Shirt",
//        StylePreset = StylePresetType.Cartoon,
//        Resolution = Resolution.Standard,
//        ClothingType = ClothingType.TShirt,
//        Color = "#FADBD8",
//        Size = ClothingSize.XS,
//        Material = Material.OrganicCotton,
//        PrintPosition = PrintPosition.Front,
//        FinalImageUrl = "https://images.unsplash.com/photo-1583795128727-6ec3642408f8?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80"
//    },

//    // Design 8: Vintage Motorcycle Hoodie
//    new Design
//    {
//        Id = Guid.NewGuid(),
//        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
//        Prompt = "Classic vintage motorcycle with flames, retro poster style",
//        NegativePrompt = "modern, electric, clean",
//        Title = "Vintage Bike Hoodie",
//        StylePreset = StylePresetType.Vintage,
//        Resolution = Resolution.High,
//        ClothingType = ClothingType.Hoodie,
//        Color = "#BA4A00",
//        Size = ClothingSize.XL,
//        Material = Material.Fleece,
//        PrintPosition = PrintPosition.Front,
//        FinalImageUrl = "https://images.unsplash.com/photo-1620799140408-edc6dcb6d633?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80"
//    },

//    // Design 9: Abstract Art Long Sleeve
//    new Design
//    {
//        Id = Guid.NewGuid(),
//        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
//        Prompt = "Abstract fluid art with gold leaf accents on black background",
//        NegativePrompt = "figurative, recognizable objects, simple",
//        Title = "Gold Abstract LS",
//        StylePreset = StylePresetType.Abstract,
//        Resolution = Resolution.Ultra,
//        ClothingType = ClothingType.LongSleeve,
//        Color = "#000000",
//        Size = ClothingSize.M,
//        Material = Material.Cotton,
//        PrintPosition = PrintPosition.Front,
//        FinalImageUrl = "https://images.unsplash.com/photo-1618005198919-d3d4b5a92ead?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80"
//    },

//    // Design 10: Galaxy Sweatshirt
//    new Design
//    {
//        Id = Guid.NewGuid(),
//        UserId = Guid.Parse("2b8b3e04-1ccf-490d-8a10-8d806e27f2a9"),
//        Prompt = "Spiral galaxy with stars and nebulas, cosmic photography style",
//        NegativePrompt = "earth, planets, solid colors",
//        Title = "Cosmic Galaxy Sweatshirt",
//        StylePreset = StylePresetType.Fantasy,
//        Resolution = Resolution.Ultra,
//        ClothingType = ClothingType.Sweatshirt,
//        Color = "#1B2631",
//        Size = ClothingSize.L,
//        Material = Material.CottonBlend,
//        PrintPosition = PrintPosition.Full,
//        FinalImageUrl = "https://images.unsplash.com/photo-1539609413529-1166774c6cb7?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80"

//    };