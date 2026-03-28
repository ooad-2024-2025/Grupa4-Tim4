using Microsoft.AspNetCore.Mvc;

namespace eOpcina.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            if (statusCode == 404)
                return View("NotFound"); 

            return View("GenericError"); 
        }
    }

}
