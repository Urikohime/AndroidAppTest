using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AppTest
{
    static class Connector
    {
        private static bool loggedIn = false;
        private static readonly HttpClient client = new HttpClient();
        private static String URI = Secrets.apiUrl;
        private static String TOKEN = Secrets.apiToken;

        private static bool GET_Users(String user)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", TOKEN);
            try
            {
                _ = client.GetStringAsync(URI + user).Result;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void POST_Users(String name, String hashpass, String swcode, String email)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", TOKEN);
            var values = new List<KeyValuePair<String, String>>();
            values.Add(new KeyValuePair<String, String>("swcode", swcode));
            values.Add(new KeyValuePair<String, String>("username", name));
            values.Add(new KeyValuePair<String, String>("safekey", hashpass));
            values.Add(new KeyValuePair<String, String>("email", email));

            var content = new FormUrlEncodedContent(values);

            Console.WriteLine(content);
            _ = client.PostAsync(URI, content);
        }

        public static async Task Register(String name, String hashpass, String swcode, String email)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", TOKEN);
            if (!GET_Users(swcode))
            {
                POST_Users(name, hashpass, swcode, email);
                await Task.Delay(100);
            }
        }

        public static bool IsNameTaken(String name)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", TOKEN);
            try
            {
                String result = client.GetStringAsync(URI).Result.ToLower();
                if(result.Contains("\"username\":\"" + name.ToLower() + "\","))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsCodeTaken(String code)
        {
            if (!GET_Users(code))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsLoggedIn()
        {
            return loggedIn;
        }

        public static void SetLoginState(bool state)
        {
            loggedIn = state;
        }

        public static void Login()
        {
            if (!IsLoggedIn())
            {

                loggedIn = true;
            }
        }
    }
}