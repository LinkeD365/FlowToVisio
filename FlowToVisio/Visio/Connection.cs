using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkeD365.FlowToVisio
{
    public class Connection
    {
        public string Name { get; set; }
        public string Api { get; set; }

        public Connection(string name, string api)
        {
            Name = name;
            Api = api;
        }

        private static List<Connection> aPIConnections;

        public static List<Connection> APIConnections
        {
            get
            {
                return aPIConnections;
            }
        }

        public static void ClearAPIs()
        {
            aPIConnections = null;
        }

        internal static void SetAPIs(JObject root)
        {
            aPIConnections = new List<Connection>();
            foreach (var item in root["properties"]["connectionReferences"].Children<JProperty>())
            {
                aPIConnections.Add(new Connection(item.Name, ((JProperty)item.Value["api"].Children().First()).Value.ToString()));
            }
        }
    }
}