using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MELI.Helpers;
using MELI.Models;
using MELI.Services;
using MELI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MELI.Pages.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class user : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataService _dataService;
        private readonly ILoggerService _loggerService;

        public user(IConfiguration configuration, IDataService dataService, ILoggerService loggerService)
        {
            _configuration = configuration;
            _dataService = dataService;
            _loggerService = loggerService;
        }

        //[ControlAutorizacion]
        //[HttpPost("test")]
        //public async Task<ActionResult> TestAPI()
        //{
        //    return new JsonResult(new { message = "OK" }) { StatusCode = StatusCodes.Status200OK };
        //}

        //[HttpPost("test2")]
        //public async Task<ActionResult> TestAPI2()
        //{
        //    List<Checkpoint> listado = await _dataService.GetCheckpointAsync();
        //    return new JsonResult(new { message = listado }) { StatusCode = StatusCodes.Status200OK };
        //}

        //[ControlAutorizacion]
        [HttpPost("jwt")]
        public async Task<ActionResult> GenToken([FromBody]Usuario usuario)

        {
            if (!ModelState.IsValid)
            {
                return handleErr();
            }

            Usuario resultado = await _dataService.CheckUserLogin(usuario);

            if ( resultado != null )
            {
                return new JsonResult(new { test = GenerateJwtToken(resultado) });
            }
            else
            {
                _loggerService.recordLogError(_loggerService.GetLastMethodName(), "Unauthorized", usuario.usuario);
                return Unauthorized();
            }
        }

        [HttpPost("loginUser")]
        public async Task<ActionResult> LoginCookie([FromForm] Usuario logAttemp)
        {
            Usuario control = await _dataService.CheckUserLogin(logAttemp);
            
            
            if (control != null)
            {
                CookieAuth auth = new CookieAuth(HttpContext);
                await auth.GenerateCookie(logAttemp);
                return RedirectToPage("/Index");
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("logoutUser")]
        public async Task<ActionResult> LogoutCookie()
        {

            CookieAuth auth = new CookieAuth(HttpContext);
            await auth.LogoutCookie();

            return RedirectToPage("/Login");

        }

        public string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:s3cret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", usuario.idUsuario.ToString()), new Claim("usuario", usuario.usuario) }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public JsonResult handleErr()
        {
            List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
           .Where(y => y.Count > 0)
           .ToList();

            foreach (var error in errors)
            {
                //_log.recordLogError(_log.GetLastMethodName(), error[0].ErrorMessage, Usuario.userConnected.email);
            }

            var errores = new
            {
                error = errors
            };

            return new JsonResult(errores);
        }

    }
}
