using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlClient;

namespace Orchestrator.WebServices
{
    /// <summary>
    /// Summary description for BatchUpload
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BatchUpload : System.Web.Services.WebService
    {

        public BatchUpload()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public int BatchUploadStart(string machineId, int numberOfScans)
        {
            int retVal = 0;

            
            string sqlINSERT = "INSERT tblScanUpload (MachineId, NumberOfScans, UploadStartDateTime ) VALUES('{0}', {1}, GETDATE()) SELECT @@IDENTITY";

            sqlINSERT = string.Format(sqlINSERT, machineId.ToString(), numberOfScans.ToString());
            using (SqlConnection sqlConn = new SqlConnection(Globals.Configuration.ConnectionString))
            {
                
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(sqlINSERT, sqlConn);
                object o = cmd.ExecuteScalar();
                retVal = int.Parse(o.ToString());
                sqlConn.Close();
            }

            return retVal;
        }

        [WebMethod]
        public int BatchUploadFinish(int scanUploadId)
        {
            int retVal = 0;
            
            string sqlUPDATE = "UPDATE tblScanUpload SET UploadFinishDateTime = GETDATE() WHERE ScanUploadId = {0}";
            sqlUPDATE = string.Format(sqlUPDATE, scanUploadId.ToString());

            using (SqlConnection sqlConn = new SqlConnection(Globals.Configuration.ConnectionString))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(sqlUPDATE, sqlConn);
                retVal = cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            return retVal;
        }

        [WebMethod]
        public int BatchUploadLogError(int scanUploadId, string errorDetails)
        {
            int retVal = 0;
            
            string sqlUPDATE = "UPDATE tblScanUpload SET IsErrors = 1, ErrorDetails = ISNULL(ErrorDetails, '') + '{1}\n' WHERE ScanUploadId = {0}";
            sqlUPDATE = string.Format(sqlUPDATE, scanUploadId.ToString(), errorDetails);
            using (SqlConnection sqlConn = new SqlConnection(Globals.Configuration.ConnectionString))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(sqlUPDATE, sqlConn);
                retVal = cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            return retVal;

        }

        [WebMethod]
        public int BatchUploadLog(int scanUploadId, string errorDetails)
        {
            int retVal = 0;

            string sqlUPDATE = "UPDATE tblScanUpload SET ErrorDetails = ISNULL(ErrorDetails, '') + '{1}\n' WHERE ScanUploadId = {0}";
            sqlUPDATE = string.Format(sqlUPDATE, scanUploadId.ToString(), errorDetails);
            using (SqlConnection sqlConn = new SqlConnection(Globals.Configuration.ConnectionString))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(sqlUPDATE, sqlConn);
                retVal = cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            return retVal;

        }


    }

}