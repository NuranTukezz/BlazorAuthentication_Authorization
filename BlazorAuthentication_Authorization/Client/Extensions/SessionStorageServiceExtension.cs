using Blazored.SessionStorage;
using System.Text.Json;
using System.Text;

namespace BlazorAuthentication_Authorization.Client.Extensions
{
    public static class SessionStorageServiceExtension
    {
        public static async Task SaveItemEncryptAsync<T>(this ISessionStorageService sessionStorageService, string key, T item)
        {
            var itemJson = JsonSerializer.Serialize(item);//JsonSerializerkullanılarak json dizesi oluşturuluyor
            var itemJsonBytes = Encoding.UTF8.GetBytes(itemJson);//json dizesini bayt'a dönüştür 
            var base64Json = Convert.ToBase64String(itemJsonBytes);//base64 stringi oluştu 

            await sessionStorageService.SetItemAsync(key, base64Json);
        }


        public static async Task<T> ReadEncryptItemAsync<T>(this ISessionStorageService sessionStorageService, string key)
        {
            var base64Json = await sessionStorageService.GetItemAsync<string>(key);//base64stringi oturum deposundan okuyacağız

            var itemJsonBytes = Convert.FromBase64String(base64Json);//bayt'a çevireceğiz

            var itemJson = Encoding.UTF8.GetString(itemJsonBytes);//baytlardan json dizesini okuyoruz

            var item = JsonSerializer.Deserialize<T>(itemJson);//json serleştiricisi kullanarak json dizesini seri hale getirme ve nesneye döndürme işlemi

            return item;
        }
    }
}
