# TshirtMaker - AI-Powered Custom Apparel Design Platform

A modern web application that allows users to create custom apparel designs using AI technology, share them with the community, and discover trending designs from other creators.

## 🌟 Features

### Core Functionality
- **AI-Powered Design Generation**: Create unique designs using OpenAI's DALL-E API
- **Clothing Customization**: Choose from multiple clothing types (T-shirts, hoodies, jackets, etc.)
- **Color & Material Selection**: Pick any color and material for your apparel
- **Print Position**: Select front or back placement for your design
- **Content Moderation**: Built-in AI safety checks to prevent inappropriate content

### Social Features
- **Community Feed**: Browse and discover designs shared by other users
- **Like System**: Appreciate designs you love
- **Prompt Copying**: Copy prompts from designs for inspiration
- **Trending Carousel**: Auto-sliding showcase of top-liked designs
- **Infinite Scroll**: Smooth loading of designs as you scroll

### User Experience
- **Dark/Light Theme**: Toggle between themes with persistent preference
- **Modern UI**: Clean, futuristic design with smooth animations
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Real-time Updates**: Instant feedback and updates

## 🏗️ Architecture

### Project Structure
```
TshirtMaker/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          # Main navigation layout
│   │   └── EmptyLayout.razor         # Minimal layout for auth pages
│   ├── Pages/
│   │   ├── Home.razor                # Landing page with trending designs
│   │   ├── Create.razor              # Design creation interface
│   │   ├── Feed.razor                # Community feed with infinite scroll
│   │   ├── Login.razor               # Authentication page
│   │   └── SignUp.razor              # User registration page
│   └── ui/
│       └── common/
│           ├── ThemeToggle.razor     # Dark/light mode toggle
│           ├── DesignCard.razor      # Reusable design card component
│           └── LoadingSpinner.razor  # Loading indicator
├── Models/
│   ├── User.cs                       # User data model
│   ├── Design.cs                     # Design data model
│   ├── ClothingType.cs               # Enums for clothing options
│   └── AI/
│       └── DesignRequestDto.cs       # AI service DTOs
├── Services/
│   ├── TestDataService.cs            # In-memory data management
│   ├── AI/
│   │   └── AIDesignService.cs        # OpenAI integration
│   └── Supabase/
│       ├── SupabaseAuthService.cs    # Auth service (ready for use)
│       └── SupabaseDbService.cs      # Database service (ready for use)
└── wwwroot/
    ├── app.css                       # Global styles with Google Fonts
    ├── themes.css                    # Theme system (light/dark)
    └── theme.js                      # Theme toggle functionality
```

### Technology Stack
- **Framework**: .NET 10 Blazor Server
- **UI**: Bootstrap 5 + Custom CSS Variables
- **Fonts**: Google Fonts (Inter)
- **AI**: OpenAI DALL-E 3 API
- **Database**: Supabase (configured, ready to use)
- **State Management**: In-memory test data service

## 🚀 Setup Instructions

### Prerequisites
- .NET 10 SDK
- Visual Studio 2022 or VS Code
- OpenAI API Key (optional, for AI features)
- Supabase Account (optional, for production database)

### Installation Steps

1. **Clone the Repository**
   ```bash
   cd TshirtMaker
   ```

2. **Install Dependencies**
   The project uses .NET 10 and Bootstrap via CDN, so no additional package installation is needed.

3. **Configure API Keys**
   
   Open `appsettings.json` and add your API keys:
   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-your-openai-api-key-here"
     },
     "Supabase": {
       "Url": "https://your-project.supabase.co",
       "Key": "your-supabase-anon-key"
     }
   }
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```
   
   Or press F5 in Visual Studio.

5. **Access the Application**
   Navigate to `https://localhost:5001` or the URL shown in your terminal.

## 🗄️ Supabase Setup (For Production)

