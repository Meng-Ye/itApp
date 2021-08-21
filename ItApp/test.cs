
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ItApp
{
    class test
    {
        readonly ClientWebSocket _webSocket = new ClientWebSocket();
        readonly CancellationToken _cancellation = CancellationToken.None;
        readonly string WS_URL = "ws://127.0.0.1:50000/echo";

        
        
        public async Task WebSocket()
        {
            try
            {
                //建立连接

                await _webSocket.ConnectAsync(new Uri(WS_URL), _cancellation);

                Console.WriteLine("输入消息：");
                var input = Console.ReadLine();
                while (input != "exit" && input != "quit")
                {
                    var bsend = Encoding.UTF8.GetBytes(input);
                    await _webSocket.SendAsync(new ArraySegment<byte>(bsend), WebSocketMessageType.Binary, true, _cancellation); //发送数据
                    StringBuilder sb = new StringBuilder();
                    var breceive = new byte[1024];
                    while(true)
                    {
                        WebSocketReceiveResult wrr = await _webSocket.ReceiveAsync(new ArraySegment<byte>(breceive), _cancellation);//接受数据
                        var str = Encoding.UTF8.GetString(breceive, 0, wrr.Count);
                        sb.Append(str);
                        if (wrr.EndOfMessage)
                        {
                            break;
                        }
                    }
                    Console.WriteLine("回复：" + sb.ToString() + "," + sb.Length);
                    Console.WriteLine("输入消息：");
                    input = Console.ReadLine();

                }
                _webSocket.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //await  WebSocket();
            }
           
        }




        public static async Task Main11(string[] args)
        {
            Console.WriteLine("test");
            var test = new test();
            await test.WebSocket();
            Console.WriteLine("按任意键退出");
            Console.ReadLine();
        }
    }
}
