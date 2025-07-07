# Azure App Service Deployment Guide
## 29 Asset Management System

### Prerequisites
- Azure Account (Free tier available)
- Azure CLI installed (optional)
- Git repository with your code

---

## Step 1: Azure Account Setup

### 1.1 Create Azure Account
1. Go to [Azure Portal](https://portal.azure.com)
2. Sign up for free account (12 months free tier available)
3. Verify your account

### 1.2 Install Azure CLI (Optional)
```bash
# macOS
brew install azure-cli

# Windows
winget install Microsoft.AzureCLI

# Linux
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

---

## Step 2: Create Azure Resources

### 2.1 Create Resource Group
1. Go to Azure Portal
2. Click "Create a resource"
3. Search for "Resource group"
4. Create new resource group:
   - **Name**: `29asset-rg`
   - **Region**: Southeast Asia (Singapore) or East Asia (Hong Kong)

### 2.2 Create MySQL Database (Azure Database for MySQL)
1. Go to Azure Portal
2. Click "Create a resource"
3. Search for "Azure Database for MySQL"
4. Select "Azure Database for MySQL - Flexible Server"
5. Configure:
   - **Server name**: `29asset-mysql`
   - **Admin username**: `admin`
   - **Password**: `StrongPassword123!`
   - **Database name**: `ams_db`
   - **Region**: Same as Resource Group
   - **Compute + storage**: Basic tier (free eligible)

### 2.3 Create App Service Plan
1. Go to Azure Portal
2. Click "Create a resource"
3. Search for "App Service Plan"
4. Configure:
   - **Name**: `29asset-plan`
   - **Region**: Same as Resource Group
   - **Operating System**: Windows
   - **Pricing Plan**: F1 (Free) or B1 (Basic)

### 2.4 Create Web App
1. Go to Azure Portal
2. Click "Create a resource"
3. Search for "Web App"
4. Configure:
   - **Name**: `29asset-app`
   - **Publish**: Code
   - **Runtime stack**: .NET 9 (LTS)
   - **Operating System**: Windows
   - **Region**: Same as Resource Group
   - **App Service Plan**: Select the one created above

---

## Step 3: Configure Environment Variables

### 3.1 Database Connection
1. Go to your Web App
2. Navigate to "Settings" → "Configuration"
3. Add these Application Settings:

```
DB_SERVER = 29asset-mysql.mysql.database.azure.com
DB_NAME = ams_db
DB_USER = admin
DB_PASSWORD = StrongPassword123!
```

### 3.2 JWT Configuration
```
JWT_SECRET_KEY = 29AssetManagementSystem2024SecretKeyForJWTTokenGeneration
JWT_ISSUER = https://29asset-app.azurewebsites.net
JWT_AUDIENCE = https://29asset-app.azurewebsites.net
```

### 3.3 Email Settings (Optional)
```
SMTP_SERVER = smtp.gmail.com
SMTP_USERNAME = your-email@gmail.com
SMTP_PASSWORD = your-app-password
SENDER_EMAIL = your-email@gmail.com
```

### 3.4 ASP.NET Core Settings
```
ASPNETCORE_ENVIRONMENT = Production
ASPNETCORE_URLS = http://0.0.0.0:80
```

---

## Step 4: Deploy Application

### 4.1 Method 1: Azure Portal (Easiest)
1. Go to your Web App
2. Navigate to "Deployment Center"
3. Choose "GitHub" or "Local Git/FTPS credentials"
4. Connect your repository
5. Deploy automatically

### 4.2 Method 2: Azure CLI
```bash
# Login to Azure
az login

# Set subscription
az account set --subscription "Your Subscription ID"

# Deploy from local folder
az webapp deployment source config-local-git --name 29asset-app --resource-group 29asset-rg

# Get deployment URL
az webapp deployment list-publishing-credentials --name 29asset-app --resource-group 29asset-rg

# Deploy using Git
git remote add azure <deployment-url>
git push azure main
```

### 4.3 Method 3: Visual Studio
1. Open project in Visual Studio
2. Right-click project → "Publish"
3. Choose "Azure App Service"
4. Select your Web App
5. Publish

---

## Step 5: Database Migration

### 5.1 Enable SSH in Web App
1. Go to your Web App
2. Navigate to "Development Tools" → "SSH"
3. Enable SSH

### 5.2 Run Migrations
```bash
# Connect to Web App via SSH
ssh 29asset-app.scm.azurewebsites.net

# Navigate to app directory
cd /home/site/wwwroot

# Run migrations
dotnet ef database update
```

---

## Step 6: Configure Custom Domain (Optional)

### 6.1 Add Custom Domain
1. Go to your Web App
2. Navigate to "Settings" → "Custom domains"
3. Add your domain
4. Configure DNS records

### 6.2 SSL Certificate
- Azure automatically provides SSL for *.azurewebsites.net
- For custom domains, you can use Azure's free SSL certificates

---

## Step 7: Monitoring and Logs

### 7.1 Application Logs
1. Go to your Web App
2. Navigate to "Monitoring" → "Log stream"
3. View real-time logs

### 7.2 Application Insights (Optional)
1. Create Application Insights resource
2. Connect to your Web App
3. Monitor performance and errors

---

## Troubleshooting

### Common Issues

#### 1. Database Connection Error
- Check connection string in App Settings
- Verify MySQL server is running
- Check firewall rules

#### 2. Build Errors
- Ensure .NET 9 runtime is selected
- Check build logs in Deployment Center

#### 3. Runtime Errors
- Check Application Logs
- Verify environment variables
- Check database migrations

### Useful Commands
```bash
# View logs
az webapp log tail --name 29asset-app --resource-group 29asset-rg

# Restart app
az webapp restart --name 29asset-app --resource-group 29asset-rg

# Scale app
az appservice plan update --name 29asset-plan --resource-group 29asset-rg --sku B1
```

---

## Cost Optimization

### Free Tier Limits
- **App Service**: 1 F1 instance (shared)
- **MySQL**: Basic tier (free eligible)
- **Bandwidth**: 5 GB/month

### Scaling Options
- **Basic Plan**: $13/month (dedicated resources)
- **Standard Plan**: $73/month (auto-scaling)
- **Premium Plan**: $146/month (VNet integration)

---

## Security Best Practices

### 1. Environment Variables
- Never commit secrets to source code
- Use Azure Key Vault for sensitive data
- Rotate passwords regularly

### 2. Network Security
- Use Azure Private Link for database
- Configure VNet integration
- Enable firewall rules

### 3. Application Security
- Enable HTTPS only
- Configure CORS properly
- Use strong JWT keys

---

## Support

### Azure Support
- **Free Tier**: Community support
- **Basic Support**: $29/month
- **Developer Support**: $100/month

### Documentation
- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Azure Database for MySQL](https://docs.microsoft.com/en-us/azure/mysql/)
- [ASP.NET Core on Azure](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/)

---

## Next Steps

1. **Set up CI/CD pipeline** with GitHub Actions
2. **Configure monitoring** with Application Insights
3. **Set up backup** for database
4. **Configure custom domain** and SSL
5. **Set up staging environment**

---

**Your app will be available at:**
`https://29asset-app.azurewebsites.net`

**Database connection:**
`29asset-mysql.mysql.database.azure.com` 