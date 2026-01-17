using TshirtMaker.Models.Core;
using TshirtMaker.Models.Enums;

namespace TshirtMaker.PublicData
{
    public static class PublicPresets
    {
        public static readonly List<MaterialPreview> MaterialPresets = new()
        {
            new MaterialPreview
            {
                Title = "Heavy Cotton",
                Material = Material.HeavyCotton.ToString(),
                PreviewImageUrl = "https://images.unsplash.com/photo-1512436991641-6745cdb1723f?w=800&h=600&fit=crop"
            },
            new MaterialPreview
            {
                Title = "Polyester",
                Material = Material.Polyester.ToString(),
                PreviewImageUrl = "https://images.unsplash.com/photo-1597260256523-55d2cd529a09?w=800&h=600&fit=crop"
            },
            new MaterialPreview
            {
                Title = "Cotton Polyester Blend",
                Material = Material.CottonPolyesterBlend.ToString(),
                PreviewImageUrl = "https://images.unsplash.com/photo-1563276790-62a0e7b46ba5?w=800&h=600&fit=crop"
            },
            new MaterialPreview
            {
                Title = "Linen",
                Material = Material.Linen.ToString(),
                PreviewImageUrl = "https://images.unsplash.com/photo-1592721759765-feff321ec04c?w=800&h=600&fit=crop"
            },
            new MaterialPreview
            {
                Title = "Wool",
                Material = Material.Wool.ToString(),
                PreviewImageUrl = "https://images.unsplash.com/photo-1546574877-5addcb46e7e5?w=800&h=600&fit=crop"
            },
            new MaterialPreview
            {
                Title = "Fleece",
                Material = Material.Fleece.ToString(),
                PreviewImageUrl = "https://images.unsplash.com/photo-1495259331600-4a2e58d38689?w=800&h=600&fit=crop"
            }
        };

        public static readonly List<StylePresetPreview> StylePresetPreviews = new()
        {
            new StylePresetPreview
            {
                Title = "Cyberpunk",
                StylePreset = StylePresetType.Cyberpunk,
                PreviewImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Anime",
                StylePreset = StylePresetType.Anime,
                PreviewImageUrl = "https://images.unsplash.com/photo-1578632767115-351597cf2477?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Minimalism",
                StylePreset = StylePresetType.Minimalism,
                PreviewImageUrl = "https://images.unsplash.com/photo-1557683316-973673baf926?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Oil Paint",
                StylePreset = StylePresetType.OilPaint,
                PreviewImageUrl = "https://images.unsplash.com/photo-1541961017774-22349e4a1262?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Watercolor",
                StylePreset = StylePresetType.Watercolor,
                PreviewImageUrl = "https://images.unsplash.com/photo-1579783902614-a53fb6c2b9c1?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Vintage",
                StylePreset = StylePresetType.Vintage,
                PreviewImageUrl = "https://images.unsplash.com/photo-1513475382585-d06e58bcb0e0?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Gothic",
                StylePreset = StylePresetType.Gothic,
                PreviewImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Streetwear",
                StylePreset = StylePresetType.Streetwear,
                PreviewImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Abstract",
                StylePreset = StylePresetType.Abstract,
                PreviewImageUrl = "https://images.unsplash.com/photo-1557672172-298e090bd0f1?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Photorealistic",
                StylePreset = StylePresetType.Photorealistic,
                PreviewImageUrl = "https://images.unsplash.com/photo-1513475382585-d06e58bcb0e0?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Pixel Art",
                StylePreset = StylePresetType.PixelArt,
                PreviewImageUrl = "https://images.unsplash.com/photo-1578632767115-351597cf2477?w=800&h=600&fit=crop"
            },
            new StylePresetPreview
            {
                Title = "Graffiti",
                StylePreset = StylePresetType.Graffiti,
                PreviewImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=800&h=600&fit=crop"
            }
        };
    }
}
