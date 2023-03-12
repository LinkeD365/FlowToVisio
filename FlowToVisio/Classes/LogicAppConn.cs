namespace LinkeD365.FlowToVisio
{
    public class LogicAppConn
    {
        public int Id = 0;
        public string Name = string.Empty;
        public string SubscriptionId = string.Empty;
        public string TenantId = string.Empty;
        public string AppId = string.Empty;
        public string ReturnURL = string.Empty;
        public bool UseDev;

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                LogicAppConn p = (LogicAppConn)obj;
                return (Id == p.Id);
            }
        }
    }
}