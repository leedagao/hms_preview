using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace learn
{
    class HttpUtil
    {

        public static bool SendHttp(out JToken outObj)
        {
            outObj = default(JToken);

            HttpWebRequest request = WebRequest.CreateHttp("http://127.0.0.1:3000");
            request.Method = "GET";

            // 使用using并不会捕获异常，一旦出现异常，程序退出！

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            string responseBody = reader.ReadToEnd();

                            Console.WriteLine(responseBody);

                            outObj = JToken.Parse(responseBody);

                            return true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }


        public async static void SendHttpAsync()
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://127.0.0.1:3000");
            request.Method = "GET";

            Task<WebResponse> task = request.GetResponseAsync();
            // 任务已经开始了，所以不要调用 Start
            // 否则 Start may not be called on a promise-style task.
            // task.Start();  
            await task;

            // 使用using并不会捕获异常，一旦出现异常，程序退出！
            using (WebResponse response = task.Result)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string responseBody = reader.ReadToEnd();

                        Console.WriteLine(responseBody);
                    }
                }
            }

        }

    }
}
