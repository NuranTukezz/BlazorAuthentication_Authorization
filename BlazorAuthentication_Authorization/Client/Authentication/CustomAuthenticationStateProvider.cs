using BlazorAuthentication_Authorization.Client.Extensions;
using BlazorAuthentication_Authorization.Shared;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorAuthentication_Authorization.Client.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage; //AuthenticationStateProvider'ın içinden geliyor

        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSession = await _sessionStorage.ReadEncryptItemAsync<UserSession>("UserSession");
                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                }
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name,userSession.UserName),
                    new Claim(ClaimTypes.Role,userSession.Role)

                }, "JwtAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));

            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }



        public async Task UpdateAuthenticationState(UserSession? userSession)//kullanıcı oturum açtığında kapattığında kullanılacak
        {

            ClaimsPrincipal claimsPrincipal;


            if (userSession != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role)
                }));

                userSession.ExpiryTimeStamp = DateTime.Now.AddSeconds(userSession.ExpiresIn);//süre için doluğ dolmadığını kontrol eder
                await _sessionStorage.SaveItemEncryptAsync("UserSession", userSession);

            }
            else
            {
                claimsPrincipal = _anonymous;
                await _sessionStorage.RemoveItemAsync("UserSession");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));//kimlik doğrulama durumunu iletiyoruz
        }

        public async Task<string> GetToken()
        {
            var result = string.Empty;
            try
            {
                var userSession = await _sessionStorage.ReadEncryptItemAsync<UserSession>("UserSession");
                if (userSession != null && DateTime.Now < userSession.ExpiryTimeStamp)
                {
                    result = userSession.Token;
                }
            }
            catch
            {
            }
            return result;
        }
    }
}
