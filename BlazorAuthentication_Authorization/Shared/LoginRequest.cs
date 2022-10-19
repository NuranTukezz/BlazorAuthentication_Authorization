namespace BlazorAuthentication_Authorization.Shared
{
    public class LoginRequest
    {
        // 8- KULLANICININ OTURUM AÇMAYA ÇALIŞTIĞINDA SUNUCUYA İSTEK GÖNDERMEK İÇİN KULLANILACAK
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
