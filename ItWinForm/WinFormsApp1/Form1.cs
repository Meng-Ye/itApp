
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private string loginScript;

        public Form1()
        {
            InitializeComponent();
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
        private bool running = true;
        private string product = "product";
        private string store = "store";
        private string cart = "cart";
        private string buyCount;
        private string prodName;
        private string multi = "multi";
        private string payment = "payment";
        private string delivery = "delivery";
        private string method = "method";

        private string appName = "TI-01";
        private string tiOrderCode;
        private string userName = "fzaw2008@163.com";
        private string password = "Wilson1234";
        private Page page;
        private string workDir = @"D:\eclipse-workspace";
        private int prodIndex = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            var text1 = this.searchProdTextBox.Text.Trim();
            var text2 = this.buyMoneyTextBox.Text.Trim();
            if (text1 == string.Empty)
            {
                MessageBox.Show("商品信息为空，请先输入要搜索的商品");
                return;
            }
            if (text2 == string.Empty)
            {
                MessageBox.Show("商品金额为空，请先输入金额");
                return;
            }
            int result = 0;
            if (!Int32.TryParse(text2, out result))
            {
                MessageBox.Show("商品金额必须是正整数");
                return;
            }
            this.startButton.Enabled = false;
            this.scriptTextBox.Clear();
            this.infoRichTextBox.Clear();
            ViewPortOptions viewPortOptions = new ViewPortOptions();
            viewPortOptions.Width = 1920;
            viewPortOptions.Height = 1080;
            prodIndex = 0;
            prodName = this.searchProdTextBox.Text.Split(new char[] { ',' ,' '})[prodIndex];
            string executablePath = @$"{workDir}\itApp\ItApp\bin\Debug\Config\chrome-win\chrome.exe";
            if (!File.Exists(executablePath)) {
                workDir = @"D:\.NET";
                executablePath = @$"{workDir}\ItApp\ItApp\bin\Debug\Config\chrome-win\chrome.exe";
            }
            var options = new LaunchOptions { Headless = true, Devtools = true, ExecutablePath = executablePath, DefaultViewport = viewPortOptions };
            new Thread(new ThreadStart(async delegate
            {
                using (var browser = await Puppeteer.LaunchAsync(options))
                {
                    page = (await browser.PagesAsync())[0];

                    page.DefaultTimeout = 3000000 * 60;
                    page.DefaultNavigationTimeout = page.DefaultTimeout;
                    await page.SetRequestInterceptionAsync(true);
                    page.Request += Page_Request;
                    page.Console += Page_Console;
                    page.Error += Page_Error;
                    page.PageError += Page_PageError;
                    page.DOMContentLoaded += Page_DOMContentLoaded;
                    string web = "https://www.ti.com.cn";
                    await page.GoToAsync($"{web}");
                    await page.ExposeFunctionAsync<string, object>("getAction", getAction);
                    await page.ExposeFunctionAsync("clearAction", clearAction);
                    while (browser.IsConnected && running)
                    {

                    }

                }
            })).Start();
        }
        private string getUrlType(string url)
        {
            string url0 = url.Split('?')[0];
            if (url0 == "https://www.ti.com.cn" || url0 == "https://www.ti.com.cn/")
            {
                return "doSearch()";
            }
            if (url0.IndexOf("authorization.oauth") != -1)
            {
                return $"login(\"{userName}\",\"{password}\")";
            }
            else if (url0.IndexOf($"/{product}/cn/{prodName}") != -1 || url.IndexOf($"/p/{product}/?p={prodName}") != -1)
            {
                return "order()";
            }
            else if (url0.IndexOf($"/{store}/ti/zh/{cart}") != -1)
            {
                return "checkCart";
            }
            else if (url0.IndexOf($"{multi}/{delivery}-address") != -1)
            {
                return $"{delivery}Address()";
            }
            else if (url0.IndexOf($"{multi}/cn-tax-invoice") != -1)
            {
                return "taxInvoice()";
            }
            else if (url0.IndexOf($"{multi}/regulations-step/choose") != -1)
            {
                return "regulations()";
            }
            else if (url0.IndexOf($"{multi}/{delivery}-{method}") != -1)
            {
                return $"{delivery}Method()";
            }
            else if (url0.IndexOf($"{multi}/{payment}-{method}/citcon/addSelectedPayment") != -1)
            {
                return "orderInfo()";
            }
            else if (url0.IndexOf($"{multi}/{payment}-{method}") != -1)
            {
                return $"{payment}Method()";
            }
            else if (url0.IndexOf("universalsearch.tsp") != -1)
            {
                return "searchItem()";
            }


            return null;
        }
        private void clearAction()
        {
            SetRickText($"clearAction is call");
            SetText("");
        }

        private object getAction(string arg)
        {
            if (arg.IndexOf("http") != -1)
            {
                string type = getUrlType(arg);
                if (type != null)
                {
                    SetText(type);
                    SetRickText($"getAction is call,arg:{arg},resp:{type}");
                }
                return scriptTextBox.Text;
            }
            else
            {
                JObject JsonData = JObject.Parse(arg);
                if (arg.IndexOf("orderablePartNumber") != -1)
                {
                    string partName = JsonData["orderablePartNumber"].ToString();
                    string inventory = JsonData["inventory"].ToString();
                    SetRickText($"{prodName}找到库存{JsonData["orderablePartNumber"]} 共{inventory}");
                    string nName = partName != prodName ? (prodName + "->" + partName) : prodName;
                    var mobileText = $"【{appName}】软件查到产品名称【{nName}】有库存【{inventory}】,请速登陆查看。";

                    sendMsg(mobileText);
                }
                else if (arg.IndexOf("buyCountInfo") != -1)
                {
                    var cardId = JsonData["cardData"]["cartId"].ToString();
                    buyCount = JsonData["buyCountInfo"]["buyCount"].ToString();
                    //prodName = JsonData["buyCountInfo"]["prodName"].ToString();
                    var singlePrice = JsonData["buyCountInfo"]["singlePrice"].ToString();
                    var inventoryLevel = JsonData["buyCountInfo"]["inventoryLevel"].ToString();
                    var totalMoney = Int32.Parse(buyCount) * Double.Parse(singlePrice);
                    SetRickText($"{prodName}加入购物车完成：总库存{inventoryLevel},加入 {buyCount} ，共 {totalMoney} 元 , cardId = {cardId}");
                }
                else if (arg.IndexOf("tiOrderCode") != -1)
                {
                    var payAddress = JsonData["payAddr"].ToString();
                    tiOrderCode = JsonData["tiOrderCode"].ToString();
                    var orderTotal = JsonData["orderTotal"].ToString();
                    sendMsg($"【{appName}】软件抢单成功，订单编号【{tiOrderCode}】，产品名称【{prodName}】，下单用户名【{userName}】,订单总额【{orderTotal}】，支付二维码 {payAddress}。");

                }
                else if (arg.IndexOf("username") != -1)
                {
                    page.TypeAsync("#username", userName, new PuppeteerSharp.Input.TypeOptions() { Delay = 50 });
                }
                else if (arg.IndexOf("password") != -1)
                {
                    page.TypeAsync("#password", password, new PuppeteerSharp.Input.TypeOptions() { Delay = 50 });
                }
                else if (arg.IndexOf("checkUser") != -1)
                {
                    string user = JsonData["checkUser"].ToString();
                    if (user == userName)
                    {
                        SetText("TRUE");
                    }
                }
                else if (arg.IndexOf("checkPass") != -1)
                {
                    string pass = JsonData["checkPass"].ToString();
                    if (pass == password)
                    {
                        SetText("TRUE");
                    }
                }
                else if (arg.IndexOf("searchTerm") != -1) 
                {                    
                    string [] tmp = this.searchProdTextBox.Text.Split(new char[] { ',', ' ' });
                    if (prodIndex < tmp.Length) {
                        prodIndex++;
                        SetRickText($"{prodName} 暂无库存，开始下一个商品{tmp[prodIndex]}");
                        prodName = tmp[prodIndex];
                    }
                    else
                    {
                        prodIndex = 0;
                        prodName = tmp[prodIndex];
                        SetRickText($"{prodName}暂无库存");
                    }
                    loginScript = null;
                    getHookScript();
                }

            }
            return "";
        }

        private void sendMsg(string mobileText)
        {
            SetRickText("发送短信:" + mobileText);
        }

        private string getHookScript()
        {
            if (loginScript == null)
            {
                loginScript = File.ReadAllText(@$"{workDir}\itApp\itShell\hook.js", Encoding.UTF8);
                loginScript = $"var prodName='{prodName}';var buyMoney={this.buyMoneyTextBox.Text};\n\n" + loginScript;
            }
            return loginScript;
        }

        private void Page_DOMContentLoaded(object sender, EventArgs e)
        {
            Page page = (Page)sender;
            SetRickText($"【ERROR】=> 【Page_DOMContentLoaded】=> {page.Url} , EvaluateExpressionAsync");
            page.EvaluateExpressionAsync(getHookScript());
            string type = getUrlType(page.Url);
            if (type == "orderInfo()")
            {
                var qrcode = $"OrderImg/{tiOrderCode}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.png";
                page.ScreenshotAsync(qrcode);
            }
        }



        private void Page_PageError(object sender, PageErrorEventArgs e)
        {
            Page page = (Page)sender;
            //SetRickText($"【ERROR】=> 【Page_PageError】=> {page.Url} ,    {e.Message}");
        }

        private void Page_Error(object sender, PuppeteerSharp.ErrorEventArgs e)
        {
            Page page = (Page)sender;
            //SetRickText($"【ERROR】=>【Page_Error】=> {page.Url} ,   {e.Error}");
        }

        private void Page_Console(object sender, ConsoleEventArgs e)
        {
            Page page = (Page)sender;
            SetRickText($"【CONSOLE】=> 【Page_Console】=> {page.Url} , {e.Message.Text}");
        }



        private void Page_Request(object sender, RequestEventArgs e)
        {
            Page page = (Page)sender;
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
                if (responseType == ResourceType.Script || responseType == ResourceType.Image || responseType == ResourceType.Img || responseType == ResourceType.Font || responseType == ResourceType.StyleSheet)
                {
                    double interval = 45000;
                    System.Timers.Timer timerTimeout = new System.Timers.Timer(interval);
                    timerTimeout.Elapsed += new ElapsedEventHandler((s, e1) =>
                    {
                        if (e.Request.Response != null && !e.Request.Response.Ok)
                        {
                            SetRickText($"【INFO】=> 页面：{page.Url} [ 请求 {e.Request.Url } => {e.Request.ResourceType} 超过{interval / 1000}秒，强制取消。]");
                            e.Request.AbortAsync();
                        }
                        timerTimeout.Enabled = false;
                    });
                    timerTimeout.Enabled = true;
                }
                SetRickText($"【INFO】=> 页面：{page.Url} [ 请求:{ e.Request.Url},type:{ e.Request.ResourceType}]");
                e.Request.ContinueAsync();
            }

        }
        delegate void SetTextCallBack(string text);

        private void SetText(string text)
        {
            if (this.scriptTextBox.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                this.scriptTextBox.Text = text;
            }
        }

        delegate void SetRickTextCallBack(string text);
        private void SetRickText(string text)
        {
            if (this.infoRichTextBox.InvokeRequired)
            {
                SetRickTextCallBack stcb = new SetRickTextCallBack(SetRickText);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                if (this.infoRichTextBox.Lines.Length > 10000)
                {
                    int len = 3000;
                    string[] tmp = new string[len];
                    Array.Copy(this.infoRichTextBox.Lines, this.infoRichTextBox.Lines.Length - len, tmp, 0, len);
                    this.infoRichTextBox.Lines = tmp;
                }
                this.infoRichTextBox.AppendText("\n");
                this.infoRichTextBox.AppendText(text);
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            running = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.startButton.Enabled = true;
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            this.buyMoneyTextBox.Text = Regex.Replace(this.buyMoneyTextBox.Text, @"\D", "");
        }


        private void button3_Click(object sender, EventArgs e)
        {
            loginScript = null;
            getHookScript();
        }

        private void button1_EnabledChanged(object sender, EventArgs e)
        {
            this.stopButton.Enabled = !this.startButton.Enabled;
            this.searchProdTextBox.Enabled = this.startButton.Enabled;
            this.buyMoneyTextBox.Enabled = this.startButton.Enabled;
            running = this.startButton.Enabled;
            
        }
    }
}
