using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SAM.DI;
using System;

namespace SAM.Models.Auth
{
    public class CustomAuthorize : ActionFilterAttribute, IAuthorizationFilter
    {
        public string Access { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var service = new ILLCDataImpl();
            var user = service.User(context.HttpContext.User.Claims);
            if (!user.ContainsRole(Access))
            {
                Console.WriteLine($"{Access} is not contained in {user.ToRoleList()}");
                context.Result = new JsonResult("error authorizing API");
            }
        }
    }
}
