# ClothIQ - AI-Powered Custom Apparel Design Platform

A modern web application that allows users to create custom apparel designs using AI technology, share them with the community, and discover trending designs from other creators.

## 🌟 Features

### 🎨 Core Functionality

- **🤖 AI-Powered Design Generation**: Create unique designs using OpenAI's DALL-E API
- **👕 Clothing Customization**: Choose from multiple clothing types (T-shirts, hoodies, jackets, etc.)
- **🌈 Color & Material Selection**: Pick any color and material for your apparel
- **📏 Size Options**: Select from various size options for your apparel
- **🎨 Fabric Texture**: Choose from different fabric textures for realistic previews
- **📝 Prompt Enhancement**: AI-powered prompt enhancement with the bolt icon

### 🛠️ Creation Workflow

- **⚙️ Setup Stage**: Configure your apparel specifications (type, color, size, material)
- **✨ Creation Stage**: Generate AI artwork based on your prompt and style preferences
- **✅ Finalization Stage**: Save and share your completed design

### 🎯 Design Engine

- **🎨 Style Presets**: Multiple AI art style presets to choose from (Cyberpunk, Vintage, etc.)
- **📝 Advanced Settings**: Negative prompts and upscale resolution options
- **⚡ Quick Actions**: Rotate, preview in fullscreen, and enhance prompts

### 📱 User Experience

- **🚀 Interactive Server**: Real-time updates with Blazor Interactive Server rendering
- **📱 Responsive Design**: Works seamlessly on desktop and mobile devices
- **🔄 Session Management**: Proper session handling with 24-hour timeout

### 📊 Social Features

- **📰 Community Feed**: Browse and discover designs shared by other users
- **❤️ Like System**: Appreciate designs you love
- **📤 Share Functionality**: Share your creations with the community
- **🔍 Discover Tab**: Explore trending and popular designs

### 👤 User Management

- **🔐 Authentication**: Secure login and signup functionality
- **📊 Dashboard**: Personal dashboard with collections and orders
- **📦 Collections**: Organize your designs into collections
- **🛒 Orders**: Track your order history (in Progress🤔)

## 🏗️ Architecture

### 📁 Project Structure

```
TshirtMaker/
├── Components/
│   ├── Layout/                 # Main layouts and navigation
│   ├── Pages/                  # Page components
│   │   ├── App/               # Application pages (Create, Dashboard, etc.)
│   │   ├── Auth/              # Authentication pages
│   │   ├── Public/            # Public-facing pages
│   │   └── System/            # System pages
│   └── ui/                    # Reusable UI components
│       ├── common/            # Common UI elements
│       ├── creation/          # Creation workflow components
│       ├── Feed/              # Feed components
│       └── LandingPage/       # Landing page components
├── Models/                    # Data models and enums
│   ├── AI/                   # AI-related models
│   ├── Core/                 # Core business models
│   ├── Enums/                # Enumeration types
│   ├── Orders/               # Order-related models
│   └── Social/               # Social features models
├── Services/                  # Business logic and external integrations
│   ├── AI/                   # AI service implementations
│   ├── Supabase/             # Supabase integration services
│   └── Other services        #  Various utility services
├── Repositories/              # Data access layer
├── PublicData/                # Static data and presets
└── wwwroot/                   # Static assets (CSS, JS, images)
```

### 🛠️ Technology Stack

- **Framework**: .NET 10.0 Blazor Server with Interactive Server Rendering
- **UI Framework**: Bootstrap 5 + Custom CSS with CSS Variables
- **Typography**: Google Fonts (Inter, Space Grotesk, Poppins)
- **AI Integration**: OpenAI DALL-E API for image generation
- **Database**: Supabase (PostgreSQL) with Row Level Security
- **Storage**: Supabase Storage for user-generated content
- **Authentication**: Supabase Auth with JWT tokens
- **State Management**: Blazor Component State Management
- **Packages**: Supabase C# Client

