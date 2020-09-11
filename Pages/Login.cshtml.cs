using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MELI.Helpers;
using MELI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MELI.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IDataService _dataService;
        
        public LoginModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void OnGet()
        {
        }
    }
}