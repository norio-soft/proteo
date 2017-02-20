using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Text;
using System.Collections.Generic;

namespace Orchestrator.WebUI.UserControls
{
    public partial class Schedule : System.Web.UI.UserControl
    {
        #region Templates
        private readonly string headerTable = @"<table class=""p1-ganttOuterTable"" cellPadding=""0"" cellSpacing=""0"" ><tr><td style=""vertical-align:top;"" ><div class=""p1-ganttDiv"" ><table cellPadding=""0"" cellSpacing=""0"" id=""GanttTable"" class=""p1-ganttInnerTable""><tr><th scope=""col"" rowSpan=""2"" class=""p1-ganttTitleHeaderCell"">{0}</th>";
        private readonly string headerWeekRow = @"<th scope=""col"" class=""p1-ganttMajorTimeUnitHeaderCell"" colspan=""7"">{0}</th>";
        private readonly string headerDayRowBegin = @"<tr class=""p1-ganttDetailTimeUnitRow"">";
        private readonly string headerDayRowEnd = @"</tr>";
        private readonly string headerDayRowday = @"<th scope=""col""><abbr title=""{0}"">{1}</abbr></th>"; // 7 x headerWeekRow
        private readonly string itemRowBegin = @"<tr tpId=""{0}"" class=""p1-ganttTaskRow"" height=""20"" ><th scope=""row"" nowrap title=""{1}"" class=""p1-ganttTitleCell""><div class=""p1-ganttTitleCellText""><a href=""{2}"">{1}</a></div></th>";
        private readonly string itemRowBlankWD = @"<td class=""p1-GWD"">{0}</td>";
        private readonly string itemRowBlankNWD = @"<td class=""p1-GNWD"">{0}</td>";
        private readonly string itemRowBlankT = @"<td class=""p1-GT"">{0}</td>";
        private readonly string itemImgStart = @"<img class=""p1-ganttNonTransparentImage"" src=""../images/StartNormal.gif"" width=""100%"" height=""20"" title=""{0}"" alt=""""/>";
        private readonly string itemImgMiddle = @"<img class=""p1-ganttNonTransparentImage"" src=""../images/CenterNormal.gif"" width=""100%"" height=""20"" title=""{0}"" alt=""""/>";
        private readonly string itemImgEnd = @"<img class=""p1-ganttNonTransparentImage"" src=""../images/EndNormal.gif"" width=""100%"" height=""20"" title=""{0}"" alt=""""/>";
        private readonly string headerTableFooterBegin = @"<tr height='1px'><td><img src='../images/blank.gif' width='100px' height='1px' alt=''/></td>";
        private readonly string headerTableFooterItem = @"<td height='1px'><img src='../images/blank.gif' width='16px' height='1px' alt=''/></td>";
        private readonly string headerTableFooterEnd = @"</tr></table></div></td></tr></table>";
        #endregion

        #region Private Fields
        private DateTime _columnStartDate = DateTime.Today;
        private DateTime _columnEndDate = DateTime.Today;
        private DateTime _scheduleStartDate = DateTime.Today;
        private DateTime _schedulEndDate = DateTime.Today;
        private int _weeksGenerated = 0;
        private int _daysGenerated = 0;
        private DataSet _data = null;

        private string _startDateField = string.Empty;
        private string _endDateField = string.Empty;
        private string _groupingField = string.Empty;
        private string _titleField = string.Empty;
        private string _TitleActionField = string.Empty;
        private string _toolTipField = string.Empty;
        private string _actionField = string.Empty;

        #endregion

        #region Properties

        public string StartDateField
        {
            get { return _startDateField; }
            set { _startDateField = value; }
        }

        public string EndDateField
        {
            get { return _endDateField; }
            set { _endDateField = value; }
        }

        public string GroupingField
        {
            get { return _groupingField; }
            set { _groupingField = value; }
        }

        public string TitleField
        {
            get { return _titleField; }
            set { _titleField = value; }
        }

        public string TitleActionField
        {
            get { return _TitleActionField; }
            set { _TitleActionField = value; }
        }

        public string ToolTipField
        {
            get { return _toolTipField; }
            set { _toolTipField = value; }
        }

        public string ActionField
        {
            get { return _actionField; }
            set { _actionField = value; }
        }

        private DateTime ColumnStartDate
        {
            get { return this._columnStartDate; }
            set { this._columnStartDate = value; }
        }

        private DateTime ColumnEndDate
        {
            get { return this._columnEndDate; }
            set { this._columnEndDate = value; }
        }

        private int WeeksGenerated
        {
            get { return this._weeksGenerated; }
            set { this._weeksGenerated = value; }
        }

        private int DaysGenerated
        {
            get { return this._daysGenerated; }
            set { this._daysGenerated = value; }
        }

        public DataSet Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public DateTime ScheduleStartDate
        {
            get { return _scheduleStartDate; }
            set { _scheduleStartDate = value; }
        }

        public DateTime ScheduleEndDate
        {
            get { return _schedulEndDate; }
            set { _schedulEndDate = value; }
        }

        #endregion

        #region Page Load/Init Events
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        #endregion

        public void DataBind()
        {
            if (_data == null)
                throw new ArgumentNullException("No DataSet has been supplied");

            phSchedule.Controls.Add(new LiteralControl(GenerateSchedule(ScheduleStartDate, ScheduleEndDate)));
        }

        #region Table Generation Methods

        private string GenerateHeader()
        {
            return string.Format(headerTable, "Driver");
        }

        private string GenerateRow()
        {
            return string.Empty;
        }

        private string GenerateSchedule(DateTime startDate, DateTime endDate)
        {

            #region Correct the dates
            // Ensure that the first day is a monday.
            if (startDate.DayOfWeek != DayOfWeek.Monday)
            {
                while (startDate.DayOfWeek != DayOfWeek.Monday)
                {
                    startDate = startDate.AddDays(-1);
                }
            }

            // ensure that the end date is a sunday
            if (endDate.DayOfWeek != DayOfWeek.Sunday)
            {
                while (endDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    endDate = endDate.AddDays(1);
                }
            }

            ColumnStartDate = startDate;
            ColumnEndDate = endDate;
            #endregion

            #region Generate The header Information
            // We need to know the number of weeks for this.
            int dayCount = ColumnEndDate.Subtract(ColumnStartDate).Days;
            int weeks = dayCount / 7;
            int rem = dayCount % 7;
            if (rem > 0) weeks++;

            DaysGenerated = 7 * weeks;
            WeeksGenerated = weeks;
            HtmlTable tbl = new HtmlTable();


            StringBuilder sb = new StringBuilder();
            sb.Append(GenerateHeader());

            string tmp = string.Empty;

            for (int i = 0; i < weeks; i++)
            {
                // determine the date for eah of the weeks for display
                tmp = string.Format(headerWeekRow, startDate.AddDays(i * 7).ToString("dd/MM/yy"));
                sb.Append(tmp);
            }
            sb.Append("</tr>");
            sb.Append(headerDayRowBegin);

            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saaturday", "Sunday" };
            for (int i = 0; i < weeks; i++)
            {
                foreach (string day in days)
                {
                    tmp = string.Format(headerDayRowday, day, day.Substring(0, 1));
                    sb.Append(tmp);
                }
            }
            sb.Append(headerDayRowEnd);

            #endregion

            #region Create the Items

            TaskRow taskRow = null;
            TaskRowItem tri = null;
            string currentValue = string.Empty;
            foreach (DataRow dr in Data.Tables[0].Rows)
            {
                if (dr[GroupingField].ToString() != currentValue)
                {
                    if (taskRow != null)
                    {
                        CreateItem(taskRow, ref sb);
                    }

                    taskRow = new TaskRow();
                    taskRow.Title = dr[TitleField].ToString();
                    if (TitleActionField != "")
                        taskRow.TitleAction = dr[TitleActionField].ToString();
                    currentValue = dr[GroupingField].ToString();
                }

                tri = new TaskRowItem();
                if (!string.IsNullOrEmpty(ToolTipField))
                    tri.ToolTip = dr[ToolTipField].ToString();

                tri.Action = string.Empty;
                tri.StartDate = (DateTime)dr[StartDateField];
                tri.EndDate = (DateTime)dr[EndDateField];
                taskRow.Items.Add(tri);
                
            }

            //string toolTip = "Holiday [ " + new DateTime(2007, 02, 02).ToString("dd/MM/yy") + " to " + new DateTime(2007, 01, 13).ToString("dd/MM/yy") + " ]";
            //TaskRow taskRow = new TaskRow("Greg Duffield", "#", toolTip, "#", new DateTime(2007, 02, 02), new DateTime(2007, 02, 05));
            //TaskRowItem tri = new TaskRowItem("#", "Working", new DateTime(2007, 01, 12), new DateTime(2007, 01, 13));
            //taskRow.Items.Add(tri);
            //CreateItem(taskRow, ref sb);

            //toolTip = "Testing";
            //taskRow = new TaskRow("A N Other", "#", toolTip, "#", new DateTime(2006, 12, 29), new DateTime(2007, 01, 10));
            //tri = new TaskRowItem("#", "Over the last day", new DateTime(2007, 01, 27), new DateTime(2007, 02, 12));
            //taskRow.Items.Add(tri);
            //CreateItem(taskRow, ref sb);

            #endregion

            #region Create the Footer

            sb.Append(headerTableFooterBegin);
            for (int i = 0; i < (7 * weeks) - 1; i++)
            {
                sb.Append(string.Format(headerTableFooterItem, ""));
            }

            sb.Append(headerTableFooterEnd);

            #endregion

            return sb.ToString();

        }

        private void CreateItem(TaskRow taskRow, ref StringBuilder sb)
        {
            DateTime workingDate = ColumnStartDate;

            // define the row
            TableRow tr = new TableRow();
            tr.CssClass = "p1-ganttTaskRow";
            tr.Height = new Unit("20px");


            // set the link/title for the row
            tr.Cells.Add(GetRowTitle(taskRow.Title, taskRow.TitleAction));

            // Generate the correct backgrounds
            TableCell td = null;
            for (int x = 0; x < DaysGenerated; x++)
            {
                td = new TableCell();
                workingDate = ColumnStartDate.AddDays(x);
                if (workingDate == DateTime.Today)
                    td.CssClass = "p1-GT";
                else if (workingDate.DayOfWeek == DayOfWeek.Saturday || workingDate.DayOfWeek == DayOfWeek.Sunday)
                    td.CssClass = "p1-GNWD";
                else
                    td.CssClass = "p1-GWD";
                tr.Cells.Add(td);
            }
            foreach (TaskRowItem tri in taskRow.Items)
            {
                if (tri.StartDate > ColumnEndDate)
                    continue;
                if (tri.EndDate < ColumnStartDate)
                    continue;
                // determine the starting cell and ending cell in the cells collection
                int startingColumn = tri.StartDate.Subtract(ColumnStartDate).Days + 1;
                if (startingColumn < 0)
                {
                    startingColumn = 1;
                    tr.Cells[startingColumn].Controls.Add(new LiteralControl(string.Format(itemImgMiddle, tri.ToolTip)));
                }
                else if (startingColumn >= 1)
                {
                    tr.Cells[startingColumn].Controls.Add(new LiteralControl(string.Format(itemImgStart, tri.ToolTip)));
                }

                int endingColumn = startingColumn + tri.EndDate.Subtract(tri.StartDate).Days;
                if (endingColumn > DaysGenerated)
                    endingColumn = DaysGenerated;

                // render out this date.
                for (int i = 1; i < (endingColumn - startingColumn); i++)
                {
                    tr.Cells[startingColumn + i].Controls.Add(new LiteralControl(string.Format(itemImgMiddle, tri.ToolTip)));
                }

                if ((startingColumn + tri.EndDate.Subtract(tri.StartDate).Days) > DaysGenerated)
                {
                    tr.Cells[endingColumn].Controls.Add(new LiteralControl(string.Format(itemImgMiddle, tri.ToolTip)));
                }
                else
                    tr.Cells[endingColumn].Controls.Add(new LiteralControl(string.Format(itemImgEnd, tri.ToolTip)));
            }

            // write out the html for rendering
            System.IO.StringWriter sr = new System.IO.StringWriter();
            HtmlTextWriter htr = new HtmlTextWriter(sr);
            tr.RenderControl(htr);
            sb.Append(htr.InnerWriter.ToString());

            sb.ToString();


        }


        private TableHeaderCell GetRowTitle(string title, string link)
        {
            TableHeaderCell th = new TableHeaderCell();
            th.Scope = TableHeaderScope.Row;
            th.Wrap = false;
            th.ToolTip = title;
            th.CssClass = "p1-ganttTitleCell";
            HtmlGenericControl div = new HtmlGenericControl();
            div.TagName = "div";
            div.Attributes.Add("class", "p1-ganttTitleCellText");
            HtmlAnchor a = new HtmlAnchor();
            a.HRef = link;
            a.InnerText = title;
            div.Controls.Add(a);
            th.Controls.Add(div);

            return th;
        }

        #endregion
    }

    internal class TaskRow
    {
        #region private fields
        private string _title = string.Empty;
        private string _titleAction = string.Empty;
        private List<TaskRowItem> _items = new List<TaskRowItem>();
        #endregion

        #region properties

        public string TitleAction
        {
            get { return _titleAction; }
            set { _titleAction = value; }
        }


        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public List<TaskRowItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }


        #endregion

        #region Constructors
        public TaskRow(string title, string titleAction, string itemToolTip, string itemAction, DateTime startDate, DateTime endDate)
        {
            TaskRowItem tri = new TaskRowItem(itemAction, itemToolTip, startDate, endDate);
            this._items.Add(tri);
            _title = title;
            _titleAction = titleAction;
        }

        public TaskRow(string title, string titleAction)
        {
            _title = title;
            _titleAction = titleAction;
        }

        public TaskRow() { }
        #endregion
    }

    internal class TaskRowItem
    {
        #region private fields
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today;
        private string _toolTip = string.Empty;
        #endregion

        #region properties
        private string _action;

        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value < _startDate)
                    throw new ArgumentOutOfRangeException("The End Date cannot be before the start date.");

                _endDate = value;

            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; }
        }

        #endregion

        #region Constructors
        public TaskRowItem() { }
        public TaskRowItem(string action, string toolTip, DateTime startDate, DateTime endDate)
        {
            _action = action;
            _toolTip = toolTip;
            _startDate = startDate;
            _endDate = endDate;
        }
        #endregion
    }
}