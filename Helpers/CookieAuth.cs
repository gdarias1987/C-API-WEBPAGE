using MELI.Models;
using MELI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MELI.Helpers
{
    public class CookieAuth
    {
        private readonly HttpContext _context;

        public CookieAuth(HttpContext context)
        {
            _context = context;
        }

        public async Task GenerateCookie(Usuario usuario)
        {

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,usuario.usuario),
                    new Claim(ClaimTypes.PrimarySid, usuario.idUsuario.ToString())
                };

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
                    IsPersistent = false
                };

                var userIdentity = new ClaimsIdentity(claims, "User");
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                await _context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userIdentity), authProperties);
        }

        public async Task LogoutCookie()
        {
            await _context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
