# Supabase Setup Guide for TshirtMaker

This document provides instructions for setting up Supabase for the TshirtMaker application, including the necessary PostgreSQL schema and configuration.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Supabase Project Setup](#supabase-project-setup)
- [Database Schema](#database-schema)
- [Configuration](#configuration)
- [VIMP Rules](#vimp-rules)
- [Indexing Strategy](#indexing-strategy)

## Prerequisites

Before setting up Supabase for TshirtMaker, ensure you have:
- A Supabase account (sign up at [supabase.com](https://supabase.com))
- The Supabase CLI installed (optional but recommended)
- Basic knowledge of PostgreSQL

## Supabase Project Setup

1. Create a new project in your Supabase dashboard
2. Note down your Project URL and anon/public API key
3. Configure Row Level Security (RLS) policies as needed for your application

## Database Schema

Run the following SQL commands in your Supabase SQL Editor to create the required tables:

```sql
-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create users table
CREATE TABLE users (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    -- password_hash is handled by Supabase authentication
    bio VARCHAR(500),
    avatar_url VARCHAR(2048),
    cover_image_url VARCHAR(2048),
    location VARCHAR(100),
    website_url VARCHAR(2048),
    total_shares INTEGER DEFAULT 0,
    following_count INTEGER DEFAULT 0,
    last_login_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON
    designs UUID[], -- Array of design IDs
    posts UUID[], -- Array of post IDs
    likes_given UUID[], -- Array of like IDs
    likes_taken UUID[], -- Array of like IDs
    comments UUID[], -- Array of comment IDs
    bookmarks UUID[], -- Array of bookmark IDs
    followers UUID[], -- Array of follower IDs
    notifications UUID[], -- Array of notification IDs
    collections UUID[] -- Array of collection IDs
);

-- Create designs table
CREATE TABLE designs (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    prompt VARCHAR(500) NOT NULL,
    negative_prompt VARCHAR(500),
    title VARCHAR(50) NOT NULL,
    style_preset INTEGER, -- Enum value for StylePresetType
    resolution INTEGER DEFAULT 0, -- Enum value for Resolution
    clothing_type INTEGER NOT NULL, -- Enum value for ClothingType
    color VARCHAR(7) DEFAULT '#FFFFFF',
    size INTEGER NOT NULL, -- Enum value for ClothingSize
    material INTEGER NOT NULL, -- Enum value for Material
    print_position INTEGER DEFAULT 0, -- Enum value for PrintPosition
    final_image_url VARCHAR(2048),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation property stored as JSON
    user_ref UUID -- Reference to user ID
);

-- Create collections table
CREATE TABLE collections (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    copied_design_id UUID REFERENCES designs(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON
    copied_design_ref UUID, -- Reference to design ID
    user_ref UUID -- Reference to user ID
);

-- Create material_previews table
CREATE TABLE material_previews (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    material VARCHAR(50) NOT NULL,
    preview_image_url VARCHAR(2048) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE
);

-- Create style_preset_previews table
CREATE TABLE style_preset_previews (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    style_preset INTEGER NOT NULL, -- Enum value for StylePresetType
    preview_image_url VARCHAR(2048) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE
);

-- Create posts table
CREATE TABLE posts (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    poster_id UUID REFERENCES users(id) ON DELETE CASCADE,
    design_id UUID REFERENCES designs(id) ON DELETE CASCADE,
    description VARCHAR(100),
    remix_count INTEGER DEFAULT 0,
    allow_remix BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON arrays
    likes UUID[], -- Array of like IDs
    comments UUID[], -- Array of comment IDs
    bookmarks UUID[], -- Array of bookmark IDs
    
    -- Navigation properties stored as JSON references
    poster_ref UUID, -- Reference to user ID
    design_ref UUID -- Reference to design ID
);

-- Create likes table
CREATE TABLE likes (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    post_id UUID REFERENCES posts(id) ON DELETE CASCADE,
    liked_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON references
    user_ref UUID, -- Reference to user ID
    post_ref UUID -- Reference to post ID
);

-- Create comments table
CREATE TABLE comments (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    post_id UUID REFERENCES posts(id) ON DELETE CASCADE,
    text VARCHAR(1000) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON references
    user_ref UUID, -- Reference to user ID
    post_ref UUID -- Reference to post ID
);

-- Create bookmarks table
CREATE TABLE bookmarks (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    post_id UUID REFERENCES posts(id) ON DELETE CASCADE,
    bookmarked_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON references
    user_ref UUID, -- Reference to user ID
    post_ref UUID -- Reference to post ID
);

-- Create followers table
CREATE TABLE followers (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    follower_id UUID REFERENCES users(id) ON DELETE CASCADE,
    following_id UUID REFERENCES users(id) ON DELETE CASCADE,
    is_mutual BOOLEAN DEFAULT FALSE,
    followed_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON references
    follower_user_ref UUID, -- Reference to follower user ID
    following_user_ref UUID -- Reference to following user ID
);

-- Create notifications table
CREATE TABLE notifications (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    recipient_id UUID REFERENCES users(id) ON DELETE CASCADE,
    sender_id UUID REFERENCES users(id) ON DELETE CASCADE,
    type INTEGER NOT NULL, -- Enum value for NotificationType
    post_id UUID REFERENCES posts(id) ON DELETE CASCADE,
    message VARCHAR(500),
    is_read BOOLEAN DEFAULT FALSE,
    action_taken BOOLEAN DEFAULT FALSE,
    notified_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON references
    recipient_ref UUID, -- Reference to recipient user ID
    sender_ref UUID, -- Reference to sender user ID
    post_ref UUID -- Reference to post ID
);

-- Create shipping_addresses table
CREATE TABLE shipping_addresses (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    full_name VARCHAR(100) NOT NULL,
    phone_number VARCHAR(20),
    email VARCHAR(255),
    address_line1 VARCHAR(200) NOT NULL,
    address_line2 VARCHAR(200),
    city VARCHAR(100) NOT NULL,
    state_province VARCHAR(100) NOT NULL,
    postal_code VARCHAR(20) NOT NULL,
    country_code CHAR(2) DEFAULT 'US',
    country VARCHAR(100),
    is_default BOOLEAN DEFAULT FALSE,
    is_billing_address BOOLEAN DEFAULT FALSE,
    label VARCHAR(50),
    is_validated BOOLEAN DEFAULT FALSE,
    validated_at TIMESTAMP WITH TIME ZONE,
    validation_notes VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation property stored as JSON reference
    user_ref UUID -- Reference to user ID
);

-- Create orders table
CREATE TABLE orders (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    order_number VARCHAR(20) UNIQUE NOT NULL,
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    status INTEGER NOT NULL DEFAULT 0, -- Enum value for OrderStatus
    progress_step INTEGER DEFAULT 1,
    subtotal DECIMAL(10,2) DEFAULT 0.00,
    shipping_cost DECIMAL(10,2) DEFAULT 0.00,
    tax DECIMAL(10,2) DEFAULT 0.00,
    discount DECIMAL(10,2) DEFAULT 0.00,
    total DECIMAL(10,2) DEFAULT 0.00,
    currency VARCHAR(3) DEFAULT 'USD',
    shipping_address_id UUID REFERENCES shipping_addresses(id),
    shipping_method VARCHAR(100),
    carrier_name VARCHAR(100),
    tracking_number VARCHAR(100),
    placed_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    confirmed_at TIMESTAMP WITH TIME ZONE,
    production_started_at TIMESTAMP WITH TIME ZONE,
    shipped_at TIMESTAMP WITH TIME ZONE,
    delivered_at TIMESTAMP WITH TIME ZONE,
    cancelled_at TIMESTAMP WITH TIME ZONE,
    estimated_delivery_date TIMESTAMP WITH TIME ZONE,
    customer_notes VARCHAR(1000),
    internal_notes VARCHAR(1000),
    cancellation_reason VARCHAR(500),
    cancelled_by_user_id UUID REFERENCES users(id),
    payment_id UUID,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON arrays
    items UUID[], -- Array of order item IDs
    tracking_events UUID[], -- Array of tracking event IDs
    
    -- Navigation properties stored as JSON references
    user_ref UUID, -- Reference to user ID
    shipping_address_ref UUID -- Reference to shipping address ID
);

-- Create order_items table
CREATE TABLE order_items (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    order_id UUID REFERENCES orders(id) ON DELETE CASCADE,
    design_id UUID REFERENCES designs(id) ON DELETE CASCADE,
    clothing_type INTEGER NOT NULL, -- Enum value for ClothingType
    size INTEGER NOT NULL, -- Enum value for ClothingSize
    material INTEGER NOT NULL, -- Enum value for Material
    color VARCHAR(7) DEFAULT '#FFFFFF',
    print_position INTEGER DEFAULT 0, -- Enum value for PrintPosition
    quantity INTEGER DEFAULT 1 CHECK (quantity >= 1 AND quantity <= 1000),
    unit_price DECIMAL(10,2) NOT NULL,
    total_price DECIMAL(10,2) NOT NULL,
    customization_notes VARCHAR(1000),
    production_status VARCHAR(50),
    printed_at TIMESTAMP WITH TIME ZONE,
    quality_checked_at TIMESTAMP WITH TIME ZONE,
    mockup_image_url VARCHAR(2048),
    print_file_url VARCHAR(2048),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation properties stored as JSON references
    order_ref UUID, -- Reference to order ID
    design_ref UUID -- Reference to design ID
);

-- Create tracking_events table
CREATE TABLE tracking_events (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    order_id UUID REFERENCES orders(id) ON DELETE CASCADE,
    title VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    location VARCHAR(200),
    event_date TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    is_active BOOLEAN DEFAULT FALSE,
    carrier_name VARCHAR(100),
    carrier_code VARCHAR(100),
    event_type VARCHAR(50),
    additional_info VARCHAR(1000),
    display_order INTEGER,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    
    -- Navigation property stored as JSON reference
    order_ref UUID -- Reference to order ID
);
```

## Configuration

Add the following configuration to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SupabaseDb": "Host=your-project.supabase.co;Database=postgres;Username=your-username;Password=your-password"
  },
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-anon-key",
    "DatabaseUrl": "postgresql://your-db-url"
  }
}
```

## VIMP Rules

### User Rules
- Users can only view their own personal information and associated data
- Users can only modify their own profile information
- Users can only delete their own posts, comments, and other user-generated content
- Users can only view orders associated with their account
- Users can only follow/unfollow other users (not themselves)

### Data Access Rules
- Posts can be viewed publicly unless marked as private
- Comments on posts are visible to anyone who can view the post
- Likes and bookmarks are visible to anyone who can view the post
- Orders are only accessible by the owner
- Personal information (email, addresses) is restricted to the owner and authorized personnel

## Indexing Strategy

The following indexes are recommended for optimal performance:

```sql
-- Users table indexes
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_created_at ON users(created_at);

-- Designs table indexes
CREATE INDEX idx_designs_user_id ON designs(user_id);
CREATE INDEX idx_designs_clothing_type ON designs(clothing_type);
CREATE INDEX idx_designs_material ON designs(material);
CREATE INDEX idx_designs_style_preset ON designs(style_preset);
CREATE INDEX idx_designs_created_at ON designs(created_at);

-- Posts table indexes
CREATE INDEX idx_posts_poster_id ON posts(poster_id);
CREATE INDEX idx_posts_design_id ON posts(design_id);
CREATE INDEX idx_posts_created_at ON posts(created_at DESC);

-- Likes table indexes
CREATE INDEX idx_likes_user_id ON likes(user_id);
CREATE INDEX idx_likes_post_id ON likes(post_id);
CREATE INDEX idx_likes_user_post_unique ON likes(user_id, post_id); -- For uniqueness constraint

-- Comments table indexes
CREATE INDEX idx_comments_post_id ON comments(post_id);
CREATE INDEX idx_comments_user_id ON comments(user_id);
CREATE INDEX idx_comments_created_at ON comments(created_at);

-- Followers table indexes
CREATE INDEX idx_followers_follower_id ON followers(follower_id);
CREATE INDEX idx_followers_following_id ON followers(following_id);
CREATE INDEX idx_followers_unique ON followers(follower_id, following_id); -- For uniqueness constraint

-- Orders table indexes
CREATE INDEX idx_orders_user_id ON orders(user_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_order_number ON orders(order_number);
CREATE INDEX idx_orders_placed_at ON orders(placed_at DESC);

-- Order items table indexes
CREATE INDEX idx_order_items_order_id ON order_items(order_id);
CREATE INDEX idx_order_items_design_id ON order_items(design_id);

-- Notifications table indexes
CREATE INDEX idx_notifications_recipient_id ON notifications(recipient_id);
CREATE INDEX idx_notifications_is_read ON notifications(is_read);
CREATE INDEX idx_notifications_created_at ON notifications(created_at DESC);

-- Shipping addresses table indexes
CREATE INDEX idx_shipping_addresses_user_id ON shipping_addresses(user_id);
CREATE INDEX idx_shipping_addresses_country_code ON shipping_addresses(country_code);
CREATE INDEX idx_shipping_addresses_is_default ON shipping_addresses(user_id, is_default);
```

## Row Level Security (RLS) Policies

Enable RLS on sensitive tables and define policies:

```sql
-- Enable RLS
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE orders ENABLE ROW LEVEL SECURITY;
ALTER TABLE shipping_addresses ENABLE ROW LEVEL SECURITY;
ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;

-- Users table policy - users can only access their own data
CREATE POLICY user_access_policy ON users
  FOR ALL TO authenticated
  USING (auth.uid() = id);

-- Orders table policy - users can only access their own orders
CREATE POLICY order_access_policy ON orders
  FOR ALL TO authenticated
  USING (user_id = auth.uid());

-- Shipping addresses table policy - users can only access their own addresses
CREATE POLICY shipping_address_access_policy ON shipping_addresses
  FOR ALL TO authenticated
  USING (user_id = auth.uid());

-- Notifications table policy - users can only access their own notifications
CREATE POLICY notification_access_policy ON notifications
  FOR ALL TO authenticated
  USING (recipient_id = auth.uid());
```

## Environment Variables

Set these environment variables in your Supabase project settings:

- `SUPABASE_URL`: Your Supabase project URL
- `SUPABASE_ANON_KEY`: Your Supabase anon key
- `SUPABASE_SERVICE_ROLE_KEY`: Your Supabase service role key (for server-side operations)