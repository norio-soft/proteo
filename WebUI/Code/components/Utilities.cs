using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using Orchestrator.Globals;
using Orchestrator.WebUI.textanywhere.ws;

using System.Net;
using System.Net.Mail;

using Telerik.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;
using System.Runtime.Serialization;
using System.Reflection;

using Newtonsoft.Json;

namespace Orchestrator.WebUI
{
	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
	public class Utilities
	{
		public Utilities(){}

		public static System.Exception LastError ;
		public static string FocusScript = "<script language=\"javascript\">document.all['{0}'].focus();</script>";

        #region Custom Validation

        public static bool ValidateDateCannotBeGreaterThanToday(string objectValue)
		{
			try 
			{
				DateTime dateTime = DateTime.Parse(objectValue);
				if (dateTime > DateTime.UtcNow)
					return false;
				else
					return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public static bool ValidateDateCanBeGreaterThanToday(string objectValue)
		{
			try 
			{
				DateTime dateTime = DateTime.Parse(objectValue);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
        public static bool ValidateNumericValue(string objectValue)
        {
            if (objectValue == string.Empty)
                return false;

            try
            {
                int value = int.Parse(objectValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

		#endregion

		#region Job Colours

		public static Color GetJobStateColour(eJobState jobState)
		{
			switch (jobState)
			{
				case eJobState.Booked:
					return Color.White;
				case eJobState.Planned:
					return Color.FromName("CCFFCC");
				case eJobState.InProgress:
					return Color.FromName("99FF99");
				case eJobState.Completed:
					return Color.LightBlue;
				case eJobState.BookingInIncomplete:
					return Color.MistyRose;
				case eJobState.BookingInComplete:
					return Color.PaleVioletRed;
				case eJobState.ReadyToInvoice:
                    return Color.Yellow;
				case eJobState.Invoiced:
					return Color.Gold;
				case eJobState.Cancelled:
					return Color.Khaki;
				default:
					return Color.Empty;
			}
		}

        public static string GetJobStateColourForHTML(eJobState jobState)
        {
            Color colour = GetJobStateColour(jobState);

            byte zeroValue = 0;

            if (colour.R == zeroValue && colour.G == zeroValue && colour.B == zeroValue)
            {
                if (colour.Name.Length > 0)
                    return "#" + colour.Name;
                else
                    return "#000000";
            }
            else
                return "#" + ConvertToHexString(colour.R, 2) + ConvertToHexString(colour.G, 2) + ConvertToHexString(colour.B, 2);
        }

        public static Color GetOrderStateColour(eOrderStatus orderStatus, bool flaggedReadyForInvoicing)
        {
            switch (orderStatus)
            {
                case eOrderStatus.Approved:
                    return Color.White;
                case eOrderStatus.Delivered:
                    if (flaggedReadyForInvoicing)
                        return Color.Yellow;
                    else
                        return Color.LightBlue;
                case eOrderStatus.Invoiced:
                    return Color.Gold;
                case eOrderStatus.Cancelled:
                    return Color.Khaki;
                default:
                    return Color.Empty;
            }
        }

        public static string GetOrderStateColourForHTML(eOrderStatus orderStatus, bool flaggedReadyForInvoicing)
        {
            Color colour = GetOrderStateColour(orderStatus, flaggedReadyForInvoicing);

            byte zeroValue = 0;

            if (colour.R == zeroValue && colour.G == zeroValue && colour.B == zeroValue)
            {
                if (colour.Name.Length > 0)
                    return "#" + colour.Name;
                else
                    return "#000000";
            }
            else
                return "#" + ConvertToHexString(colour.R, 2) + ConvertToHexString(colour.G, 2) + ConvertToHexString(colour.B, 2);
        }

	    private static string ConvertToHexString(byte value, int requiredLength)
        {
            string working = Microsoft.VisualBasic.Conversion.Hex(value);

            while (working.Length < requiredLength)
                working = "0" + working;

            return working;
        }

		#endregion

		#region Activity Colours

		public static Color GetActivityColour(eResourceType resourceType, int activityTypeId)
		{
			switch (resourceType)
			{
				case eResourceType.Driver:
					return GetDriverActivityColour((eDriverActivityType) activityTypeId);
				case eResourceType.Vehicle:
					return GetVehicleActivityColour((eVehicleActivityType) activityTypeId);
				case eResourceType.Trailer:
					return GetTrailerActivityColour((eTrailerActivityType) activityTypeId);
				default:
					return Color.Empty;
			}
		}

		public static Color GetDriverActivityColour(eDriverActivityType driverActivityType)
		{
			switch (driverActivityType)
			{
				case eDriverActivityType.Free:
					return Color.White;
				case eDriverActivityType.Tentative:
					return Color.Yellow;
				case eDriverActivityType.Busy:
					return Color.Green;
				default:
					return Color.Empty;
			}
		}

		public static Color GetVehicleActivityColour(eVehicleActivityType vehicleActivityType)
		{
			switch (vehicleActivityType)
			{
				case eVehicleActivityType.Free:
					return Color.White;
				case eVehicleActivityType.Tentative:
					return Color.Yellow;
				case eVehicleActivityType.Busy:
					return Color.Green;
				default:
					return Color.Empty;
			}
		}

		public static Color GetTrailerActivityColour(eTrailerActivityType trailerActivityType)
		{
			switch (trailerActivityType)
			{
				case eTrailerActivityType.Free:
					return Color.White;
				case eTrailerActivityType.Tentative:
					return Color.Yellow;
				case eTrailerActivityType.Busy:
					return Color.Green;
				default:
					return Color.Empty;
			}
		}

		#endregion

		#region String Manipulation

        public static IDictionary<int, string> GetEnumPairs<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            var t = typeof(TEnum);

            if (!t.IsEnum)
                throw new ArgumentException("TEnum must be an enumerated type");

            return Enum.GetValues(t).Cast<TEnum>().ToDictionary(
                ct => Convert.ToInt32(ct),
                ct => Utilities.UnCamelCase(Enum.GetName(t, ct)));
        }

        public static T SelectedEnumValue<T>(string val)
	    {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Must be an enum.", "T");
            else if (string.IsNullOrEmpty(val))
                throw new ArgumentException("Must supply a selected value", "val");

            return (T)Enum.Parse(typeof(T), val.Replace(" ", ""));
    	}

		public static string UnCamelCase(string val)
		{
			string[] vals = { val };
			vals = UnCamelCase(vals);
			return vals[0];
		}

		public static string[] UnCamelCase(string[] vals)
		{
			Regex camelSplitter = new Regex("[A-Z_]");
			
			for (int i = 0; i < vals.Length; i++)
			{
                MatchCollection caps = camelSplitter.Matches(vals[i]);
                
                if (caps.Count > 0)
                {
                    string[] humps = camelSplitter.Split(vals[i]);

                    vals[i] = humps[0];

                    for (int j = 0; j < caps.Count; j++)
                    {
                        var cap = caps[j].Value;
                        vals[i] += " " + (cap == "_" ? string.Empty : cap) + humps[j + 1];
                    }

                    vals[i] = vals[i].TrimStart();
                }
			}

			return vals;
		}

		#endregion

		#region TextAnywhere Methods

		public static eTextAnywhereSendSMS SendSMS(string clientReference, string billingReference, eTextAnywhereOriginatorType originatorType, string recipientsNumber, string messageText, string user)
		{
			TextAnywhere_SMS myTextAnywhere_SMS = new TextAnywhere_SMS();

			string formattedNumber = myTextAnywhere_SMS.FormatNumber(recipientsNumber);

			eTextAnywhereSendSMS wsResult = (eTextAnywhereSendSMS) myTextAnywhere_SMS.SendSMS(
				Orchestrator.Globals.Configuration.TextAnywhereUsername,
				Orchestrator.Globals.Configuration.TextAnywherePassword,
				clientReference,
				Orchestrator.Globals.Configuration.TextAnywhereOriginator,
				Int32.Parse(Orchestrator.Globals.Configuration.TextAnywhereConnection),
				Configuration.TextAnywhereOriginator,
				(int) originatorType,
				formattedNumber,
				messageText,
				0,
				0);

			if (wsResult == eTextAnywhereSendSMS.SMSSent)
			{
				// Log the message data for audit trail
				Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();
				facDriverCommunication.RecordSMS(clientReference, billingReference, recipientsNumber, messageText, user);
			}

			return wsResult;
		}

		public static eTextAnywhereSMSStatus SMSStatus(string clientReference)
		{
			TextAnywhere_SMS myTextAnywhere_SMS = new TextAnywhere_SMS();

			eTextAnywhereSMSStatus wsResult = (eTextAnywhereSMSStatus) myTextAnywhere_SMS.SMSStatus(
				Orchestrator.Globals.Configuration.TextAnywhereUsername,
				Orchestrator.Globals.Configuration.TextAnywherePassword,
				clientReference);

			return wsResult;
		}

		#endregion

		#region Pegasus Licensing

		public static string GetPegasusLicenseKey()
		{
			string webKey = String.Empty;

			string requestUrl = HttpContext.Current.Request.Url.ToString().ToLower();

			if (requestUrl.IndexOf("p1.Orchestrator.co.uk") > 1)
				webKey = "p1.Orchestrator.co.uk?B882F2B87F3323493EF4FA8F42085174";
			else if (requestUrl.IndexOf("hms.Orchestrator.co.uk") > -1)
				webKey = "hms.Orchestrator.co.uk?D196E6D44F66C9D4CF18EC83D36113E0";
			else if (requestUrl.IndexOf("localhost") > -1)
				webKey = "localhost?C0D75518493F4FD41777E366C3EDE4FC";
			else if (requestUrl.IndexOf("hmstest.Orchestrator.co.uk") > -1)
				webKey = "hmstest.Orchestrator.co.uk?A4358745B6B0AC2F7700070E51FD5E98";;

			return webKey;
		}

        #endregion

        #region Date Functions
        /// <summary>
        /// Get the start of the week for the specified date
        /// From http://stackoverflow.com/a/38064
        /// </summary>
        /// <param name="dt">Date</param>
        /// <param name="startOfWeek">DayOfWeek.Monday, DayOfWeek.Sunday</param>
        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
        #endregion DateFunctions

        public static string GetVersionNumber(System.Web.Caching.Cache cache)
        {
            const string versionNumberCacheKey = "versionNumber";
            string versionNumber = string.Empty;

            try
            {
                versionNumber = (string)cache.Get(versionNumberCacheKey);
            }
            catch { }

            if (string.IsNullOrEmpty(versionNumber))
            {
                const string style = "color:{0};font-weight:{1};";
                string colour = "white";
                string weight = "normal";

                System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                if (executingAssembly != null)
                {
                    System.Reflection.AssemblyName executingAssemblyName = executingAssembly.GetName();
                    if (executingAssemblyName != null)
                        versionNumber = executingAssemblyName.Version.Major + "." + executingAssemblyName.Version.Minor + "." + executingAssemblyName.Version.Build + " ";
                }

                #if DEBUG
                versionNumber += "DEBUG";
                // Two attributes below are Handled via CSS
                colour = "bddbf2";
                weight = "normal";

                // Add the database name we are currently connected to.
                string databaseName = "unknown";
                try
                {
                    using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString))
                    {
                        databaseName = con.Database;
                    }
                }
                catch {}
                versionNumber = string.Format("{0} <span>{1}</span>", versionNumber, databaseName);
                #endif

                if (versionNumber != string.Empty)
                {
                    versionNumber = "<span style=\"" + string.Format(style, colour, weight) + "\">v" + versionNumber + "</span>";
                    try
                    {
                        cache.Insert(versionNumberCacheKey,
                            versionNumber);
                    }
                    catch { }
                }
            }

            return versionNumber;
        }

        public static bool MapSqlException(SqlException sqlException)
        {
            switch (sqlException.Number)
            {
                case -2: /* Client Timeout */
                case 701: /* Out of Memory */
                case 1204: /* Lock Issue */
                case 1205: /* Deadlock Victim */
                case 1222: /* Lock Request Timeout */
                case 8645: /* Timeout waiting for memory resource */
                case 8651: /* Low memory condition */
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Clears all the session variables ever used in the invoicing section.
        /// </summary>
        public static void ClearInvoiceSession()
        {
            HttpContext.Current.Session["ExtraIdCSV"] = null;
            HttpContext.Current.Session["IdentityId"] = null;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = null;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = null;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = null;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = null;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = null;
            HttpContext.Current.Session["_selectedExtras"] = null;
            HttpContext.Current.Session["_selectedExtraIdentityId"] = null;
            HttpContext.Current.Session["JobIds"] = null;
            HttpContext.Current.Session["ExtraIds"] = null;
            HttpContext.Current.Session["FileLocation"] = null;
            HttpContext.Current.Session["FileName"] = null;
            HttpContext.Current.Session["BatchId"] = null;
            HttpContext.Current.Session["ClientId"] = null;
            HttpContext.Current.Session["ClientName"] = null;
            HttpContext.Current.Session["StartDate"] = null;
            HttpContext.Current.Session["EndDate"] = null;
            HttpContext.Current.Session["ExportCSV"] = null;
            HttpContext.Current.Session["Type"] = null;
            HttpContext.Current.Session["__ExportDS"] = null;
            HttpContext.Current.Session["InvoiceReport"] = null;
            HttpContext.Current.Session["Batches"] = null;
            //#15857 J.Steele Added BatchId
            HttpContext.Current.Session["BatchId"] = null;

            HttpContext.Current.Session["jobSubTotal"] = null;
            HttpContext.Current.Session["dsExtras"] = null;
        }

        #region SendSupportEmail Helper

        private static string UserDataInformation(Exception exception)
        {
            XmlSerializer xmls;
            StringWriter strWriter = null;
            StringBuilder sbInformation = new StringBuilder();

            sbInformation.Append("Type: ");
            sbInformation.Append(exception.GetType().Name);
            sbInformation.Append(Environment.NewLine);
            sbInformation.Append("Message: ");
            sbInformation.Append(exception.Message);
            sbInformation.Append(Environment.NewLine);
            sbInformation.Append("Stack Trace: ");
            sbInformation.Append(exception.StackTrace);
            sbInformation.Append(Environment.NewLine);
            sbInformation.Append("Source: ");
            sbInformation.Append(exception.Source);
            sbInformation.Append(Environment.NewLine);
            if (exception.GetType().IsSerializable)
            {
                try
                {
                    sbInformation.Append("   Serialized as: ");
                    sbInformation.Append(Environment.NewLine);
                    xmls = new XmlSerializer(exception.GetType());
                    strWriter = new StringWriter();
                    xmls.Serialize(strWriter, exception);
                    sbInformation.Append(strWriter.ToString());
                }
                catch (Exception exc)
                {
                    sbInformation.Append("   SERIALIZATION FAILED");
                }
                finally
                {
                    if (strWriter != null)
                    {
                        strWriter.Close();
                        strWriter = null;
                    }
                    sbInformation.Append(Environment.NewLine);
                }
            }

            if (exception.InnerException != null)
            {
                sbInformation.Append(Environment.NewLine);
                sbInformation.Append(Environment.NewLine);
                sbInformation.Append(Environment.NewLine);
                sbInformation.Append(Environment.NewLine);
                sbInformation.Append(UserDataInformation(exception.InnerException));
            }

            return sbInformation.ToString();
        }

        public static void SendSupportEmailHelper(string methodCalled, Exception exception)
        {
            try
            {
                // Configuration or service availablity error - email support.
                Orchestrator.WebUI.Alerts.Email email = new Orchestrator.WebUI.Alerts.Email();

                StringBuilder sbMessage = new StringBuilder();
                sbMessage.Append("Error calling ");
                sbMessage.Append(methodCalled);
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append("Host Machine: ");
                sbMessage.Append(Environment.MachineName);
                sbMessage.Append(Environment.NewLine);
                sbMessage.Append(Environment.NewLine);

                sbMessage.Append(UserDataInformation(exception));
                email.SendSupportEmail("Error calling " + methodCalled, sbMessage.ToString(), string.Empty);
            }
            catch
            {
            }
        }

        #endregion

        #region Cookie Handling

        static readonly string C_TRAFFIC_SHEET_JSON = "TrafficSheetFilterJSON";

        public static Entities.TrafficSheetFilter GetFilterFromCookie(string sessionID, System.Web.HttpRequest Request)
        {

            Entities.TrafficSheetFilter f = null;
            try
            {
                string trafficSheetJSON = Request.Cookies[sessionID][C_TRAFFIC_SHEET_JSON];

               

                if (Request.Cookies[sessionID] != null && trafficSheetJSON != null)
                {
                    if (trafficSheetJSON.Length > 0)
                    {
                        f =  JsonConvert.DeserializeObject<Entities.TrafficSheetFilter>(trafficSheetJSON);
                       
                    }
                }

                return f;
            }
            catch (Exception ex)
            {
                
            }
            

            return f;
        }


        public static void SetTrafficSheetCookie(string sessionID, System.Web.HttpResponse response, Entities.TrafficSheetFilter filter)
        {
            HttpCookie tSheetCookie = new HttpCookie(sessionID);
            string trafficSheetFilterJSON;


            trafficSheetFilterJSON = JsonConvert.SerializeObject(filter);


            tSheetCookie[C_TRAFFIC_SHEET_JSON] = trafficSheetFilterJSON;

            // Settings option to have the cookie expire daily or never
            if (Orchestrator.Globals.Configuration.ResetTrafficCookieDaily)
            {
                tSheetCookie.Expires = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);
            }
            else
            {
                tSheetCookie.Expires = new DateTime(2099, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);
            }

            response.SetCookie(tSheetCookie);
        }

        public static Entities.TrafficSheetFilter GenerateCookie(string sessionID, HttpResponse response, int identityID)
        {
            Entities.TrafficSheetFilter _filter = new Orchestrator.Entities.TrafficSheetFilter();
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            _filter = facTrafficSheetFilter.GetDefault(identityID);
         
            #region // Configure the default dates.
            // Default dates are from the start of today until:
            //   1) On a Saturday, until the end of Monday.
            //   2) On any other day, until the end of tomorrow.
            DateTime startOfToday = DateTime.Now;
            startOfToday = startOfToday.Subtract(startOfToday.TimeOfDay);
            DateTime endOfTomorrow = startOfToday.Add(new TimeSpan(1, 23, 59, 59));
            _filter.FilterStartDate = startOfToday;

            //ViewState["m_startDate"] = startOfToday;
            if (startOfToday.DayOfWeek == DayOfWeek.Saturday)
            {
                DateTime endOfMonday = startOfToday.Add(new TimeSpan(2, 23, 59, 59));
                //ViewState["m_endDate"] = endOfMonday;
                _filter.FilterEnddate = endOfMonday;
            }
            else
                _filter.FilterEnddate = endOfTomorrow;
            //ViewState["m_endDate"] = endOfTomorrow;

            #endregion

            return _filter;
        }

        public static string GetRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        #endregion

        #region Grid State Persisting
        
        [DataContract]
        private class GridColumnState
        {
            [DataMember(Name="v")]
            public bool Visible { get; set; }
            [DataMember(Name = "w")]
            public Unit Width { get; set; }
            [DataMember(Name = "o")]
            public int OrderIndex { get; set; }
        }

        [DataContract]
        private class GridState
        {
            [DataMember(Name = "g")]
            public IEnumerable<string> GroupByExpressionViewStates { get; set; }
            [DataMember(Name = "s")]
            public string SortExpressionsViewState { get; set; }
            [DataMember(Name = "f")]
            public string FilterExpression { get; set; }
            [DataMember(Name = "c")]
            public IDictionary<string, GridColumnState> ColumnStates { get; set; }

            public string Serialize()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            public static GridState Deserialize(string serialized)
            {
                GridState retVal = null;

                // Swallow any exception (in order to return null) if the string is not a valid serialized GridState object
                try { retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<GridState>(serialized); } catch { }

                return retVal;
            }
        }

        public static void SaveGridSettings(Telerik.Web.UI.RadGrid gridInstance, eGrid grid, string userName)
        {
            var masterTable = gridInstance.MasterTableView;
            var groupByExpressions = masterTable.GroupByExpressions.Cast<GridGroupByExpression>();
            var allColumns = masterTable.Columns.Cast<GridColumn>().Concat(masterTable.AutoGeneratedColumns);

            var losFormatter = new LosFormatter();

            var serializeViewState = new Func<IStateManager, string>(sm =>
            {
                using (var writer = new StringWriter())
                {
                    losFormatter.Serialize(writer, sm.SaveViewState());
                    return writer.ToString();
                }
            });

            var gridState = new GridState
            {
                GroupByExpressionViewStates = groupByExpressions.Select(gbe => serializeViewState(gbe)),
                SortExpressionsViewState = serializeViewState(masterTable.SortExpressions),
                FilterExpression = masterTable.FilterExpression,
                ColumnStates = allColumns.ToDictionary(
                    c => c.UniqueName,
                    c => new GridColumnState { Visible = c.Visible, Width = c.HeaderStyle.Width, OrderIndex = c.OrderIndex }),
            };

            Orchestrator.DataAccess.SystemSettings.SaveGridSettings(userName, (int)grid, gridState.Serialize(), null);
        }

        public static void ResetGridSettings(eGrid grid, string userName)
        {
            Orchestrator.DataAccess.SystemSettings.SaveGridSettings(userName, (int)grid, string.Empty, string.Empty);
        }

        public static void LoadSettings(Telerik.Web.UI.RadGrid gridInstance, eGrid grid, string userName)
        {
            IEnumerable<string> columnsToHide;
            LoadSettings(gridInstance, grid, out columnsToHide, userName);
        }

        public static void LoadSettings(Telerik.Web.UI.RadGrid gridInstance, eGrid grid, out IEnumerable<string> columnsToHide, string userName)
        {
            var settingsRow = DataAccess.SystemSettings.GetGridSettings(userName, (int)grid).Tables[0].AsEnumerable().FirstOrDefault();
            
            var settings = string.Empty;
            columnsToHide = Enumerable.Empty<string>();

            if (settingsRow != null)
            {
                if (!settingsRow.IsNull("DATA"))
                    settings = settingsRow.Field<string>("DATA");

                if (!settingsRow.IsNull("ColumnsToHide"))
                    columnsToHide = settingsRow.Field<string>("ColumnsToHide").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            var masterTable = gridInstance.MasterTableView;
            var allColumns = masterTable.Columns.Cast<GridColumn>().Concat(masterTable.AutoGeneratedColumns);

            if (!string.IsNullOrWhiteSpace(settings) || columnsToHide.Any())
            {
                var gridState = GridState.Deserialize(settings);

                #region Old Load Method

                // The following 5 lines are only in place to allow people to upgrade from any older version of HE to the latest version without losing their saved grid settings.
                // For any future release of HE these saved settings will have been migrated to the new format so these lines can be removed.
                if (gridState == null)
                {
                    LoadSettings_OldMethod(gridInstance, settings, columnsToHide);
                    SaveGridSettings(gridInstance, grid, userName);
                    return;
                }

                #endregion Old Load Method

                var losFormatter = new LosFormatter();

                var deserializeViewState = new Action<string, IStateManager>((s, sm) =>
                {
                    using (var reader = new StringReader(s))
                    {
                        try
                        {
                            var viewState = losFormatter.Deserialize(reader);
                            sm.LoadViewState(viewState);
                        }
                        catch
                        {
                            // Don't throw an exception if the grid data can't be deserialised... sometimes this can happen if the Telerik controls have been updated to a more recent version.
                        }
                    }
                });

                // Load group by expressions
                var groupByExpressions = masterTable.GroupByExpressions;
                groupByExpressions.Clear();

                foreach (var vs in gridState.GroupByExpressionViewStates)
                {
                    var expression = new GridGroupByExpression();
                    deserializeViewState(vs, expression);
                    groupByExpressions.Add(expression);
                }

                // Load sort expressions
                var sortExpressions = masterTable.SortExpressions;
                sortExpressions.Clear();
                deserializeViewState(gridState.SortExpressionsViewState, sortExpressions);

                // Load filter expression
                masterTable.FilterExpression = gridState.FilterExpression;

                foreach (var column in allColumns)
                {
                    if (gridState.ColumnStates.ContainsKey(column.UniqueName))
                    {
                        var columnState = gridState.ColumnStates[column.UniqueName];
                        column.OrderIndex = columnState.OrderIndex;
                        column.ItemStyle.Width = column.HeaderStyle.Width = columnState.Width;
                        column.Visible = columnState.Visible;
                    }
                }
            }

            columnsToHide = from c in allColumns where !c.Visible select c.UniqueName;
        }

        private static void LoadSettings_OldMethod(Telerik.Web.UI.RadGrid gridInstance, string settings, IEnumerable<string> columnsToHide)
        {
            var columnOrdersAndWidths = Enumerable.Empty<Pair>();
            var visibleColumns = Enumerable.Empty<bool>();

            if (!string.IsNullOrWhiteSpace(settings))
            {
                object[] gridSettings;

                using (var reader = new StringReader(settings))
                {
                    var formatter = new LosFormatter();
                    gridSettings = (object[])formatter.Deserialize(reader);
                }

                if (gridSettings.Count() == 5)
                {
                    //Load groupBy
                    var groupByExpressions = gridInstance.MasterTableView.GroupByExpressions;
                    groupByExpressions.Clear();

                    var savedGroupByExpressions = (object[])gridSettings[0];

                    foreach (var obj in savedGroupByExpressions)
                    {
                        var expression = new GridGroupByExpression();
                        ((IStateManager)expression).LoadViewState(obj);
                        groupByExpressions.Add(expression);
                    }

                    //Load sort expressions
                    var sortExpressions = gridInstance.MasterTableView.SortExpressions;
                    sortExpressions.Clear();
                    ((IStateManager)sortExpressions).LoadViewState(gridSettings[1]);

                    columnOrdersAndWidths = (IEnumerable<Pair>)gridSettings[2];
                    visibleColumns = ((IEnumerable)gridSettings[4]).Cast<bool>();

                    //Load filter expression
                    gridInstance.MasterTableView.FilterExpression = (string)gridSettings[3];
                }
            }

            var allColumns = gridInstance.MasterTableView.Columns.Cast<GridColumn>().Concat(gridInstance.MasterTableView.AutoGeneratedColumns);

            if (allColumns.Count() == columnOrdersAndWidths.Count())
            {
                int i = 0;

                foreach (var column in allColumns)
                {
                    var columnOrderAndWidth = columnOrdersAndWidths.ElementAt(i);
                    column.OrderIndex = (int)columnOrderAndWidth.First;
                    column.ItemStyle.Width = column.HeaderStyle.Width = (Unit)columnOrderAndWidth.Second;
                    column.Visible = visibleColumns.ElementAt(i);
                    i++;
                }
            }
            else
            {
                // Column count doesn't match saved data, so use columnsToHide to set column visibility instead
                foreach (var column in allColumns)
                {
                    column.Visible &= !columnsToHide.Contains(column.UniqueName);
                }
            }
        }

        #endregion Grid State Persisting

        public static bool SendEmail(Dictionary<string, string> recipients, string title, string body, Dictionary<string, byte[]> attachments)
        {
             try
            {
                MailMessage mailMessage = new System.Net.Mail.MailMessage();

                mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress,
                    Orchestrator.Globals.Configuration.MailFromName);

                foreach (KeyValuePair<string, string> di in recipients)
                {
                    mailMessage.To.Add(new MailAddress(di.Value, di.Key));
                }

                if(attachments != null)
                    foreach (KeyValuePair<string, byte[]> attachment in attachments)
                    {
                        mailMessage.Attachments.Add(new Attachment(
                            new MemoryStream(attachment.Value), attachment.Key));
                    }

                mailMessage.Subject = title;
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = body;

                SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = Globals.Configuration.MailServer;
                smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername, Globals.Configuration.MailPassword);
                smtp.Send(mailMessage);

                mailMessage.Dispose();

                return true;
            }
            catch { }
             return false;
        }



        #region Nullable Parse

        public static Nullable<T> ParseNullable<T>(string input)
            where T : struct
        {
            Nullable<T> retVal = null;

            if (!string.IsNullOrEmpty(input))
            {
                if (typeof(T).IsEnum)
                {
                    try
                    {
                        retVal = (T)Enum.Parse(typeof(T), input);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
                else if (typeof(T) == typeof(Guid))
                {
                    try
                    {
                        retVal = (T)(object)new Guid(input);
                    }
                    catch (FormatException)
                    {
                    }
                }
                else if (typeof(T) == typeof(TimeSpan))
                {
                    TimeSpan ts;
                    if (TimeSpan.TryParse(input, out ts))
                        retVal = (T)(object)ts;
                }
                else
                {
                    try
                    {
                        retVal = (T)((IConvertible)input).ToType(typeof(T), System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            return retVal;
        }


        #endregion Nullable Parse

        #region Allocation

        public static bool IsAllocationEnabled()
        {
            Facade.IAllocation facAllocation = new Facade.Allocation();
            return facAllocation.IsAllocationEnabled;
        }

        #endregion Allocation

        public static List<int> ExtractCheckBoxListValues(CheckBoxList checkBoxList)
        {
            var resultingIDs = new List<int>();
            foreach (ListItem item in checkBoxList.Items)
            {
                if (item.Selected)
                {
                    resultingIDs.Add(int.Parse(item.Value));
                }
            }
            return resultingIDs;
        }

        public static Uri GetSignatureImageBaseUri()
        {
            return Orchestrator.Globals.Configuration.SignatureImageBaseUri;
        }

        internal static void ExportReportAsPdf(Telerik.Reporting.InstanceReportSource instanceReportSource, HttpResponse response, string fileName)
        {
            var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
            var deviceInfo = new System.Collections.Hashtable();

            var result = reportProcessor.RenderReport("PDF", instanceReportSource, deviceInfo);

            response.Clear();
            response.ContentType = "application/pdf";
            response.BufferOutput = true;
            response.AppendHeader("accept-ranges", "none");
            response.AppendHeader("content-disposition", string.Concat("attachment; filename=", fileName));
            response.BinaryWrite(result.DocumentBytes);
            response.Flush();
            response.End();
        }

    }

       
    public interface IDefaultButton
    {
        System.Web.UI.Control DefaultButton {get;}
    }

    public static class ExtensionMethods
    {

        #region Enum Helpers

        // an extension to retrieve enum attributes
        public static TAttribute GetAttribute<TAttribute>(this Enum @this)
            where TAttribute : Attribute
        {
            var field = @this.GetType().GetField(
                @this.ToString(),
                BindingFlags.Public | BindingFlags.Static
            );
            return field.GetCustomAttribute<TAttribute>();
        }

        #endregion

        public static T CastByExample<T>(this object o, T example)
        {
            return (T)o;
        }

        public static string ToCSVString<T>(this List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in list)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(item.ToString());
            }

            return sb.ToString();

        }


         public static string ToCSV(this DataTable dataTable, IEnumerable<string> ignoreColumnNames)
         {

             StringBuilder sb = new StringBuilder();

             var columns = dataTable.Columns.Cast<DataColumn>().Where(ch => !ignoreColumnNames.Contains(ch.ColumnName)).ToList();
             var columnHeaders = columns.Select(ch => ch.ColumnName).ToArray();
             sb.AppendLine(string.Join(",", columnHeaders));             

             foreach (DataRow row in dataTable.Rows)
             {
                 var rowString = string.Join(",", columns.Select(c => row[c].ToString()).ToArray());
                 sb.AppendLine(rowString);
             }

             return sb.ToString();

         }

    }

}
