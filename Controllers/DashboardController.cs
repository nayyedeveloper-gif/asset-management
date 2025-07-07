using AMS.ConHelper;
using AMS.Data;
using AMS.Models;
using AMS.Models.CommonViewModel;
using AMS.Models.DashboardViewModel;
using AMS.Models.ManageUserRolesVM;
using AMS.Models.UserProfileViewModel;
using AMS.Pages;
using AMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace AMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IRoles _roles;
        public DashboardController(ApplicationDbContext context, ICommon iCommon, IRoles roles)
        {
            _context = context;
            _iCommon = iCommon;
            _roles = roles;
        }

        [Authorize(Roles = Pages.RoleViewModel.Dashboard)]
        public IActionResult Index()
        {
            try
            {
                // Ensure all roles are created in database
                _roles.GenerateRolesFromPageList().Wait();
                
                // Assign General role to users who don't have Super Admin role
                var usersWithoutGeneralRole = _context.UserProfile.Where(u => u.RoleId != 2).ToList();
                foreach (var user in usersWithoutGeneralRole)
                {
                    user.RoleId = 2; // General role
                    _context.UserProfile.Update(user);
                }
                _context.SaveChanges();
                
                // Ensure ManageUserRoles entries exist for General role
                var generalManageRole = _context.ManageUserRoles.FirstOrDefault(r => r.Name == "General");
                if (generalManageRole == null)
                {
                    generalManageRole = new ManageUserRoles
                    {
                        Name = "General",
                        Description = "User Role: General",
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CreatedBy = "System",
                        ModifiedBy = "System"
                    };
                    _context.ManageUserRoles.Add(generalManageRole);
                    _context.SaveChanges();
                }
                
                // Create ManageUserRolesDetails for General role if they don't exist
                var generalRoleDetails = _context.ManageUserRolesDetails.Where(d => d.ManageRoleId == generalManageRole.Id).ToList();
                if (!generalRoleDetails.Any())
                {
                    var allRoles = _context.Roles.ToList(); // Get all Identity roles
                    foreach (var role in allRoles)
                    {
                        var roleDetail = new ManageUserRolesDetails
                        {
                            ManageRoleId = generalManageRole.Id,
                            RoleId = role.Id,
                            RoleName = role.Name,
                            IsAllowed = role.Name == "Dashboard" || role.Name == "User Profile", // Basic permissions for General users
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            CreatedBy = "System",
                            ModifiedBy = "System"
                        };
                        _context.ManageUserRolesDetails.Add(roleDetail);
                    }
                    _context.SaveChanges();
                }
                
                DashboardSummaryViewModel vm = new DashboardSummaryViewModel();
                var _UserProfile = _context.UserProfile.ToList();
                var _Departments = _context.Department.ToList();
                var _IsInRole = User.IsInRole("Admin");

                vm.TotalUser = _UserProfile.Count();
                vm.TotalActive = _UserProfile.Where(x => x.Cancelled == false).Count();
                vm.TotalInActive = _UserProfile.Where(x => x.Cancelled == true).Count();
                vm.listUserProfile = _UserProfile.Where(x => x.Cancelled == false)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(10)
                    .Select(u => {
                        var dept = _Departments.FirstOrDefault(d => d.Id == u.Department);
                        var userVm = (AMS.Models.UserProfileViewModel.UserProfileCRUDViewModel)u;
                        userVm.DepartmentDisplay = dept != null ? dept.Name : "";
                        return userVm;
                    })
                    .ToList();

                var _Asset = _context.Asset.Where(x => x.Cancelled == false).ToList();
                vm.TotalAsset = _Asset.Count();
                vm.TotalAssignedAsset = _Asset.Where(x => x.AssignEmployeeId != 0).Count();
                vm.TotalUnAssignedAsset = _Asset.Where(x => x.AssignEmployeeId == 0).Count();
                vm.listAssetCRUDViewModel = _iCommon.GetGridAssetList(_IsInRole).Take(10).ToList();

                vm.TotalEmployee = _context.UserProfile.Where(x => x.Cancelled == false).Count();
                vm.TotalAssetRequest = _context.AssetRequest.Where(x => x.Cancelled == false).Count();
                vm.TotalIssue = _context.AssetIssue.Where(x => x.Cancelled == false).Count();

                return View(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetPieChartData()
        {
            var AssetGroupBy = _context.Asset.Where(x => x.Cancelled == false).GroupBy(p => p.AssetStatus).Select(g => new
            {
                Id = g.Key,
                AssetStatus = g.Count()
            }).ToList();

            var result = (from _AssetGroupBy in AssetGroupBy
                          join _AssetStatus in _context.AssetStatus on _AssetGroupBy.Id equals _AssetStatus.Id
                          select new PieChartViewModel
                          {
                              Name = _AssetStatus.Name,
                              Total = _AssetGroupBy.AssetStatus,
                          }).ToList();
            return new JsonResult(result.ToDictionary(x => x.Name, x => x.Total));
        }

        [HttpGet]
        public JsonResult GetAssetCategoryBreakdown()
        {
            var data = _context.Asset
                .Where(x => !x.Cancelled)
                .GroupBy(x => x.Category)
                .Select(g => new {
                    Category = _context.AssetCategorie.FirstOrDefault(c => c.Id == g.Key).Name,
                    Count = g.Count()
                }).ToList();
            return new JsonResult(data);
        }

        [HttpGet]
        public JsonResult GetAssetStatusBreakdown()
        {
            var data = _context.Asset
                .Where(x => !x.Cancelled)
                .GroupBy(x => x.AssetStatus)
                .Select(g => new {
                    Status = _context.AssetStatus.FirstOrDefault(s => s.Id == g.Key).Name,
                    Count = g.Count()
                }).ToList();
            return new JsonResult(data);
        }

        [HttpGet]
        public JsonResult GetAssetAllocationBreakdown()
        {
            var allocated = _context.Asset.Where(x => !x.Cancelled && x.AssignEmployeeId != 0).Count();
            var unallocated = _context.Asset.Where(x => !x.Cancelled && x.AssignEmployeeId == 0).Count();
            var data = new[] {
                new { Label = "Allocated Assets", Count = allocated },
                new { Label = "Unallocated Assets", Count = unallocated }
            };
            return new JsonResult(data);
        }

        [HttpGet]
        public JsonResult GetAssetIssuesBreakdown()
        {
            var newIssues = _context.AssetIssue.Where(x => !x.Cancelled && x.Status == "New").Count();
            var resolvedIssues = _context.AssetIssue.Where(x => !x.Cancelled && x.Status == "Resolved").Count();
            var underMaintenance = _context.AssetIssue.Where(x => !x.Cancelled && x.Status == "Under Maintenance").Count();
            
            var data = new[] {
                new { Label = "New Issues", Count = newIssues },
                new { Label = "Resolved Issues", Count = resolvedIssues },
                new { Label = "Under Maintenance", Count = underMaintenance }
            };
            return new JsonResult(data);
        }

        [HttpGet]
        public JsonResult GetAssetDistributionSummary()
        {
            var totalAssets = _context.Asset.Where(x => !x.Cancelled).Count();
            var allocatedAssets = _context.Asset.Where(x => !x.Cancelled && x.AssignEmployeeId != 0).Count();
            var unallocatedAssets = _context.Asset.Where(x => !x.Cancelled && x.AssignEmployeeId == 0).Count();
            var assetIssues = _context.AssetIssue.Where(x => !x.Cancelled).Count();
            var underMaintenance = _context.AssetIssue.Where(x => !x.Cancelled && x.Status == "Under Maintenance").Count();

            var data = new {
                TotalAssets = totalAssets,
                AllocatedAssets = allocatedAssets,
                UnallocatedAssets = unallocatedAssets,
                AssetIssues = assetIssues,
                UnderMaintenance = underMaintenance
            };
            return new JsonResult(data);
        }
    }
}