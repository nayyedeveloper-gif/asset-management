# 🚀 Railway Deployment Guide - AMS

## Quick Start (5 Minutes)

### 1. GitHub ကို Push လုပ်ပါ
```bash
git add .
git commit -m "🚀 Production ready for Railway"
git push origin main
```

### 2. Railway မှာ Deploy လုပ်ပါ
1. [railway.app](https://railway.app) သွားပါ
2. GitHub နဲ့ login လုပ်ပါ
3. "Deploy from GitHub repo" ကို နှိပ်ပါ
4. AMS repository ကို ရွေးပါ
5. **App Name ကို `29asset` ထားပါ** (အကြံပြုထားတာ)

### 3. MySQL Database ထည့်ပါ
1. Railway dashboard မှာ "New" ကို နှိပ်ပါ
2. "Database" → "MySQL" ရွေးပါ
3. Database connection details ကို မှတ်ထားပါ

### 4. Environment Variables ထည့်ပါ
Railway dashboard မှာ "Variables" tab သွားပြီး အောက်ပါ variables တွေ ထည့်ပါ:

```env
# Required Variables
ASPNETCORE_ENVIRONMENT=Production
DB_HOST=your-mysql-host.railway.app
DB_NAME=AMS
DB_USER=your-db-user
DB_PASSWORD=your-db-password
DB_PORT=3306
JWT_SECRET=your-very-long-random-secret-key-here
# Note: Set to your preferred subdomain
JWT_ISSUER=https://29asset.railway.app
JWT_AUDIENCE=https://29asset.railway.app
ADMIN_PASSWORD=your-strong-admin-password

# Optional Email Variables
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-password
```

### 5. Deploy လုပ်ပါ
Railway က automatically deploy လုပ်ပေးပါလိမ့်မယ်။

### 6. Test လုပ်ပါ
1. Railway dashboard မှာ app URL ကို နှိပ်ပါ
2. Login လုပ်ပါ (admin@yourcompany.com / your-admin-password)
3. Features တွေ test လုပ်ပါ

## 🔧 Troubleshooting

### Database Connection Error
- DB_HOST, DB_USER, DB_PASSWORD မှန်မမှန် စစ်ပါ
- Railway MySQL service က running ဖြစ်မဖြစ် စစ်ပါ

### JWT Error
- JWT_SECRET က အနည်းဆုံး 256 bits (32 characters) ရှိရပါမယ်
- JWT_ISSUER နဲ့ JWT_AUDIENCE ကို ဗလာ ထားပါ (Railway က free subdomain ပေးပါလိမ့်မယ်)

### Build Failed
- Dockerfile မှာ syntax error ရှိမရှိ စစ်ပါ
- .NET dependencies တွေ ပြည့်စုံမပြည့်စုံ စစ်ပါ

## 📞 Support
- Railway Docs: https://docs.railway.app/
- .NET on Railway: https://docs.railway.app/deploy/deployments/dockerfile

## 🌐 Railway Free Subdomain

Railway က free subdomain ပေးပါတယ်။ ဥပမာ:
- `https://29asset.railway.app` (အကြံပြုထားတာ)
- `https://asset-management.railway.app`
- `https://ams-system.railway.app`

### Subdomain ရွေးချယ်ခြင်း
1. **29asset** - Short နဲ့ memorable (အကြံပြုထားတာ)
2. **asset-management** - Professional နဲ့ clear
3. **ams-system** - Technical နဲ့ descriptive

Domain မရှိပဲ သုံးနိုင်ပါတယ်။ နောက်မှ domain ဝယ်ရင် Railway မှာ custom domain ထည့်နိုင်ပါတယ်။

## 🎉 Success!
Your AMS application is now live on Railway! 🚀 