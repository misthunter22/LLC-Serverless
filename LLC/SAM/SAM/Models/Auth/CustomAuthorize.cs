using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SAM.DI;

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
                context.Result = new JsonResult("error authorizing API");
            }
        }
    }
}