## 🚀 Setup Instructions

### 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 / VS Code with C# extension
- OpenAI API Key (for AI features)
- Supabase Account (for production database)

### 🛠️ Installation Steps

1. **📥 Clone the Repository**

```bash
git clone https://github.com/Bhbored/TshirtMaker
cd TshirtMaker
```

2. **🔧 Install Dependencies**
   The project uses .NET 10 and the Supabase client, so restore packages:

   ```bash
   dotnet restore
   ```

3. **⚙️ Configure API Keys**

   Edit  `launchSettings.json` and add your API keys:

```json
{
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "SUPABASE_URL": "https://your-project.supabase.co",
    "SUPABASE_ANON_KEY": "your_anon_key_here",
    "OPENAI_API_KEY": "your_openai_key_here"
  }
}
```
4. **🏃Run the Application**

   ```bash
   dotnet run
   ```

## 🗄️ Supabase Setup (For Production)

### 🆕 Step 1: Create a Supabase Project

1. Go to [supabase.com](https://supabase.com)
2. Click "New Project"
3. Fill in project details and create

### 🗃️ Step 2: Set Up Database Tables

Run this storage-rls-design-images.sql in Supabase SQL Editor for Tables creation , RLS , Indexing and Storage.

### 💾 Step 3: Configure Storage (For Image Uploads)

1. Go to Storage in Supabase dashboard
2. Create buckets named "Design images"
3. Set the buckets to public
4. Configure policies for upload/download

### 🔄 Step 5: Update Services

The services are already implemented and configured in the project:

- `Services/Supabase/SupabaseAuthService.cs` - Authentication service
- `Repositories/` - Data access layer with comprehensive CRUD operations

## 🤖 AI Service Explanation

### 🧠 How the AI Design Service Works

The `OpenAIDesignService` integrates with OpenAI's DALL-E API to generate custom designs:

#### 1. 🔍 Content Moderation

```csharp
// Checks user input against a list of banned keywords
var unsafeKeywords = new[] { "nudity", "violence", "hate speech", "gore", "explicit", "sexual" };
```

- Checks user input against a list of banned keywords
- Prevents generation of inappropriate or harmful content
- Returns a friendly message if content is blocked

#### 2. 📝 Prompt Enhancement

```csharp
string modifiedPrompt = $"Create 1 variation of the following prompt: {prompt}";
```

- Takes user's simple prompt and enhances it with context
- Adds information about style preferences
- Optimizes prompt for apparel-specific design generation

#### 3. 🖼️ Image Generation

```csharp
public async Task<List<string>> GenerateInitialDesignsAsync(...)
```

- Validates API key configuration
- Performs content safety check
- Sends HTTP POST request to OpenAI API
- Handles errors gracefully
- Returns generated image URLs or error message

#### 4. 🎯 Design Finalization

```csharp
public async Task<string> FinalizeDesignAsync(...)
```

- Combines the generated design with the clothing image
- Applies color adjustments to match user preferences
- Creates a realistic preview of the final product

## 📊 Repository Pattern

The application implements a comprehensive repository pattern:

- 🔐 Secure data access with Supabase authentication
- 🔄 Comprehensive CRUD operations for all entities
- 🛡️ Proper error handling and validation
- 📦 Separated concerns with dedicated repository interfaces

## 📝 Development Notes

### ✅ Current State

- Fully functional with Supabase integration
- AI integration ready (needs API key)
- Complete authentication system
- Modern, responsive UI with interactive elements
- Comprehensive repository pattern implementation
- Full e-commerce functionality (collections, orders, etc.)

### 📞 Contact

Bourhan Hassoun - [bhbored2022@gmail.com] | [[LinkedIn Profile](https://www.linkedin.com/in/bourhan-hassoun-327670303/)]

Project Link:

```
https://github.com/Bhbored/TshirtMaker
```
