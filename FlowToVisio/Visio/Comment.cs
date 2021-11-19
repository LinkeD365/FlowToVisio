using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using System;

namespace LinkeD365.FlowToVisio
{
    public class Comment
    {
        public Comment(Entity entity)
        {
            AnchorId = entity["anchor"].ToString();
            CommentString = JObject.Parse(entity["body"].ToString())["ops"][0]["insert"].ToString();
            Commenter = ((EntityReference)entity["createdby"]).Name;
            Kind = ((OptionSetValue)entity["kind"]).Value;
            Created = (DateTime)entity["createdon"];
        }

        public string AnchorId { get; set; }

        public int Kind { get; set; }

        public string Commenter { get; set; }

        public string CommentString { get; set; }

        public DateTime Created { get; set; }
    }
}