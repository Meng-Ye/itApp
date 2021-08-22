
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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

                        client.DownloadFileAsync(new Uri($"http://{ serverHost }:{API_PORT}/soft/chrome-win.zip"), filepath);
                        Console.WriteLine("连接服务器正常，正在初始化数据");
                        while (!initOk)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                        Directory.CreateDirectory(CONFIG);
                        UnZip(filepath, CONFIG);
                    }
                    appInfo = new AppInfo();
                    appInfo.ServerHost = serverHost;
                    appInfo.AppName = appName;
                    appInfo.machineId = machineId;

                    BinaryFormatter bf = new BinaryFormatter();

                    using (FileStream fsWrite = new FileStream(CONFIG_DB, FileMode.Create, FileAccess.Write))
                    {
                        //二进制序列化
                        bf.Serialize(fsWrite, appInfo);
                    }

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
            Console.WriteLine("配置完成");
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


        private static string CONFIG = "Config";
        private static string CONFIG_DB = "Config/__DB";
        private static string API_PORT = "50001";
        private static string WS_PORT = "50000";
        private static bool runTest = false;
        private static AppInfo appInfo;
        private static Page page;
        private static string executablePath = $"{CONFIG}/chrome-win/chrome.exe";
        private static string pPath = "se" + "arch";
        private static string pPpPath = "gpn";
        private static string seNamee = "search" + "Term";
        private static string ssPath = "store" + "services";
        private static string cart = "cart";
        private static string otPath = "opn" + "inventory";
        private static string web = "https://www.ti.com.cn";
        private static string slfPath = "secure" + "-" + "link" + "-" + "forward";
        private static string store = "store"; 
        private static string product = "product";
        private static string loginScript;
         
        private static Thread changeThread;
        public static async Task Main(string[] args)
        {
            if (runTest)
            {
                await test.Main11(args);
                return;
            }
            if (!Directory.Exists(CONFIG) || !File.Exists(CONFIG_DB))
            {
                init();
            }


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


            if (!File.Exists(executablePath))
            {
                Console.WriteLine("数据初使化失败，再次尝试！");
                init();
                return;
            }
            var iaccount = 0;
            while (iaccount == 1)
            {
                Console.WriteLine("输入账户：");
                var loginName = Console.ReadLine();
                Console.WriteLine("输入密码：");
                var pwd = ReadPassword();
                var machineId = appInfo.machineId;
                Console.WriteLine("");

                var res = Post($"http://{appInfo.ServerHost}:{API_PORT}/api/checkLogin", $"loginName={loginName}&pwd={ pwd }&machineId={ machineId}");
                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
                if (data["msg"] == "success")
                {
                    appInfo.apiHost = data["apiHost"];
                    break;
                }
                Console.WriteLine("\n登录失败:" + data);
            }
            Console.WriteLine("程序已启用中");
            Console.WriteLine("请输入要监控的商品名称，多个商品用空格分隔，比如 LM10");
            string searchTerm = Console.ReadLine();
            if (searchTerm == string.Empty)
            {
                searchTerm = "LM10";
            }
            string[] arr = searchTerm.Split(new char[2] { ',', ' ' });
            var hit = "";
            string item = "";
            int inventory = 0;
            JToken jt = null;
            while (hit == "")
            {
                Console.WriteLine($"需要查找商品：{  searchTerm } 共 {arr.Length } 个");
                for (var count = 0; count < arr.Length; count++)
                {
                    item = arr[count];
                    Console.WriteLine($"   查找第{count + 1} 个 {item}");
                    var url = $"{web}/{pPath}/{pPpPath}?{seNamee}={item}&locale=zh-CN";
                    try
                    {

                        var res = Get(url);
                        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                        JArray opns = (JArray)data["opns"];

                        foreach (var opn in opns)
                        {
                            if (opn.ToString() == item)
                            {
                                jt = opn;
                            }
                        }
                        if (jt != null)
                        {
                            Console.Write($"   找到精准商品:{jt}");
                            try
                            {
                                url = $"{web}/{ssPath}/{cart}/{otPath}?opn={jt}";
                                var opnRes = Get(url);
                                //{"orderable_number":"LM10CLN/NOPB","inventory":3969}
                                Dictionary<string, object> opnData = JsonConvert.DeserializeObject<Dictionary<string, object>>(opnRes);
                                inventory = Int32.Parse(opnData["inventory"].ToString());
                                if (inventory > 0)
                                {
                                    hit = jt.ToString();
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"url:{ url}");
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"   找到同型号商品{opns.Count} 个: {opns}");
                            foreach (var opn in opns)
                            {
                                Console.Write($"      检查商品{opn}库存");
                                try
                                {
                                    url = $"{web}/{ssPath}/{cart}/{otPath}?opn={opn}";
                                    var opnRes = Get(url);
                                    //{"orderable_number":"LM10CLN/NOPB","inventory":3969}
                                    Dictionary<string, object> opnData = JsonConvert.DeserializeObject<Dictionary<string, object>>(opnRes);
                                    inventory = Int32.Parse(opnData["inventory"].ToString());
                                    if (inventory > 0)
                                    {
                                        hit = opn.ToString();
                                        break;
                                    }
                                    Console.WriteLine(":0");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"url:{ url}");
                                    Console.WriteLine(e.Message);
                                    Console.WriteLine(e.StackTrace);
                                }

                            }
                        }

                        if (inventory > 0)
                        {
                            break;
                        }
                        Console.WriteLine("");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"url:{ url}");
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
                Thread.Sleep(3000);
            }
            Console.WriteLine($" 找到库存共{inventory}");

            var mobileText = $"【{appInfo.AppName}】软件查到产品名称【{(jt == null ? item + "->" + hit : hit)}】有库存【{inventory}】,请速登陆查看。";

            sendMsg(mobileText);

            if (!Directory.Exists("OrderImg"))
            {
                Directory.CreateDirectory("OrderImg");
            }

            ViewPortOptions viewPortOptions = new ViewPortOptions();
            viewPortOptions.Width = 1440;
            viewPortOptions.Height = 1280;
            var options = new LaunchOptions { Headless = false, Devtools = true, ExecutablePath = executablePath, DefaultViewport = viewPortOptions };

            var browser = await Puppeteer.LaunchAsync(options);
            browser.TargetCreated += Browser_TargetCreated;
            browser.TargetChanged += Browser_TargetCreated;

            page = (await browser.PagesAsync())[0];
            page.DefaultTimeout = 3000000 * 60;
            page.DefaultNavigationTimeout = page.DefaultTimeout;

            await page.SetRequestInterceptionAsync(true);
            page.Request += Page_Request;
            var prodUrl = $"{web}/{product}/cn/{prodName}";
            if (jt != null)
            {
                Console.WriteLine($"开始下单：{hit}");
                prodUrl = $"{web}/{store}/ti/zh/p/{product}/?p={prodName}";
            }
            else {
                Console.WriteLine($"开始下单：{item}->{hit}");

            }
            await page.GoToAsync($"{web}/{slfPath}/?gotoUrl={prodUrl}");

            Console.WriteLine("按任意键退出");
            Console.ReadLine();
            return;

        }

        private static void Page_Request(object sender, RequestEventArgs e)
        {
            if (e.Request.Url.IndexOf("google") != -1)
            {
                e.Request.AbortAsync();
            }
            else
            if (e.Request.Url.IndexOf("adroll.com") != -1)
            {
                e.Request.AbortAsync();
            }
            else
            if (e.Request.Url.IndexOf("doubleclick") != -1)
            {
                e.Request.AbortAsync();
            }
            else
            if (e.Request.Url.IndexOf("facebook") != -1)
            {
                e.Request.AbortAsync();
            }
            else
            if (e.Request.Url.IndexOf("bluekai") != -1)
            {
                e.Request.AbortAsync();
            }
            else
            if (e.Request.Url.IndexOf("supplyframe") != -1)
            {
                e.Request.AbortAsync();
            }
            else
            if (e.Request.Url.IndexOf("fonts") != -1)
            {
                e.Request.AbortAsync();
            }
            else
            if (e.Request.Url.IndexOf("supplyframe") != -1)
            {
                e.Request.AbortAsync();
            }
            else if (e.Request.Url.IndexOf(".ti.com") == -1)
            {
                e.Request.AbortAsync();
            }
            else
            {
                ResourceType responseType = e.Request.ResourceType;
                if (responseType == ResourceType.Script || responseType == ResourceType.Image || responseType == ResourceType.Img || responseType == ResourceType.Font || responseType == ResourceType.StyleSheet || responseType == ResourceType.Other)
                {
                    double interval = 15000;
                    System.Timers.Timer timerTimeout = new System.Timers.Timer(interval);
                    timerTimeout.Elapsed += new ElapsedEventHandler((s, e1) =>
                    {
                        // Console.WriteLine($"请求 {e.Request.Url } => {e.Request.ResourceType} 超过{interval / 1000}秒，强制取消。");
                        e.Request.AbortAsync();
                        timerTimeout.Enabled = false;
                    });
                    timerTimeout.Enabled = true;
                }
                // Console.WriteLine($"url:{ e.Request.Url},type:{ e.Request.ResourceType}, method:{e.Request.Method}");
                e.Request.ContinueAsync();
            }

        }

        private static string getUrlType(string url)
        {
            string url0 = url.Split('?')[0];
            if (url0.IndexOf("authorization.oauth") != -1)
            {
                return "login";
            }
            else if (url0.IndexOf($"/{product}/cn/{prodName}") != -1 || url.IndexOf($"/p/{product}/?p={prodName}") != -1)
            {
                return "order";
            }
            else if (url0.IndexOf($"/{store}/ti/zh/{cart}") != -1)
            {
                return "checkCart";
            }
            else if (url0.IndexOf("multi/delivery-address") != -1)
            {
                return "deliveryAddress";
            }
            else if (url0.IndexOf("multi/cn-tax-invoice") != -1)
            {
                return "taxInvoice";
            }
            else if (url0.IndexOf("multi/regulations-step/choose") != -1)
            {
                return "regulations";
            }
            else if (url0.IndexOf("multi/delivery-method") != -1)
            {
                return "deliveryMethod";
            }
            else if (url0.IndexOf("multi/payment-method/citcon/addSelectedPayment") != -1)
            {
                return "orderInfo";
            }
            else if (url0.IndexOf("multi/payment-method") != -1)
            {
                return "paymentMethod";
            }


            return null;
        }



        private static void Browser_TargetCreated(object sender, TargetChangedArgs e)
        {
            if (e.Target.Url.IndexOf(".ti.com") != -1)
            {
                async Task change()
                {
                    var waitForFunctionOtions = new WaitForFunctionOptions() { Timeout = 2000, PollingInterval = 5000 };
                    if (loginScript == null)
                    {
                        loginScript = Get($"http://{appInfo.ServerHost}:{API_PORT}/hook.js");
                    }
                    System.Console.WriteLine("是目标页面，开始注入jQuery脚本");
                    await page.WaitForNavigationAsync();
                    await page.EvaluateExpressionAsync(loginScript);
                    await page.WaitForFunctionAsync("addJquery", waitForFunctionOtions);
                    System.Console.WriteLine("jQuery注入完成");
                    string url = page.Url;
                    string type = getUrlType(url);
                    System.Console.WriteLine($"界面类型为：{type}");
                    string buyCount = "1";
                    string prodName = "";
                    if (type != null)
                    {
                        switch (type)
                        {

                            case "login":
                                System.Console.WriteLine($"操作登录");
                                var username = appInfo.ItUser;
                                if (username == null)
                                {
                                    username = "fzaw2008@163.com";
                                }
                                var password = appInfo.ItPwd;
                                if (password == null)
                                {
                                    password = "Wilson1234";
                                }
                                await page.EvaluateExpressionAsync($"login('{username}','{password}')");
                                break;
                            case "order":
                                System.Console.WriteLine($"操作下单");
                                if (appInfo.OrderMoney == null)
                                {
                                    appInfo.OrderMoney = "100";
                                }
                                var orderResult = await page.WaitForFunctionAsync("order", waitForFunctionOtions, appInfo.OrderMoney);
                                var json = await orderResult.JsonValueAsync();
                                JObject data = (JObject)json;
                                if (data["cardData"]["statusCode"].ToString() == "200")
                                {
                                    var cardId = data["cardData"]["cartId"].ToString();
                                    buyCount = data["buyCountInfo"]["buyCount"].ToString();
                                    prodName = data["buyCountInfo"]["prodName"].ToString();
                                    var singlePrice = data["buyCountInfo"]["singlePrice"].ToString();
                                    var inventoryLevel = data["buyCountInfo"]["inventoryLevel"].ToString();
                                    var totalMoney = Int32.Parse(buyCount) * Double.Parse(singlePrice);
                                    Console.WriteLine($"{prodName}加入购物车完成：总库存{inventoryLevel},加入 {buyCount} ，共 {totalMoney} 元");
                                    Console.WriteLine(json.ToString());
                                }
                                break;
                            case "checkCart":
                                System.Console.WriteLine($"操作购物车");
                                if (prodName != string.Empty)
                                {
                                    await page.WaitForFunctionAsync("checkCart", waitForFunctionOtions, prodName, buyCount);
                                }
                                break;
                            case "orderInfo":
                                System.Console.WriteLine($"操作获取订单信息");
                                var orderInfo = await page.WaitForFunctionAsync("orderInfo", waitForFunctionOtions);
                                var orderInfoObj = await orderInfo.JsonValueAsync();
                                Console.WriteLine($"下单结果：{orderInfoObj}");
                                JObject orderObj = (JObject)orderInfoObj;
                                var payAddress = orderObj["payAddr"].ToString();
                                var tiOrderCode = orderObj["tiOrderCode"].ToString();
                                var orderTotal = orderObj["orderTotal"].ToString();
                                var qrcode = $"OrderImg/{tiOrderCode}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.png";
                                await screenshots(page, qrcode);
                                sendMsg($"【{appInfo.AppName}】软件抢单成功，订单编号【{tiOrderCode}】，产品名称【{prodName}】，下单用户名【{appInfo.ItUser}】,订单总额【XXXX】，支付二维码 {payAddress}。");

                                break;
                            default:
                                System.Console.WriteLine($"操作{type}");
                                await page.WaitForFunctionAsync(type, waitForFunctionOtions);
                                break;
                        }
                        System.Console.WriteLine("这波操作结束");
                        return;

                    }else{
                        System.Console.WriteLine($"找不到操作目标，地址为：{url}");
                    }

                    System.Console.WriteLine("这个界面作什么也没有做");

                };
                if (changeThread!=null && changeThread.IsAlive)
                {
                    changeThread.Abort();
                    changeThread = null;
                }
                if (changeThread == null) {
                    changeThread = new Thread(async () => {
                        await change();
                    });
                    changeThread.Start();
                }           
                
                
                Console.WriteLine("browser create " + e.Target.Url);
            }

        }


        private static void sendMsg(string mobileText)
        {
            Console.WriteLine($"发送短信：{mobileText}");
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

        private async static
        Task
screenshots(Page page, string outputFile)
        {
            await page.ScreenshotAsync(outputFile);
        }

    }
}
