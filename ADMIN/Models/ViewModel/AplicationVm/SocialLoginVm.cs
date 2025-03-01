using System.ComponentModel.DataAnnotations;

namespace ADMIN.Models.ViewModel.AplicationVm
{
    public class SocialLoginBaseVm
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        public string AccessToken { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }

    }
    public class SocialLoginDtoVm : SocialLoginBaseVm
    {

    }

    public class SocialLoginVm : SocialLoginBaseVm
    {
    }
    public class InputFieldSocialLogin
    {
    }

    public class SocialLoginAddOrEdit : InputFieldSocialLogin
    {
        public string? Id { get; set; }
    }
}
