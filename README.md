# 29 Asset Management System

A professional asset management system built with ASP.NET Core 9.0, Entity Framework Core, and MySQL.

## Features

- **Asset Management**: Track, assign, and manage company assets
- **User Management**: Role-based access control with Super Admin and General roles
- **Department Management**: Organize assets by departments and sub-departments
- **Asset Categories**: Categorize assets for better organization
- **Asset Requests**: Request and approve asset allocations
- **Asset Issues**: Report and track asset issues
- **Dashboard**: Comprehensive analytics and reporting
- **Email Notifications**: Automated email alerts for asset activities
- **Audit Logs**: Complete audit trail for all system activities

## Technology Stack

- **Backend**: ASP.NET Core 9.0
- **Database**: MySQL 8.0
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity with JWT
- **Frontend**: Razor Pages with Bootstrap 5
- **Email**: SMTP (Gmail support)
- **Deployment**: Docker, AWS, Azure

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- MySQL 8.0
- Docker (optional)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/29-asset-management.git
   cd 29-asset-management
   ```

2. **Configure database**
   ```bash
   # Update connection string in appsettings.Development.json
   # Create MySQL database
   mysql -u root -p
   CREATE DATABASE AMS CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```

3. **Run migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - URL: https://localhost:5001
   - Default admin: admin@29asset.com / Admin123!

### Docker Deployment

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

2. **Access the application**
   - URL: http://localhost

## AWS Deployment

### Option 1: Elastic Beanstalk (Recommended)
```bash
# Build deployment package
dotnet publish -c Release -o ./publish
cd publish && zip -r ../ams-deployment.zip . && cd ..

# Upload to AWS Elastic Beanstalk
# Follow AWS-DEPLOYMENT.md for detailed steps
```

### Option 2: EC2 with Docker
```bash
# Deploy to EC2 instance
ssh -i your-key.pem ec2-user@your-ec2-ip
git clone https://github.com/your-username/29-asset-management.git
cd 29-asset-management
docker-compose up -d
```

### Option 3: App Runner
```bash
# Build and push to ECR
docker build -t 29-asset-management .
docker tag 29-asset-management:latest your-account-id.dkr.ecr.region.amazonaws.com/29-asset-management:latest
docker push your-account-id.dkr.ecr.region.amazonaws.com/29-asset-management:latest
```

## Environment Variables

Create a `.env` file or set environment variables:

```bash
# Database
DB_HOST=your-database-host
DB_NAME=AMS
DB_USER=ams_user
DB_PASSWORD=your_secure_password
DB_PORT=3306

# Email (Gmail)
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_app_password
FROM_EMAIL=your_email@gmail.com

# JWT
JWT_SECRET_KEY=your_very_long_secret_key_here

# Application
ASPNETCORE_ENVIRONMENT=Production
```

## Database Schema

### Core Tables
- `AspNetUsers` - User accounts
- `AspNetRoles` - User roles
- `Asset` - Asset information
- `AssetAssigned` - Asset assignments
- `AssetRequest` - Asset requests
- `AssetIssue` - Asset issues
- `Department` - Departments
- `SubDepartment` - Sub-departments
- `AssetCategorie` - Asset categories
- `AssetSubCategorie` - Asset sub-categories

### Sample Data
The system includes sample data for:
- Admin user (admin@29asset.com)
- Sample departments and categories
- Sample assets and assignments

## Security Features

- **Authentication**: ASP.NET Core Identity
- **Authorization**: Role-based access control
- **JWT Tokens**: Secure API authentication
- **Password Policies**: Strong password requirements
- **Audit Logging**: Complete activity tracking
- **HTTPS**: Secure communication

## API Endpoints

### Authentication
- `POST /Account/Login` - User login
- `POST /Account/Logout` - User logout
- `POST /Account/Register` - User registration

### Assets
- `GET /Asset` - List assets
- `POST /Asset/Create` - Create asset
- `PUT /Asset/Edit/{id}` - Update asset
- `DELETE /Asset/Delete/{id}` - Delete asset

### Asset Requests
- `GET /AssetRequest` - List requests
- `POST /AssetRequest/Create` - Create request
- `PUT /AssetRequest/Approve/{id}` - Approve request

## Monitoring and Health Checks

- **Health Endpoint**: `/health`
- **Database Connectivity**: Automatic health checks
- **Application Metrics**: Built-in monitoring
- **Error Logging**: Comprehensive error tracking

## Backup and Recovery

### Database Backup
```bash
# Manual backup
mysqldump -h your-host -u your-user -p AMS > backup.sql

# Restore
mysql -h your-host -u your-user -p AMS < backup.sql
```

### Application Backup
- Git repository for code versioning
- Backup uploads directory
- Export configuration files

## Troubleshooting

### Common Issues

1. **Database Connection**
   - Verify connection string
   - Check MySQL service status
   - Ensure proper permissions

2. **Email Not Working**
   - Verify Gmail app password
   - Check SMTP settings
   - Enable 2-factor authentication

3. **Build Errors**
   - Ensure .NET 9.0 SDK installed
   - Clear obj/bin folders
   - Restore NuGet packages

### Logs
- **Application**: Check console output
- **Database**: MySQL error logs
- **Docker**: `docker logs container-name`

## Support

- **Developer**: Nay Ye
- **Application**: 29 Asset Management System
- **Version**: 2.0.0
- **Documentation**: See AWS-DEPLOYMENT.md for deployment details

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

---

**Note**: For production deployment, ensure proper security measures, SSL certificates, and monitoring are in place.
