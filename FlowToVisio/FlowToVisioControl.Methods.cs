using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase
    {
        private EntityCollection flowRecords;

        private void LoadFlows()
        {
            gridFlows.OrganizationService = Service;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Flows",
                Work = (w, e) =>
                {
                    var qe = new QueryExpression("workflow");
                    qe.ColumnSet.AddColumns("ismanaged", "clientdata", "description", "name", "createdon", "modifiedon", "modifiedby", "createdby");
                    qe.Criteria.AddCondition("category", ConditionOperator.Equal, 5);

                    var flowRecords = Service.RetrieveMultiple(qe);

                    e.Result = flowRecords;
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    flowRecords = (EntityCollection)e.Result;
                    gridFlows.DataSource = flowRecords;
                },
            });
        }
    }
}