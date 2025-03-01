using Microsoft.AspNetCore.Authorization;

namespace ADMIN.AuthHandler
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Lấy tất cả các claim permission của người dùng
            //var permissions = context.User.FindAll(c => c.Type == "permissions");
            var permissionsClaims = context.User.Claims
                            .Where(c => c.Type == "permissions")
                            .Select(c => c.Value)
                            .ToList();
            // Kiểm tra nếu người dùng có quyền tương ứng với permission yêu cầu
            if (permissionsClaims.Contains(requirement.Permission))
            {
                context.Succeed(requirement); // Nếu có quyền, thành công
            }

            return Task.CompletedTask;
        }
    }
}
