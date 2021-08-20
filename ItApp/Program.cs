
using Fleck;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ItApp
{
    class Program
    {

        #region 字符串和Byte之间的转化
        /// <summary>
        /// 数字和字节之间互转
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int IntToBitConverter(int num)
        {
            int temp = 0;
            byte[] bytes = BitConverter.GetBytes(num);//将int32转换为字节数组
            temp = BitConverter.ToInt32(bytes, 0);//将字节数组内容再转成int32类型
            return temp;
        }

        /// <summary>
        /// 将字符串转为16进制字符，允许中文
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StringToHexString(string s, Encoding encode, string spanString)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符
            {
                result += Convert.ToString(b[i], 16) + spanString;
            }
            return result;
        }
        /// <summary>
        /// 将16进制字符串转为字符串
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HexStringToString(string hs, Encoding encode)
        {
            string strTemp = "";
            byte[] b = new byte[hs.Length / 2];
            for (int i = 0; i < hs.Length / 2; i++)
            {
                strTemp = hs.Substring(i * 2, 2);
                b[i] = Convert.ToByte(strTemp, 16);
            }
            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }
        /// <summary>
        /// byte[]转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <returns></returns>
        public static string Get(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            if (req == null || req.GetResponse() == null)
                return string.Empty;

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            if (resp == null)
                return string.Empty;

            using (Stream stream = resp.GetResponseStream())
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static string Post(string url, string postData)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            if (req == null)
                return string.Empty;

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Timeout = 15000;

            byte[] data = Encoding.UTF8.GetBytes(postData);
            //注意：无需手动指定长度 (否则可能会报流未处理完就关闭的异常，因为ContentLength时候会比真实post数据长度大)
            //req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            if (resp == null)
                return string.Empty;

            using (Stream stream = resp.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string UnZip(string inputFile)
        {
            string outDir = System.Environment.CurrentDirectory + "\\executor\\";
            if (UnZip(inputFile, outDir))
                return outDir;
            else
                return "";
        }


        public static bool UnZip(string inputFile, string outputDir)
        {
            bool result = false;
            try
            {

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(inputFile)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        //string directoryName = outputDir;
                        string fileName = Path.GetFileName(theEntry.Name);

                        // create directory
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(outputDir + "/" + directoryName);
                        }

                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(outputDir + "/" + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                        streamWriter.Flush();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                result = true;
            }
            catch
            {
            }
            return result;
        }

        static void init()
        {
            Console.WriteLine("IT抢单神器首次启动，请先完成配置,输入 Q 退出.");
            var machineId = Machine.Value();
            //AEC8-3DC7-2E3E-9222-DD3F-E6A5-55C4-FB71
            Console.WriteLine("设备编号 ：" + machineId);
            bool init = true;
            do
            {
                Console.WriteLine("服务器地址：");
                var serverHost = Console.ReadLine();
                if (serverHost == "Q" || serverHost == "q")
                {
                    return;
                }
                Console.WriteLine("指定客户端名称：");
                var appName = Console.ReadLine();
                if (appName == "Q" || appName == "q")
                {
                    return;
                }

                Console.WriteLine("请稍候，正在尝试连接服务器");
                string filepath = Path.GetTempFileName();
                bool initOk = false;
                try
                {
                    if (!File.Exists(CONFIG + "/chrome-win/chrome.exe"))
                    {

                        WebClient client = new WebClient();
                        int lastProgress = 0;
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) =>
                        {
                            int progress = e.ProgressPercentage;
                            if (progress != lastProgress)
                            {
                                lastProgress = progress;
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write(progress + "%");
                            }
                        });
                        //下载完成的响应事件
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler((sender, e) =>
                        {
                            initOk = true;
                            Console.WriteLine("");
                        });

                        client.DownloadFileAsync(new Uri("http://" + serverHost + "/chrome-win.zip"), filepath);
                        Console.WriteLine("连接服务器正常，正在初始化数据");
                        while (!initOk)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                        Directory.CreateDirectory(CONFIG);
                        UnZip(filepath, CONFIG);
                    }
                    AppInfo appInfo = new AppInfo();
                    appInfo.ServerHost = serverHost;
                    appInfo.AppName = appName;
                    appInfo.machineId = machineId;

                    BinaryFormatter bf = new BinaryFormatter();

                    using (FileStream fsWrite = new FileStream(CONFIG_DB, FileMode.Create, FileAccess.Write))
                    {
                        //二进制序列化
                        bf.Serialize(fsWrite, appInfo);
                    }
                    Console.WriteLine("配置完成");
                    init = false;
                }
                catch
                {
                    Console.WriteLine("连接服务器失败");
                    init = true;
                }
                finally
                {
                    File.Delete(filepath);
                }


            } while (init);
        }

        static System.Timers.Timer timerTimeout;
        /// <summary>
        /// 在指定时间过后执行指定的表达式
        /// </summary>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param>
        /// <param name="action">要执行的表达式</param>
        public static void SetTimeout(double interval, Action action)
        {
            if (timerTimeout == null)
            {
                timerTimeout = new System.Timers.Timer(interval);
            }
            timerTimeout.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
           {
               timerTimeout.Enabled = false;
               action();
           };
            timerTimeout.Enabled = true;
        }

        static System.Timers.Timer timerInterval;
        /// <summary>
        /// 在指定时间周期重复执行指定的表达式
        /// </summary>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param>
        /// <param name="action">要执行的表达式</param>
        public static void SetInterval(double interval, Action<ElapsedEventArgs> action)
        {
            if (timerInterval == null)
            {
                timerInterval = new System.Timers.Timer(interval);
            }

            timerInterval.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
        {
            action(e);
        };
            timerInterval.Enabled = true;
        }

        public static void cleanTimeout()
        {
            if (timerTimeout != null && timerTimeout.Enabled)
            {
                timerTimeout.Enabled = false;
                timerTimeout = null;
            }


        }

        public static void cleanInterval()
        {
            if (timerInterval != null && timerInterval.Enabled)
            {
                timerInterval.Enabled = false;
                timerInterval = null;
            }
        }


        /// <summary>

        /// Read password from console

        /// </summary>

        /// <returns>password</returns>

        public static string ReadPassword()

        {

            char[] revisekeys = new char[3];

            revisekeys[0] = (char)0x08;

            revisekeys[1] = (char)0x20;

            revisekeys[2] = (char)0x08;



            StringBuilder sb = new StringBuilder();

            while (true)

            {

                ConsoleKeyInfo kinfo = Console.ReadKey(true);



                if (kinfo.Key == ConsoleKey.Enter)

                {

                    break;

                }



                if (kinfo.Key == ConsoleKey.Backspace)

                {

                    if (sb.Length != 0)

                    {

                        int rIndex = sb.Length - 1;

                        sb.Remove(rIndex, 1);

                        Console.Write(revisekeys);

                    }

                    continue;

                }

                sb.Append(Convert.ToString(kinfo.KeyChar));

                Console.Write("*");

            }

            return sb.ToString();

        }

        #endregion


        static string CONFIG = "Config";
        static string CONFIG_DB = "Config/__DB";
        static IWebSocketConnection socket;

        public static async Task Main1(string[] args)
        {
            if (!Directory.Exists(CONFIG) || !File.Exists(CONFIG_DB))
            {
                init();
            }

            AppInfo appInfo = new AppInfo();
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                using (FileStream fsRead = new FileStream(CONFIG_DB, FileMode.Open, FileAccess.Read))
                {
                    appInfo = bf.Deserialize(fsRead) as AppInfo;
                }
            }
            catch
            {
            }
            if (appInfo == null || appInfo.ServerHost == null || appInfo.ServerHost == string.Empty)
            {
                File.Delete(CONFIG_DB);
                init();
            }
            /*
           Console.WriteLine("This example downloads the default version of Chromium to a custom location");

           var currentDirectory = Directory.GetCurrentDirectory();
           var downloadPath = Path.Combine(currentDirectory, "..", "..", "CustomChromium");
           Console.WriteLine($"Attemping to set up puppeteer to use Chromium found under directory {downloadPath} ");

           if (!Directory.Exists(downloadPath))
           {
               Console.WriteLine("Custom directory not found. Creating directory");
               Directory.CreateDirectory(downloadPath);
           }

           Console.WriteLine("Downloading client");

           var browserFetcherOptions = new BrowserFetcherOptions { Path = downloadPath };
           var browserFetcher = new BrowserFetcher(browserFetcherOptions);
           await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
           */
            //var executablePath = browserFetcher.GetExecutablePath(BrowserFetcher.DefaultRevision);
            var executablePath = $"{CONFIG}/chrome-win/chrome.exe";

            if (!File.Exists(executablePath))
            {
                Console.WriteLine("数据初使化失败，再次尝试！");
                init();
                return;
            }
            while (true)
            {
                Console.WriteLine("输入账户：");
                var loginName = Console.ReadLine();
                Console.WriteLine("输入密码：");
                var pwd = ReadPassword();
                var machineId = appInfo.machineId;
                Console.WriteLine("");

                var res = Post("http://" + appInfo.ServerHost + "/checkLogin", "loginName=" + loginName + "&pwd=" + pwd + "&machineId=" + machineId);
                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
                if (data["msg"] == "success")
                {
                    appInfo.apiHost = data["apiHost"];
                    break;
                }
                Console.WriteLine("\n登录失败:" + data);
            }

            var options = new LaunchOptions { Headless = true, ExecutablePath = executablePath };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = (await browser.PagesAsync())[0])
            {
                await page.GoToAsync("https://www.it.com.cn");
                var jQuery = @"var s=document.createElement('script');s.src='https://libs.baidu.com/jquery/2.0.3/jquery.min.js';document.getElementsByTagName('body')[0].appendChild(s);";
                await page.EvaluateExpressionAsync(jQuery);

            }
            return;

        }

        private async static void pdf(Page page, string path)
        {
            //设置PDF选项
            PdfOptions pdfOptions = new PdfOptions();
            pdfOptions.DisplayHeaderFooter = false; //是否显示页眉页脚
            pdfOptions.FooterTemplate = "";   //页脚文本
            pdfOptions.Format = new PuppeteerSharp.Media.PaperFormat(8.27m, 11.69m);  //pdf纸张格式 英寸为单位
            pdfOptions.HeaderTemplate = "";   //页眉文本
            pdfOptions.Landscape = false;     //纸张方向 false-垂直 true-水平
            pdfOptions.MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Bottom = "0px", Left = "0px", Right = "0px", Top = "0px" }; //纸张边距，需要设置带单位的值，默认值是None
            pdfOptions.Scale = 1m;            //PDF缩放，从0-1
            await page.PdfAsync(path, pdfOptions);
        }

        private async static void screenshots(Page page, string outputFile)
        {
            await page.ScreenshotAsync(outputFile);
        }
        
    }
}
