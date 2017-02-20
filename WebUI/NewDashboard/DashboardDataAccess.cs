using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Orchestrator.Globals;
using Orchestrator.Exceptions;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Orchestrator.WebUI.NewDashboard
{
    public class DashboardDataAccess
    {
        public static DataSet getHomeDashboardDetails(int userID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@userId", userID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                return db.ExecuteDataSet("spDashboard_GetHomeDashboardDetails", userID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static DataSet getDashboardsForUser(int userID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@userId", userID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                return db.ExecuteDataSet("spDashboard_GetDashboardsForUser", userID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static DataSet getDBWidgetsForDashboardID(int dashboardID, int userID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@dashboardID", dashboardID), new SqlParameter("@userID", userID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                return db.ExecuteDataSet("spDashboard_GetDBWidgetsForDashboardID", dashboardID, userID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static DataSet getWidgetsNotOnDBForDBID(int dashboardID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@dashboardID", dashboardID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                return db.ExecuteDataSet("spDashboard_GetWidgetsNotOnDBForDBID", dashboardID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }

        }

        public static DataSet getModesForWidget(int widgetID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@widgetID", widgetID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                return db.ExecuteDataSet("spDashboard_GetModesForWidget", widgetID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }

        }

        public static void updateWidgetOnDashboardMode(int modeID, int widgetOnDashboardID)
        {
            try
            {
                //SqlParameter[] sqlParams = {new SqlParameter("@widgetOnDashboardID", widgetOnDashboardID), new SqlParameter("@modeID", modeID)  };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_UpdateWidgetOnDashboardMode", widgetOnDashboardID, modeID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static void updateWidgetPosition(int widgetOnDashboardID, int topPosistion, int leftPosistion)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@widgetOnDashboardID", widgetOnDashboardID), new SqlParameter("@topPos", topPosistion), new SqlParameter("@leftPos", leftPosistion) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_UpdateWidgetOnDashboardPosistion", widgetOnDashboardID, topPosistion, leftPosistion); 
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static void updateWidgetSize(int widgetOnDashboardID, int height, int width)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@widgetOnDashboardID", widgetOnDashboardID), new SqlParameter("@width", width), new SqlParameter("@height", height) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_UpdateWidgetOnDashboardSize", widgetOnDashboardID, width, height);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static void updateDashboardName(string dashboardName, int dashboardID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@dashboardName", dashboardName), new SqlParameter("@dashboardID", dashboardID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_UpdateDashboardName", dashboardName, dashboardID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static void makeHomeDashboard(int dashboardID, int userID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@dashboardID", dashboardID), new SqlParameter("@userID", userID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_MakeDashboardHome", dashboardID, userID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static void addWidgetToDashboard(int widgetID, int dashboardID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@widgetID", widgetID), new SqlParameter("@dashboardID", dashboardID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_InsertWidgetOnDashboard", widgetID, dashboardID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static DataSet insertNewDashboard(int userID, String dashboardName)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@userID", userID), new SqlParameter("@dashboardName", dashboardName) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                return db.ExecuteDataSet("spDashboard_InsertNewDashboard", userID, dashboardName);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static void deleteWidgetFromDB(int widgetOnDashboardID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@WidgetOnDashboardID", widgetOnDashboardID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_DeleteWidgetFromDashboard", widgetOnDashboardID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static void deleteDashboard(int dashboardID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@dashboardID", dashboardID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                db.ExecuteNonQuery("spDashboard_DeleteDashboard", dashboardID);
            }
            catch (Exception e)
            {
                throw new DataAccessException("Problem executing query.", e);
            }
        }

        public static bool homeDashboard(int dashboardID)
        {
            try
            {
                //SqlParameter[] sqlParams = { new SqlParameter("@dashboardID", dashboardID) };
                var db = new SqlDatabase(Configuration.ConnectionString);
                DataSet homeDB = db.ExecuteDataSet("spDashboard_CheckIfHomeDB", dashboardID);
                return (bool)homeDB.Tables[0].Rows[0]["homeDashboard"];

            }
            catch (Exception e)
            {
                throw new DataException("Problem executing query.", e);
            }
        }

        public static int getFleetIdling(DateTime startDate, DateTime endDate)
        {
            try
            {
                SqlConnection conn = new SqlConnection(Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand("spCAN_GetFleetIdlingTime", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                SqlParameter startDateP = new SqlParameter("@FromDate", SqlDbType.DateTime);
                startDateP.Value = startDate;
                startDateP.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(startDateP);
                
                SqlParameter endDateP = new SqlParameter("@ToDate", SqlDbType.DateTime);
                endDateP.Value = endDate;
                endDateP.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(endDateP);

                SqlParameter idlingSeconds = new SqlParameter("@IdlingSeconds", SqlDbType.Int);
                idlingSeconds.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(idlingSeconds);

                SqlParameter durationSeconds = new SqlParameter("@DurationSeconds", SqlDbType.Int);
                durationSeconds.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(durationSeconds);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                int idlingSecs = (int)cmd.Parameters["@IdlingSeconds"].Value;
                int durationSecs = (int)cmd.Parameters["@DurationSeconds"].Value;
                if (durationSecs == 0)
                {
                    return 0;
                }
                else
                {
                    decimal fleetIdling = Decimal.Divide(idlingSecs,durationSecs) * 100;
                    return (int) fleetIdling;
                }
                    
            }
            catch (Exception e)
            {
                throw new DataException("Problem executing query.", e);
            }
        }

    }
}