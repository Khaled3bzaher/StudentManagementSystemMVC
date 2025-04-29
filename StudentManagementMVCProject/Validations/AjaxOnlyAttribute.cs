using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentManagementMVCProject.Validations
{
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
