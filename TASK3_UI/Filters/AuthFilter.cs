using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TASK3_UI.Filters {
  public class AuthFilter : IActionFilter {
    public void OnActionExecuted(ActionExecutedContext context) {
      var controller = context.Controller as ControllerBase;
      if (context.HttpContext.Request.Cookies["token"] == null) {
        context.Result = controller.RedirectToAction(
          actionName: "Login",
          controllerName: "Account",
          new { returnUrl = context.HttpContext.Request.Path }
        );
      }
    }

    public void OnActionExecuting(ActionExecutingContext context) {
    }
  }
}
