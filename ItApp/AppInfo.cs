using System;

namespace ItApp
{
    [Serializable]
    public class AppInfo

    {
        public AppInfo()
        {
        }
        private string serverHost;
        private string appName;
        private string loginUser;
        private string itUser;
        private string itPwd;
        private string procName;
        private string orderNotifyMobile;
        private string storeNotifyMobile;
        private string orderMoney;
        private int delaySeconds = 3;
        private DateTime now = DateTime.Now;

        public int DelaySeconds { get => delaySeconds; set => delaySeconds = value; }
        public string OrderMoney { get => orderMoney; set => orderMoney = value; }
        public string ServerHost { get => serverHost; set => serverHost = value; }
        public string AppName { get => appName; set => appName = value; }
        public string LoginUser { get => loginUser; set => loginUser = value; }
        public string ItUser { get => itUser; set => itUser = value; }
        public string ItPwd { get => itPwd; set => itPwd = value; }
        public string ProcName { get => procName; set => procName = value; }
        public string OrderNotifyMobile { get => orderNotifyMobile; set => orderNotifyMobile = value; }
        public string StoreNotifyMobile { get => storeNotifyMobile; set => storeNotifyMobile = value; }
        public DateTime Now { get => now; set => now = value; }
        public string machineId { get; internal set; }
        public string apiHost { get; internal set; }
    }


}