🛒 ASP.NET Core E-Commerce API
This is a full-featured ASP.NET Core Web API built using modern tools and libraries for managing users, products, orders, and payments.

🚀 Features
✅ JWT Authentication — Secure API access using tokens

🧾 Stripe Integration — Create and manage payment intents

🖼 Cloudinary — Upload and manage product images

🔐 ASP.NET Core Identity — User and role management

🧪 FluentValidation — Request validation with clean separation of concerns

🗂 Entity Framework Core — SQL Server-based ORM with migrations

🌍 dotenv.net — Load environment variables from .env files

🔄 AutoMapper — Streamlined mapping between DTOs and entities

📖 Swagger — API documentation with interactive UI

🛠 Technologies
Tool / Library	Purpose
Microsoft.AspNetCore.Authentication.JwtBearer	JWT authentication
Microsoft.AspNetCore.Identity.EntityFrameworkCore	Identity user management
Stripe.net	Stripe payments
CloudinaryDotNet	Image uploads
dotenv.net	Environment variables from .env
FluentValidation	Input validation
AutoMapper	Object mapping
EF Core (SqlServer + Tools)	Database ORM
Swashbuckle.AspNetCore	Swagger / OpenAPI UI

📦 Project Structure
bash
Copy
Edit
/Controllers        # API endpoints
/Models             # Data models and enums
/DTOs               # Data transfer objects
/Services           # Business logic and helpers
/Middleware         # Custom middleware (e.g. whitelisting, culture, etc.)
/Helpers            # Utilities and config extensions
images/             # Static image folder (optional)
.env                # Environment-specific variables
Program.cs          # Application entry point
🔐 Authentication
All secure routes are protected using JWT tokens. Users are authenticated with ASP.NET Core Identity. Claims and roles are embedded in the token.

💳 Payments
Stripe is used to:

Create and manage payment intents

Handle secure payment processing

Use your Stripe API key via environment variables.

☁️ Image Uploads
Images are uploaded using Cloudinary, with signed secure uploads from the backend.

⚙️ Configuration
Use .env for secrets and keys. Example:

env
Copy
Edit
STRIPE_SECRET_KEY=sk_test_...
CLOUDINARY_CLOUD_NAME=mycloud
CLOUDINARY_API_KEY=123456
CLOUDINARY_API_SECRET=abcdef
JWT_SECRET=myjwtsecret
JWT_REFRESH_SECRET_KEY=refreshKey
📚 API Docs
Swagger UI is available at:

arduino
Copy
Edit
https://yourdomain.com/swagger
📥 Getting Started
bash
Copy
Edit
# Install dependencies and tools
dotnet restore

# Apply EF migrations
dotnet ef database update

# Run the application
dotnet run
🧪 Testing with Postman
Use Postman to test all endpoints. Remember to pass the JWT token in the Authorization header for protected routes.

