using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowRunForm : Form
    {
        public List<FlowRun> FlowRuns { get; set; }
        private FlowDefinition flowDefinition;
        private FlowConn flowConn;
        private HttpClient _client;
        private FlowToVisioControl parent;

        public FlowRunForm(List<FlowRun> runs, FlowDefinition flow, FlowConn flowConn, HttpClient client, FlowToVisioControl flowToVisioControl)
        {
            InitializeComponent();
            FlowRuns = runs;
            flowDefinition = flow;
            this.flowConn = flowConn;
            _client = client;
            parent = flowToVisioControl;
            dgvFlowRuns.DataSource = FlowRuns;
            SetupFilter();
            SetupColumns();
            SortFlowGrid("Start", SortOrder.Descending);
            Text = "Runs for " + flowDefinition.Name;
            lblCancel.Visible = false;
        }

        private void SetupFilter()
        {
            ddlFilter.Items.Clear();
            ddlFilter.Items.Add("Filter By");
            ddlFilter.Items.AddRange(FlowRuns.Select(fr => fr.Status).Distinct().ToArray());
            ddlFilter.SelectedIndex = 0;
        }

        private void SetupColumns()
        {
            dgvFlowRuns.Columns["Selected"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void SortFlowGrid(string name, SortOrder sortOrder)
        {
            List<FlowRun> sortingFlowRuns = (List<FlowRun>)dgvFlowRuns.DataSource;
            sortingFlowRuns.Sort(new FlowRunComparer(name, sortOrder));
            dgvFlowRuns.DataSource = null;
            dgvFlowRuns.DataSource = sortingFlowRuns;
            SetupColumns();
            dgvFlowRuns.Columns[name].HeaderCell.SortGlyphDirection = sortOrder;
        }

        private void dgvFlowRuns_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvFlowRuns.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable) return;
            SortOrder sortOrder = GetSortOrder(e.ColumnIndex);

            SortFlowGrid(dgvFlowRuns.Columns[e.ColumnIndex].Name, sortOrder);
        }

        private SortOrder GetSortOrder(int columnIndex)
        {
            if (dgvFlowRuns.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None ||
               dgvFlowRuns.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending)
            {
                dgvFlowRuns.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                return SortOrder.Ascending;
            }
            else
            {
                dgvFlowRuns.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                return SortOrder.Descending;
            }
        }

        private void btnStopFlows_Click(object sender, EventArgs e)
        {
            List<FlowRun> flowRuns = ((List<FlowRun>)dgvFlowRuns.DataSource).Where(fr => fr.Selected && fr.Status == "Running").ToList();
            if (flowRuns.Any())
            {
                CancelAllFlows(flowRuns);
            }
            else MessageBox.Show("Please select one or more running flows before cancelling", "Select a Flow", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private FlowRun CancelFlow(FlowRun flowRun)
        {
            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{flowDefinition.UniqueId}/runs/{flowRun.Id}/cancel?api-version=2016-11-01";
            flowRun.Message = _client.PostAsync(url, null).Result;
            return flowRun;
        }

        private List<FlowRun> CancelFlows(List<FlowRun> flowRuns, BackgroundWorker w)
        {
            return flowRuns.Select(fr => CancelFlow(fr)).ToList();
        }

        private void CancelAllFlows(List<FlowRun> flowRuns)
        {
            lblCancel.Visible = true;
            parent.WorkAsync(new WorkAsyncInfo
            {
                Message = "Cancelling " + flowRuns.Count + " Flows for " + flowDefinition.Name,
                Work = (w, args) => args.Result = CancelFlows(flowRuns, w),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null) { parent.ShowError(args.Error.Message, "Error"); }
                    else
                    {
                        List<FlowRun> returnFlows = args.Result as List<FlowRun>;
                        DialogResult = DialogResult.Yes;
                        // this.Close();
                    }
                }
            });
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            List<FlowRun> flowRuns = dgvFlowRuns.DataSource as List<FlowRun>;
            flowRuns.ForEach(fr => fr.Selected = chkAll.Checked);
            dgvFlowRuns.DataSource = null;
            dgvFlowRuns.DataSource = flowRuns;
            SetupColumns();
        }

        private void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlFilter.SelectedIndex <= 0) return;
            //List<FlowRun> flowRuns = dgvFlowRuns.DataSource as List<FlowRun>;

            dgvFlowRuns.DataSource = null;
            dgvFlowRuns.DataSource = FlowRuns.Where(fr => fr.Status == ddlFilter.Text).ToList();
            SetupColumns();
        }
    }
}