### Step 1: Create a Supabase Project
1. Go to [supabase.com](https://supabase.com)
2. Click "New Project"
3. Fill in project details and create

### Step 2: Set Up Database Tables

Run this SQL in Supabase SQL Editor:

```sql
-- Users table
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username TEXT UNIQUE NOT NULL,
    email TEXT UNIQUE NOT NULL,
    avatar_url TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Designs table
CREATE TABLE designs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    username TEXT NOT NULL,
    user_avatar TEXT,
    prompt TEXT NOT NULL,
    clothing_type TEXT NOT NULL,
    color TEXT NOT NULL,
    size TEXT NOT NULL,
    material TEXT NOT NULL,
    print_position TEXT NOT NULL,
    generated_image_url TEXT NOT NULL,
    final_image_url TEXT NOT NULL,
    likes INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    is_shared BOOLEAN DEFAULT false
);

-- Create indexes for better performance
CREATE INDEX idx_designs_user_id ON designs(user_id);
CREATE INDEX idx_designs_created_at ON designs(created_at DESC);
CREATE INDEX idx_designs_likes ON designs(likes DESC);
CREATE INDEX idx_designs_is_shared ON designs(is_shared);

-- Enable Row Level Security
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE designs ENABLE ROW LEVEL SECURITY;

-- Create policies (example - adjust based on your needs)
CREATE POLICY "Users can view all designs" ON designs
    FOR SELECT USING (is_shared = true);

CREATE POLICY "Users can insert their own designs" ON designs
    FOR INSERT WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update their own designs" ON designs
    FOR UPDATE USING (auth.uid() = user_id);
```

### Step 3: Enable Authentication
1. Go to Authentication > Settings in Supabase dashboard
2. Enable Email authentication
3. Configure email templates (optional)
4. Copy your project URL and anon key to `appsettings.json`

### Step 4: Configure Storage (Optional, for Image Uploads)
1. Go to Storage in Supabase dashboard
2. Create a new bucket named "designs"
3. Set the bucket to public
4. Configure policies for upload/download

### Step 5: Update Services
Once Supabase is configured, update the service implementations in:
- `Services/Supabase/SupabaseAuthService.cs`
- `Services/Supabase/SupabaseDbService.cs`

Install the Supabase NuGet package:
```bash
dotnet add package supabase-csharp
```

## 🤖 AI Service Explanation

### How the AI Design Service Works

The `AIDesignService` integrates with OpenAI's DALL-E 3 API to generate custom designs:

#### 1. Content Moderation
```csharp
private bool IsPromptSafe(string prompt)
```
- Checks user input against a list of banned keywords
- Prevents generation of inappropriate or harmful content
- Returns a friendly message if content is blocked
- **Production Note**: Consider using OpenAI's Moderation API for more robust filtering

#### 2. Prompt Enhancement
```csharp
private string EnhancePromptForClothing(DesignRequestDto request)
```
- Takes user's simple prompt and enhances it with context
- Adds information about clothing type, color, and print position
- Optimizes prompt for apparel-specific design generation
- Ensures better results by providing structured context to the AI

#### 3. Image Generation
```csharp
public async Task<DesignResponseDto> GenerateDesign(DesignRequestDto request)
```
- Validates API key configuration
- Performs content safety check
- Sends HTTP POST request to OpenAI API
- Handles errors gracefully
- Returns generated image URL or error message

#### API Request Structure
```json
{
  "model": "dall-e-3",
  "prompt": "Enhanced prompt with clothing context",
  "n": 1,
  "size": "1024x1024",
  "quality": "standard"
}
```

#### Error Handling
- Missing API key detection
- Network error handling
- API error response parsing
- User-friendly error messages

### Alternative Implementation (Using OpenAI Package)

If you prefer using the official OpenAI NuGet package:

```bash
dotnet add package OpenAI
```

Then modify the service:
```csharp
using OpenAI;
using OpenAI.Images;

public class AIDesignService
{
    private readonly OpenAIClient _client;

    public AIDesignService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        _client = new OpenAIClient(apiKey);
    }

    public async Task<DesignResponseDto> GenerateDesign(DesignRequestDto request)
    {
        // Content moderation logic...
        
        var response = await _client.ImagesEndpoint.GenerateImageAsync(
            enhancedPrompt,
            1,
            ImageSize._1024,
            quality: ImageQuality.Standard
        );

        return new DesignResponseDto
        {
            Success = true,
            ImageUrl = response.First().Url
        };
    }
}
```

## 🎨 Theme System

The application uses CSS custom properties for theming:
- Light/Dark mode toggle
- Smooth transitions between themes
- Persists across components
- Easy to customize colors

Edit `wwwroot/themes.css` to customize the color scheme.

## 🧪 Test Data

The application comes with pre-populated test data:
- 5 sample users
- 30 sample designs
- Random likes and timestamps

To use real data, switch from `TestDataService` to Supabase services in `Program.cs`.

## 📝 Development Notes

### Current State
- ✅ Fully functional with test data
- ✅ AI integration ready (needs API key)
- ✅ Supabase services scaffolded
- ✅ Modern, responsive UI
- ✅ Dark/Light theme support

### Production Checklist
- [ ] Add OpenAI API key
- [ ] Configure Supabase project
- [ ] Implement Supabase service methods
- [ ] Add authentication state management
- [ ] Implement image upload to storage
- [ ] Add user profile pages
- [ ] Add design deletion functionality
- [ ] Implement search functionality
- [ ] Add pagination to feed
- [ ] Set up CI/CD pipeline

## 🤝 Contributing

This project follows clean architecture principles and uses minimal dependencies. When contributing:
- Keep components small and focused
- Use Bootstrap and Tailwind utilities over custom CSS
- Follow the established naming conventions
- Add comments only where necessary

## 📄 License

This project is provided as-is for educational and commercial use.

## 🙋 Support

For issues or questions, please check the existing issues or create a new one.

---

**Made with ❤️ using .NET 10 Blazor and OpenAI**
