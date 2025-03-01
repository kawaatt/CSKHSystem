namespace ADMIN.Models.ViewModel.AplicationVm
{
    public class EmployeeBaseVm
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public bool IsBlock { get; set; }
        public bool IsActive { get; set; }
        public string Site { get; set; }
        public string RoleName { get; set; }
    }
    public class EmployeeDtoVm : EmployeeBaseVm
    {

    }

    public class EmployeeVm : EmployeeBaseVm
    {
        public List<EmployeeSiteVm> EmployeeSite { get; set; }
    }

    public class InputFieldEmployee
    {
        public string FullName { get; set; }
    }

    public class EmployeeAddOrEdit : InputFieldEmployee
    {
        public string? Id { get; set; }
    }
}
