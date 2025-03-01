namespace TELEBOT_CSKH.Models.OidcModel
{
    public class UserAllocationClaimVm
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public string? SiteId { get; set; }
        public string? DepartmentId { get; set; }
        public string? AreaId { get; set; }
        public string? SiteName { get; set; }
        public string? DepartmentName { get; set; }
        public string? AreaName { get; set; }

        public virtual ICollection<AreaVm> AllocationAreas { get; set; } = new List<AreaVm>();

        public virtual ICollection<DepartmentVm> AllocationDepartments { get; set; } = new List<DepartmentVm>();

        public virtual ICollection<SiteVm> AllocationSites { get; set; } = new List<SiteVm>();
    }

    public partial class SiteVm
    {
        public string Id { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }
    }
    public partial class DepartmentVm
    {
        public string Id { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }
    }
    public partial class AreaVm
    {
        public string Id { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }
    }
}
