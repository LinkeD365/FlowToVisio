using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Forms;

namespace LinkeD365.FlowToVisio
{
    public class FlowRun
    {
        public string Id;

        [DisplayName(" ")]
        public bool Selected { get; set; }

        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        [Browsable(false)]
        public TimeSpan? DurationTS
        {
            get { return (End == null) ? null : End - Start; }
        }

        public string Duration
        {
            get
            {
                if (DurationTS == null) { return string.Empty; }
                TimeSpan durationTS = DurationTS.Value;
                if (durationTS.TotalDays >= 1) return durationTS.ToString(@"d\d hh.mm");
                if (durationTS.TotalHours >= 1) return durationTS.ToString(@"hh\:mm");
                if (durationTS.TotalMinutes >= 1) return durationTS.ToString(@"mm\:ss");
                if (durationTS.TotalSeconds >= 1) return durationTS.ToString(@"ss\.fff") + " s";
                return durationTS.ToString(@"fff") + " ms";
            }
        }

        public string Status { get; set; }

        [Browsable(false)]
        public HttpResponseMessage Message { get; internal set; }
    }

    internal class FlowRunComparer : IComparer<FlowRun>
    {
        private string fieldName = string.Empty;
        private SortOrder sortOrder = SortOrder.None;

        public FlowRunComparer(string fieldName, SortOrder sortingOrder)
        {
            this.fieldName = fieldName;
            sortOrder = sortingOrder;
        }

        public int Compare(FlowRun flowRun1, FlowRun flowRun2)
        {
            switch (fieldName)
            {
                case "Selected":
                default:
                    return sortOrder == SortOrder.Ascending ? flowRun1.Selected.CompareTo(flowRun2.Selected) : flowRun2.Selected.CompareTo(flowRun1.Selected);

                case "Start":
                    return sortOrder == SortOrder.Ascending ? flowRun1.Start.CompareTo(flowRun2.Start) : flowRun2.Start.CompareTo(flowRun1.Start);

                case "End":
                    return (int)(sortOrder == SortOrder.Ascending ? flowRun1.End.HasValue ? flowRun1.End?.CompareTo(flowRun2.End) : 0 : flowRun2.End?.CompareTo(flowRun1.End));

                case "Duration":
                case "DurationTS":
                    return (int)(sortOrder == SortOrder.Ascending ? flowRun1.DurationTS?.CompareTo(flowRun2.DurationTS) : flowRun2.DurationTS?.CompareTo(flowRun1.DurationTS));

                case "Status":
                    return sortOrder == SortOrder.Ascending ? flowRun1.Status.CompareTo(flowRun2.Status) : flowRun2.Status.CompareTo(flowRun1.Status);
            }
        }
    }
}