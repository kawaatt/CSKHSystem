namespace ADMIN.Models.ViewModel.AplicationVm
{
    public class PlayerBaseVm
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string UserCode { get; set; }
        public string DisplayName { get; set; }
        public string? Avatar { get; set; }
        public string? Cover { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Followers { get; set; }
        public int? Hot { get; set; }
        public int? Diamond { get; set; }
        public string Password { get; set; }
        public int lockType { get; set; }
        public string reason { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class PlayerDtoVm : PlayerBaseVm
    {

    }

    public class PlayerVm : PlayerBaseVm
    {
    }
    public class InputFieldPlayer
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string UserCode { get; set; }
        public string DisplayName { get; set; }
        public string? Avatar { get; set; }
        public string? Cover { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Followers { get; set; }
        public int? Hot { get; set; }
        public int? Diamond { get; set; }
        public string Password { get; set; }
        public int lockType { get; set; }
        public string reason { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class PlayerAddOrEdit : InputFieldLiveStream
    {
        public string? Id { get; set; }
        public string? Password { get; set; }
        public int lockType { get; set; }
        public string reason { get; set; }
    }
}
