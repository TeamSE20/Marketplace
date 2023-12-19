using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAppCourse2022.Controllers
{
    public class AccessController : Controller
    {
        [AllowAnonymous]
        public IActionResult AllAccess()
        {
            return View();
        }

        [Authorize]
        public IActionResult AuthorizedAccess()
        {
            return View();
        }

        [Authorize(Roles = "Client")]
        public IActionResult UserAccess()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeAccess()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminAccess()
        {
            return View();
        }


        [Authorize(Policy = "FirstNameAuth")]
        public IActionResult FirstNameAuth()
        {
            return View();
        }
    }
}
