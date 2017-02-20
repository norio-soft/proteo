using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchestrator.WebUI.ProfilesService
{
    public partial class TrackingProfile
    {
        public string TimeFrequencyString
        {
            get
            {
                return this.TimeFrequency.ToString();
            }
        }

        public string DistanceFrequencyString
        {
            get
            {
                return this.DistanceFrequency.ToString();
            }
        }

        public string IdlingFrequencyString
        {
            get
            {
                return this.IdlingFrequency.ToString();
            }
        }

        public string SleepingFrequencyString
        {
            get
            {
                return this.SleepingFrequency.ToString();
            }
        }

        public string SendFrequencyString
        {
            get
            {
                return this.SendFrequency.ToString();
            }
        }

        public string WeightStabFrequencyString
        {
            get { return this.WeightStabFrequency.ToString(); }
        }

        public string NumberOfVehiclesString
        {
            get
            {
                return this.NumberOfVehicles.ToString();
            }
        }

        public string DistanceFrequencyKmString
        {
            get
            {
                return (this.DistanceFrequency / new Decimal(1000.0)).ToString() + " km";
            }
        }

        public string TimedFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.TimeFrequency), 0);
                return t.ToString();
            }
        }

        public string SleepingFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.SleepingFrequency), 0);
                return t.ToString();
            }
        }

        public string SendFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.SendFrequency), 0);
                return t.ToString();
            }
        }
        public string IdlingFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.IdlingFrequency), 0);
                return t.ToString();
            }
        }

        public string IdleTimeFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.IdleTimeFrequency), 0);
                return t.ToString();
            }
        }

        public string FilterFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.FilterFrequency), 0);
                return t.ToString();
            }
        }

        public string WeightFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.WeightFrequency), 0);
                return t.ToString();
            }
        }

        public string WeightStabFrequencyTitle
        {
            get
            {
                TimeSpan t = new TimeSpan(0, 0, 0, Convert.ToInt32(this.WeightStabFrequency), 0);
                return t.ToString();
            }
        }

        public string SendSizeTitle
        {
            get
            {
                return this.SendSize.ToString();
            }
        }

        public string BatchSizeTitle
        {
            get
            {
                return this.BatchSize.ToString();
            }
        }

        public string QueueSizeTitle
        {
            get
            {
                return this.QueueSize.ToString();
            }
        }

        public string DirectionAngleTitle
        {
            get
            {
                return this.DirectionAngle.ToString();
            }
        }
    }
}