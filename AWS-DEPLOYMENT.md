# AWS Deployment Guide for 29 Asset Management System

## Prerequisites
- AWS Account (Free Tier available)
- AWS CLI installed and configured
- Docker installed locally
- Git repository ready

## Option 1: AWS Elastic Beanstalk (Recommended)

### Step 1: Prepare Application
```bash
# Build the application
dotnet publish -c Release -o ./publish

# Create deployment package
cd publish
zip -r ../ams-deployment.zip .
cd ..
```

### Step 2: Create Elastic Beanstalk Environment

1. **AWS Console** → **Elastic Beanstalk** → **Create Application**
2. **Application name**: `29-asset-management`
3. **Platform**: `.NET Core on Linux`
4. **Platform branch**: `.NET Core running on 64bit Amazon Linux 2`
5. **Platform version**: Latest
6. **Application code**: Upload your code
7. **Environment type**: Single instance (free tier)

### Step 3: Configure Environment Variables
```
ASPNETCORE_ENVIRONMENT=Production
DB_HOST=your-rds-endpoint
DB_NAME=AMS
DB_USER=ams_user
DB_PASSWORD=your_secure_password
DB_PORT=3306
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_app_password
FROM_EMAIL=your_email@gmail.com
JWT_SECRET_KEY=your_jwt_secret_key
```

### Step 4: Create RDS MySQL Database
1. **AWS Console** → **RDS** → **Create database**
2. **Engine type**: MySQL
3. **Template**: Free tier
4. **DB instance identifier**: `ams-database`
5. **Master username**: `ams_user`
6. **Master password**: `your_secure_password`
7. **Public access**: Yes (for development)
8. **VPC security group**: Create new

## Option 2: AWS EC2 with Docker

### Step 1: Launch EC2 Instance
1. **AWS Console** → **EC2** → **Launch Instance**
2. **AMI**: Amazon Linux 2023
3. **Instance type**: t2.micro (free tier)
4. **Security Group**: Allow ports 22, 80, 443, 3306
5. **Key pair**: Create or select existing

### Step 2: Connect and Setup
```bash
# Connect to EC2
ssh -i your-key.pem ec2-user@your-ec2-ip

# Update system
sudo yum update -y

# Install Docker
sudo yum install -y docker
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -a -G docker ec2-user

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Logout and login again
exit
ssh -i your-key.pem ec2-user@your-ec2-ip
```

### Step 3: Deploy Application
```bash
# Clone repository
git clone https://github.com/your-username/your-repo.git
cd your-repo

# Create .env file
cat > .env << EOF
DB_HOST=your-rds-endpoint
DB_NAME=AMS
DB_USER=ams_user
DB_PASSWORD=your_secure_password
DB_PORT=3306
DB_ROOT_PASSWORD=your_root_password
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_app_password
FROM_EMAIL=your_email@gmail.com
JWT_SECRET_KEY=your_jwt_secret_key
EOF

# Deploy with Docker Compose
docker-compose up -d
```

## Option 3: AWS App Runner

### Step 1: Prepare Container Image
```bash
# Build and push to ECR
aws ecr create-repository --repository-name 29-asset-management

# Login to ECR
aws ecr get-login-password --region your-region | docker login --username AWS --password-stdin your-account-id.dkr.ecr.your-region.amazonaws.com

# Build and tag image
docker build -t 29-asset-management .
docker tag 29-asset-management:latest your-account-id.dkr.ecr.your-region.amazonaws.com/29-asset-management:latest

# Push to ECR
docker push your-account-id.dkr.ecr.your-region.amazonaws.com/29-asset-management:latest
```

### Step 2: Create App Runner Service
1. **AWS Console** → **App Runner** → **Create service**
2. **Source**: Container registry
3. **Provider**: Amazon ECR
4. **Container image**: Select your image
5. **Port**: 80
6. **Environment variables**: Add all required variables

## Database Setup

### Create MySQL Database
```sql
-- Connect to your RDS instance
mysql -h your-rds-endpoint -u ams_user -p

-- Create database
CREATE DATABASE IF NOT EXISTS AMS CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Use database
USE AMS;

-- Run migrations (if using EF Core)
-- dotnet ef database update
```

### Initialize Sample Data
```sql
-- Insert sample data
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
VALUES ('1', 'Super Admin', 'SUPER ADMIN', NEWID());

INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount) 
VALUES ('1', 'admin@29asset.com', 'ADMIN@29ASSET.COM', 'admin@29asset.com', 'ADMIN@29ASSET.COM', 1, 'AQAAAAEAACcQAAAAELbX...', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', NEWID(), NULL, 0, 0, NULL, 1, 0);

INSERT INTO AspNetUserRoles (UserId, RoleId) 
VALUES ('1', '1');
```

## Environment Variables

### Required Variables
```bash
# Database
DB_HOST=your-rds-endpoint.amazonaws.com
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

### Gmail App Password Setup
1. Enable 2-factor authentication on Gmail
2. Generate App Password: Google Account → Security → App passwords
3. Use the generated password in SMTP_PASSWORD

## Security Configuration

### Security Groups
- **RDS**: Allow MySQL (3306) from EC2/EB security group
- **EC2/EB**: Allow HTTP (80), HTTPS (443), SSH (22)

### SSL Certificate (Optional)
```bash
# Install Certbot for Let's Encrypt
sudo yum install -y certbot

# Get SSL certificate
sudo certbot certonly --standalone -d your-domain.com

# Configure nginx with SSL
```

## Monitoring and Logs

### CloudWatch Setup
```bash
# Install CloudWatch agent
sudo yum install -y amazon-cloudwatch-agent

# Configure monitoring
sudo /opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-config-wizard
```

### Health Checks
- Application health endpoint: `/health`
- Database connectivity check
- Email service verification

## Cost Optimization

### Free Tier Usage
- **EC2**: 750 hours/month (t2.micro)
- **RDS**: 750 hours/month (db.t3.micro)
- **Elastic IP**: 1 free per month
- **Data Transfer**: 15 GB/month

### Cost Monitoring
- Set up AWS Budgets
- Monitor CloudWatch metrics
- Use AWS Cost Explorer

## Troubleshooting

### Common Issues
1. **Database Connection**: Check security groups and credentials
2. **Email Not Working**: Verify Gmail app password
3. **Port Conflicts**: Ensure correct port mapping
4. **Memory Issues**: Upgrade instance type if needed

### Logs Location
- **Application**: `/var/log/ams/`
- **Docker**: `docker logs container-name`
- **CloudWatch**: AWS Console → CloudWatch → Logs

## Backup Strategy

### Database Backup
```bash
# Automated RDS snapshots
# Manual backup
mysqldump -h your-rds-endpoint -u ams_user -p AMS > backup.sql

# Restore
mysql -h your-rds-endpoint -u ams_user -p AMS < backup.sql
```

### Application Backup
- Use Git for code versioning
- Backup uploads directory
- Export configuration files

## Support and Maintenance

### Regular Tasks
- Monitor application performance
- Update security patches
- Review CloudWatch metrics
- Backup database regularly
- Check SSL certificate expiration

### Contact Information
- **Developer**: Nay Ye
- **Application**: 29 Asset Management System
- **Version**: 2.0.0

---

**Note**: This guide assumes basic AWS knowledge. For production deployments, consider additional security measures, monitoring, and backup strategies. 