# Render Deployment - Quick Start Guide

## 🚀 100% Free Deployment (No Credit Card Required)

### Step 1: Create Render Account
1. Go to [render.com](https://render.com)
2. Click "Get Started for Free"
3. Sign up with GitHub account
4. **No credit card required!**

### Step 2: Deploy Your App

#### Option A: Auto Deploy (Recommended)
1. Click "New +" → "Web Service"
2. Connect your GitHub repository
3. Select your repository: `29-asset-management`
4. Choose branch: `main` or `master`
5. Click "Create Web Service"

#### Option B: Manual Deploy
1. Click "New +" → "Web Service"
2. Choose "Build and deploy from a Git repository"
3. Connect GitHub and select your repo
4. Configure:
   - **Name**: `29-asset-management`
   - **Environment**: `Dotnet`
   - **Build Command**: `dotnet build`
   - **Start Command**: `dotnet run --urls http://0.0.0.0:$PORT`

### Step 3: Create Database
1. Click "New +" → "PostgreSQL"
2. Configure:
   - **Name**: `ams-database`
   - **Plan**: Free
   - **Database**: `ams`
   - **User**: `ams_user`
3. Click "Create Database"

### Step 4: Link Database to App
1. Go to your web service
2. Click "Environment"
3. Add these variables:
   ```
   DB_HOST=your-db-host.onrender.com
   DB_NAME=ams
   DB_USER=ams_user
   DB_PASSWORD=your-db-password
   DB_PORT=5432
   ```

### Step 5: Add Email Settings
Add these environment variables:
```
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_app_password
FROM_EMAIL=your_email@gmail.com
```

### Step 6: Gmail Setup
1. Enable 2-factor authentication on Gmail
2. Go to Google Account → Security → App passwords
3. Generate app password for "Mail"
4. Use this password in `SMTP_PASSWORD`

## 🎯 Your App URL
After deployment, your app will be available at:
```
https://29-asset-management.onrender.com
```

## 📊 Free Tier Limits
- **Web Services**: 750 hours/month
- **Databases**: 90 days free trial
- **Bandwidth**: 100 GB/month
- **Build Time**: 500 minutes/month

## ⚡ Quick Commands

### Check Deployment Status
```bash
# Your app will be available at the URL shown in Render dashboard
```

### View Logs
1. Go to your web service in Render dashboard
2. Click "Logs" tab
3. Monitor deployment and runtime logs

### Update App
1. Push changes to GitHub
2. Render will automatically redeploy
3. Check logs for any issues

## 🔧 Troubleshooting

### Build Failures
- Check build logs in Render dashboard
- Ensure all dependencies are in `.csproj`
- Verify .NET version compatibility

### Database Connection
- Verify database environment variables
- Check database status in Render dashboard
- Ensure proper connection string format

### Email Issues
- Verify Gmail app password
- Check SMTP settings
- Test email configuration

## 🆘 Support
- **Render Docs**: [docs.render.com](https://docs.render.com)
- **Community**: Render Discord
- **Status**: [status.render.com](https://status.render.com)

## 💡 Tips
- Free tier services go to sleep after inactivity
- First request may take 30-60 seconds to wake up
- Monitor usage in dashboard to stay within limits
- Use sleep mode for development/testing

---

**🎉 Congratulations!** Your 29 Asset Management System is now live on Render for free! 