namespace LinkeD365.FlowToVisio
{
    public class FlowConn
    {
        public int Id = 0;
        public string Name;
        public string AppId;// = string.Empty;
        public string TenantId = string.Empty;
        public string ReturnURL = string.Empty;
        public string Environment = string.Empty;
        public bool UseDev;

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Legacy version of settings
    /// </summary>
    public class FlowConnection
    {
        public string AppId;// = string.Empty;
        public string TenantId = string.Empty;
        public string ReturnURL = string.Empty;
        public string Environment = string.Empty;
        public bool UseDev;
        public string SubscriptionId = string.Empty;

        public string LATenantId = string.Empty;
        public string LAAppId = string.Empty;
        public string LAReturnURL = string.Empty;
        public bool LAUseDev;
    }
}