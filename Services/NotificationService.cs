using AMS.Models;
using AMS.Models.AssetRequestViewModel;
using AMS.Models.AssetViewModel;
using AMS.Services;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace AMS.Services
{
    public interface INotificationService
    {
        Task SendAssetRequestNotification(AssetRequestCRUDViewModel request, string adminEmail);
        Task SendAssetUpdateNotification(AssetCRUDViewModel asset, string userEmail);
        Task SendNewAssetNotification(AssetCRUDViewModel asset, List<string> adminEmails);
        Task SendMaintenanceReminder(AssetCRUDViewModel asset, string userEmail);
        Task SendApprovalNotification(AssetRequestCRUDViewModel request, string userEmail, bool isApproved);
    }

    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommon _iCommon;

        public NotificationService(IEmailSender emailSender, UserManager<ApplicationUser> userManager, ICommon iCommon)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _iCommon = iCommon;
        }

        public async Task SendAssetRequestNotification(AssetRequestCRUDViewModel request, string adminEmail)
        {
            var subject = "🔔 New Asset Request - 29 Gem & Jewellery";
            var message = BuildAssetRequestEmail(request);
            
            await _emailSender.SendEmailAsync(adminEmail, subject, message);
        }

        public async Task SendAssetUpdateNotification(AssetCRUDViewModel asset, string userEmail)
        {
            var subject = "📝 Asset Updated - 29 Gem & Jewellery";
            var message = BuildAssetUpdateEmail(asset);
            
            await _emailSender.SendEmailAsync(userEmail, subject, message);
        }

        public async Task SendNewAssetNotification(AssetCRUDViewModel asset, List<string> adminEmails)
        {
            var subject = "✨ New Asset Added - 29 Gem & Jewellery";
            var message = BuildNewAssetEmail(asset);
            
            foreach (var email in adminEmails)
            {
                await _emailSender.SendEmailAsync(email, subject, message);
            }
        }

        public async Task SendMaintenanceReminder(AssetCRUDViewModel asset, string userEmail)
        {
            var subject = "🔧 Maintenance Reminder - 29 Gem & Jewellery";
            var message = BuildMaintenanceReminderEmail(asset);
            
            await _emailSender.SendEmailAsync(userEmail, subject, message);
        }

        public async Task SendApprovalNotification(AssetRequestCRUDViewModel request, string userEmail, bool isApproved)
        {
            var status = isApproved ? "✅ Approved" : "❌ Rejected";
            var subject = $"Asset Request {status} - 29 Gem & Jewellery";
            var message = BuildApprovalEmail(request, isApproved);
            
            await _emailSender.SendEmailAsync(userEmail, subject, message);
        }

        private string BuildAssetRequestEmail(AssetRequestCRUDViewModel request)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");
            sb.AppendLine("<div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine("<h2>🎯 New Asset Request</h2>");
            sb.AppendLine("<p>29 Gem & Jewellery Asset Management System</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; background: #f8f9fa;'>");
            sb.AppendLine($"<h3>Request Details:</h3>");
            sb.AppendLine($"<p><strong>Asset ID:</strong> {request.AssetId}</p>");
            sb.AppendLine($"<p><strong>Requested By:</strong> {request.RequestedEmployeeDisplay}</p>");
            sb.AppendLine($"<p><strong>Request Date:</strong> {request.RequestDate:dd/MM/yyyy}</p>");
            sb.AppendLine($"<p><strong>Status:</strong> {request.Status}</p>");
            sb.AppendLine($"<p><strong>Details:</strong> {request.RequestDetails}</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; text-align: center;'>");
            sb.AppendLine("<a href='https://localhost:5001/AssetRequest' style='background: #667eea; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Request</a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            
            return sb.ToString();
        }

        private string BuildAssetUpdateEmail(AssetCRUDViewModel asset)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");
            sb.AppendLine("<div style='background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine("<h2>📝 Asset Updated</h2>");
            sb.AppendLine("<p>29 Gem & Jewellery Asset Management System</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; background: #f8f9fa;'>");
            sb.AppendLine($"<h3>Asset Details:</h3>");
            sb.AppendLine($"<p><strong>Asset ID:</strong> {asset.AssetId}</p>");
            sb.AppendLine($"<p><strong>Asset Name:</strong> {asset.Name}</p>");
            sb.AppendLine($"<p><strong>Category:</strong> {asset.CategoryDisplay}</p>");
            sb.AppendLine($"<p><strong>Status:</strong> {asset.AssetStatusDisplay}</p>");
            sb.AppendLine($"<p><strong>Last Updated:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; text-align: center;'>");
            sb.AppendLine("<a href='https://localhost:5001/Asset' style='background: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Asset</a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            
            return sb.ToString();
        }

        private string BuildNewAssetEmail(AssetCRUDViewModel asset)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");
            sb.AppendLine("<div style='background: linear-gradient(135deg, #ffc107 0%, #fd7e14 100%); color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine("<h2>✨ New Asset Added</h2>");
            sb.AppendLine("<p>29 Gem & Jewellery Asset Management System</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; background: #f8f9fa;'>");
            sb.AppendLine($"<h3>New Asset Details:</h3>");
            sb.AppendLine($"<p><strong>Asset ID:</strong> {asset.AssetId}</p>");
            sb.AppendLine($"<p><strong>Asset Name:</strong> {asset.Name}</p>");
            sb.AppendLine($"<p><strong>Category:</strong> {asset.CategoryDisplay}</p>");
            sb.AppendLine($"<p><strong>Model:</strong> {asset.AssetModelNo}</p>");
            sb.AppendLine($"<p><strong>Unit Price:</strong> {asset.UnitPrice:N0} MMK</p>");
            sb.AppendLine($"<p><strong>Purchase Date:</strong> {asset.DateOfPurchase:dd/MM/yyyy}</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; text-align: center;'>");
            sb.AppendLine("<a href='https://localhost:5001/Asset' style='background: #ffc107; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Asset</a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            
            return sb.ToString();
        }

        private string BuildMaintenanceReminderEmail(AssetCRUDViewModel asset)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");
            sb.AppendLine("<div style='background: linear-gradient(135deg, #dc3545 0%, #fd7e14 100%); color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine("<h2>🔧 Maintenance Reminder</h2>");
            sb.AppendLine("<p>29 Gem & Jewellery Asset Management System</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; background: #f8f9fa;'>");
            sb.AppendLine($"<h3>Asset Requiring Maintenance:</h3>");
            sb.AppendLine($"<p><strong>Asset ID:</strong> {asset.AssetId}</p>");
            sb.AppendLine($"<p><strong>Asset Name:</strong> {asset.Name}</p>");
            sb.AppendLine($"<p><strong>Category:</strong> {asset.CategoryDisplay}</p>");
            sb.AppendLine($"<p><strong>Last Maintenance:</strong> {asset.DateOfPurchase:dd/MM/yyyy}</p>");
            sb.AppendLine("<p><strong>Action Required:</strong> Schedule maintenance service</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; text-align: center;'>");
            sb.AppendLine("<a href='https://localhost:5001/Asset' style='background: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Asset</a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            
            return sb.ToString();
        }

        private string BuildApprovalEmail(AssetRequestCRUDViewModel request, bool isApproved)
        {
            var status = isApproved ? "Approved" : "Rejected";
            var color = isApproved ? "#28a745" : "#dc3545";
            var icon = isApproved ? "✅" : "❌";
            
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");
            sb.AppendLine($"<div style='background: linear-gradient(135deg, {color} 0%, #6c757d 100%); color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine($"<h2>{icon} Asset Request {status}</h2>");
            sb.AppendLine("<p>29 Gem & Jewellery Asset Management System</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; background: #f8f9fa;'>");
            sb.AppendLine($"<h3>Request Details:</h3>");
            sb.AppendLine($"<p><strong>Asset ID:</strong> {request.AssetId}</p>");
            sb.AppendLine($"<p><strong>Request Date:</strong> {request.RequestDate:dd/MM/yyyy}</p>");
            sb.AppendLine($"<p><strong>Status:</strong> <span style='color: {color}; font-weight: bold;'>{status}</span></p>");
            if (!isApproved)
            {
                sb.AppendLine($"<p><strong>Reason:</strong> {request.RequestDetails}</p>");
            }
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='padding: 20px; text-align: center;'>");
            sb.AppendLine("<a href='https://localhost:5001/AssetRequest' style='background: #6c757d; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Requests</a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            
            return sb.ToString();
        }
    }
} 