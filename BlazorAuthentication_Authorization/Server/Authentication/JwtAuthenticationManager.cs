using BlazorAuthentication_Authorization.Shared;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorAuthentication_Authorization.Server.Authentication
{
    public class JwtAuthenticationManager
    {
        public const string JWT_SECURITY_KEY = "yPkCqn4kSWLtaJwXvN2jGzpQRyTZ3gdXkt7FeBJP";//güvenlik anahtarı

        private const int JWT_TOKEN_VALIDITY_MINS = 20;//20dakika geçerli

        private UserAccountService _userAccountService;

        public JwtAuthenticationManager(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }



        public UserSession? GenerateJwtToken(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }


            /* Validating The User Credentials - Kullanıcı Kimlik Bilgilerini Doğrulama */

            var userAccount = _userAccountService.GetUserAccouontByUserName(userName);//kullanıcının adını alıyoruz

            if (userAccount == null || userAccount.Password != password)
            {
                return null;
            }


            /* Generating JWT Token - JWT Simgesi Oluşturma */

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);//20dk otantike süresi

            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);//security key' baytlarına ayırıyoruz

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name,userAccount.UserName),
                new Claim(ClaimTypes.Role,userAccount.Role)
            });//Username ve rol oluşturdum

            var singingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);//güvenlik algoritması

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = singingCredentials
            };//talep kimliğini sona erme zamanı ve kimlik bilgileri


            // --Token oluşturuluyor__//

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();//jwtsecurity işleyicisinin nesnesi oluşturuldu

            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);//güvenlik belirteci nesnesi jwtSecurityTokenHandler kullanılarak oluşturuluyor

            var token = jwtSecurityTokenHandler.WriteToken(securityToken);//oluşturulan güvenlik belirteci değişkene aktarılıyor

            // --Token oluşturuldu//


            /* Returing the User Session object - Kullanıcı Oturumu nesnesini döndürme */

            var userSession = new UserSession
            {
                UserName = userAccount.UserName,
                Role = userAccount.Role,
                Token = token,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
            };//kullanıcı nesnesi oluşturuldu

            return userSession;
        }
    }
}
