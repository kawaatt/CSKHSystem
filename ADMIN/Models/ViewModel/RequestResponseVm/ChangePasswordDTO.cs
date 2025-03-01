using ADMIN.Models.ViewModel.AplicationVm;

namespace ADMIN.Models.ViewModel.RequestResponseVm
{
    public class ChangePasswordDTO
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordDtoVm : ChangePasswordDTO
    {

    }

    public class ChangePasswordVm : ChangePasswordDTO
    {
    }
    public class InputFieldChangePassword
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordAddOrEdit : InputFieldChangePassword
    {
        public string? UserId { get; set; }
    }
}
