using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.CAN
{
    public partial class AddUpdateDriverGrading : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnSaveChanges.Click += new EventHandler(btnSaveChanges_Click);
            if (!this.IsPostBack)
            {
                LoadData();
            }

        }

        void btnSaveChanges_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        void LoadData()
        {
            var driverGrading = EF.DataContext.Current.CANDriverGradings.FirstOrDefault(dg => dg.IsEnabled == true);

            if (driverGrading == null)
            {
                txtDistancePoints.Text = string.Empty;
                txtIdlingPoints.Text = string.Empty;
                txtBrakingPoints.Text = string.Empty;
                txtSpeedingPoints.Text = string.Empty;
                txtOverRevvingPoints.Text = string.Empty;
                txtUneconomicalPoints.Text = string.Empty;
            }
            else
            {
                txtDistancePoints.Value = (double)driverGrading.DistanceTravelledPoints;
                txtIdlingPoints.Value = (double)driverGrading.IdlingPoints;
                txtBrakingPoints.Value = (double)driverGrading.BrakingPoints;
                txtSpeedingPoints.Value = (double)driverGrading.SpeedingPoints;
                txtOverRevvingPoints.Value = (double)driverGrading.OverRevvingPoints;
                txtUneconomicalPoints.Value = (double)driverGrading.UneconomicalPoints;
            }
        }

        void SaveData()
        {
            //Disable the old one if it exists
            var oldDriverGrading = EF.DataContext.Current.CANDriverGradings.FirstOrDefault(dg => dg.IsEnabled == true);
            if (oldDriverGrading != null)
                oldDriverGrading.IsEnabled = false;

            //Create a new one
            var newDriverGrading = new EF.CANDriverGrading();
            newDriverGrading.IsEnabled = true;
            newDriverGrading.DistanceTravelledPoints = (decimal)txtDistancePoints.Value;
            newDriverGrading.IdlingPoints = (decimal)txtIdlingPoints.Value;
            newDriverGrading.BrakingPoints = (decimal)txtBrakingPoints.Value;
            newDriverGrading.SpeedingPoints = (decimal)txtSpeedingPoints.Value;
            newDriverGrading.OverRevvingPoints = (decimal)txtOverRevvingPoints.Value;
            newDriverGrading.UneconomicalPoints = (decimal)txtUneconomicalPoints.Value;
            EF.DataContext.Current.AddToCANDriverGradings(newDriverGrading);

            EF.DataContext.Current.SaveChanges();
        }
    }
}