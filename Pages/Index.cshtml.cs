using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MELI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MELI.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILoggerService _loggerService;

        public IndexModel(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public ActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("Login");
            }
            else
            {
                return Page();
            }
        }
    }
}
