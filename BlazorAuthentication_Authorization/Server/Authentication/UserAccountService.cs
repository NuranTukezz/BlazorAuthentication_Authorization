namespace BlazorAuthentication_Authorization.Server.Authentication
{
    public class UserAccountService
    {
        private List<UserAccount> _userAccountList;

        public UserAccountService()
        {
            _userAccountList = new List<UserAccount>
            {
                new UserAccount{UserName="admin",Password="admin",Role="Adminisrator"},
                new UserAccount{UserName="user",Password="user",Role="User"}
            };
        }

        public UserAccount? GetUserAccouontByUserName(string userName)
        {
            return _userAccountList.FirstOrDefault(x => x.UserName == userName);//özel değişkende filtreleme yapıyoruz
        }
    }
}
