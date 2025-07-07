# Render Deployment Guide - 100% Free (No Credit Card Required)

## Why Render?
- ✅ **100% Free Tier** - No credit card required
- ✅ **ASP.NET Core Support** - Official support
- ✅ **Easy Deployment** - Connect GitHub directly
- ✅ **Free SSL** - Automatic HTTPS
- ✅ **Free Subdomain** - your-app.onrender.com
- ✅ **Database Included** - PostgreSQL free tier

## Step 1: Prepare Your Application

### 1.1 Update for Render
```bash
# Create render.yaml for configuration
```

### 1.2 Create render.yaml
```yaml
services:
  - type: web
    name: 29-asset-management
    env: dotnet
    buildCommand: dotnet build
    startCommand: dotnet run --urls http://0.0.0.0:$PORT
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://0.0.0.0:$PORT
      - key: DB_HOST
        fromDatabase:
          name: ams-database
          property: host
      - key: DB_NAME
        fromDatabase:
          name: ams-database
          property: database
      - key: DB_USER
        fromDatabase:
          name: ams-database
          property: username
      - key: DB_PASSWORD
        fromDatabase:
          name: ams-database
          property: password
      - key: DB_PORT
        fromDatabase:
          name: ams-database
          property: port
      - key: SMTP_SERVER
        value: smtp.gmail.com
      - key: SMTP_PORT
        value: 587
      - key: SMTP_USERNAME
        sync: false
      - key: SMTP_PASSWORD
        sync: false
      - key: FROM_EMAIL
        sync: false
      - key: JWT_SECRET_KEY
        generateValue: true

databases:
  - name: ams-database
    databaseName: ams
    user: ams_user
    plan: free
```

### 1.3 Update appsettings.Production.json for Render
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD};Port=${DB_PORT};"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "EmailSettings": {
    "SmtpServer": "${SMTP_SERVER}",
    "SmtpPort": "${SMTP_PORT}",
    "SmtpUsername": "${SMTP_USERNAME}",
    "SmtpPassword": "${SMTP_PASSWORD}",
    "FromEmail": "${FROM_EMAIL}",
    "FromName": "29 Asset Management System"
  },
  "JwtSettings": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "Issuer": "29AssetManagement",
    "Audience": "29AssetUsers",
    "ExpirationHours": 24
  }
}
```

## Step 2: Deploy to Render

### 2.1 Create Render Account
1. Go to [render.com](https://render.com)
2. Click "Get Started for Free"
3. Sign up with GitHub (no credit card required)

### 2.2 Connect GitHub Repository
1. Click "New +" → "Web Service"
2. Connect your GitHub account
3. Select your repository
4. Choose the branch (main/master)

### 2.3 Configure Service
- **Name**: `29-asset-management`
- **Environment**: `Dotnet`
- **Build Command**: `dotnet build`
- **Start Command**: `dotnet run --urls http://0.0.0.0:$PORT`

### 2.4 Add Environment Variables
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_app_password
FROM_EMAIL=your_email@gmail.com
```

### 2.5 Create Database
1. Click "New +" → "PostgreSQL"
2. **Name**: `ams-database`
3. **Plan**: Free
4. **Database**: `ams`
5. **User**: `ams_user`

### 2.6 Link Database to Web Service
1. Go to your web service
2. Click "Environment"
3. Add database environment variables:
   ```
   DB_HOST=your-db-host.onrender.com
   DB_NAME=ams
   DB_USER=ams_user
   DB_PASSWORD=your-db-password
   DB_PORT=5432
   ```

## Step 3: Database Migration

### 3.1 Update for PostgreSQL
```bash
# Install PostgreSQL provider
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# Update connection string
# Server=your-host;Database=ams;User Id=ams_user;Password=your-password;Port=5432;
```

### 3.2 Run Migrations
```bash
# Connect to Render database
psql "postgresql://ams_user:password@host:5432/ams"

# Or use Render's database console
```

## Step 4: Configure Email (Gmail)

### 4.1 Gmail App Password Setup
1. Enable 2-factor authentication on Gmail
2. Go to Google Account → Security → App passwords
3. Generate app password for "Mail"
4. Use this password in SMTP_PASSWORD

### 4.2 Update Environment Variables
```
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_16_char_app_password
FROM_EMAIL=your_email@gmail.com
```

## Step 5: Custom Domain (Optional)

### 5.1 Add Custom Domain
1. Go to your web service
2. Click "Settings" → "Custom Domains"
3. Add your domain
4. Update DNS records

### 5.2 Update Application URL
```json
{
  "ApplicationUrl": "https://your-domain.com"
}
```

## Free Tier Limits

### Render Free Tier
- **Web Services**: 750 hours/month
- **Databases**: 90 days free trial
- **Bandwidth**: 100 GB/month
- **Build Time**: 500 minutes/month

### Cost Optimization
- Use sleep mode for development
- Optimize build times
- Monitor usage in dashboard

## Troubleshooting

### Common Issues
1. **Build Failures**
   - Check build logs
   - Verify .NET version
   - Ensure all dependencies

2. **Database Connection**
   - Verify connection string
   - Check database status
   - Ensure proper permissions

3. **Email Not Working**
   - Verify Gmail app password
   - Check SMTP settings
   - Test email configuration

### Support
- **Render Docs**: [docs.render.com](https://docs.render.com)
- **Community**: Render Discord
- **Status**: [status.render.com](https://status.render.com)

## Alternative Free Options

### 1. **Railway** (Limited Free)
- Free tier available
- Easy deployment
- PostgreSQL included

### 2. **Heroku** (Limited Free)
- Free tier discontinued
- Paid plans only

### 3. **Vercel** (No ASP.NET Support)
- Only supports Node.js, Python, etc.
- No ASP.NET Core support

### 4. **Netlify** (No ASP.NET Support)
- Only for static sites
- No backend support

## Recommendation

**Render** က အသင့်ဆုံးဖြစ်ပါတယ်။
- ✅ 100% free
- ✅ Credit card မလို
- ✅ ASP.NET Core support
- ✅ Database included
- ✅ Easy deployment
- ✅ Free SSL

---

**Note**: Render free tier မှာ service က sleep mode ဖြစ်သွားနိုင်တယ်။ ပထမဆုံး request လာတဲ့အခါ wake up ဖြစ်ဖို့ အချိန်အနည်းငယ် ကြာနိုင်ပါတယ်။ 