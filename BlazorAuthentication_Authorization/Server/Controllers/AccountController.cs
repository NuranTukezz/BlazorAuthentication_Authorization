using BlazorAuthentication_Authorization.Server.Authentication;
using BlazorAuthentication_Authorization.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuthentication_Authorization.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserAccountService _userAccountService;

        public AccountController(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }


        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public ActionResult<UserSession> Login([FromBody] LoginRequest loginRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);//JwtAuthenticationManager nesnesi oluşturalım

            var userSession = jwtAuthenticationManager.GenerateJwtToken(loginRequest.UserName, loginRequest.Password);//username ve password'u kullanarak token yöntemini çagırıyoruz

            if (userSession is null)
            {
                return Unauthorized();//yetkisiz
            }
            else
            {
                return userSession;
            }

        }
    }
}
