using ADMIN.Models.OidcModel;

namespace ADMIN.Models.DTO
{
    public class UserDTO
    {
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public string? Site { get; set; }
        //public string? ProjectCode { get; set; }
        public List<string> SitesCollection { get; set; } = null;
        public UserAllocationClaimVm UserAllocationInfors { get; set; } = null;
        public string? TenantId { get; set; }
        public string? AccessToken { get; set; }
        public string? Identityoken { get; set; }
        public UserInforVm UserInfor { get; set; } = null;
        public List<string> Permissions { get; set; } = null;
    }


    public class UserInforVm
    {
        public string Id { get; set; }

        public string? AvataPath { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool IsNeedChangePassword { get; set; }

        public bool? IsAdmin { get; set; }

        #region Claim common

        public string? Name { get; set; }

        public string? GivenName { get; set; }

        public string? FamilyName { get; set; }
        #endregion
    }
}
