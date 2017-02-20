using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Schedule
{

    // Replacement for telerik schedulers appointment comparer which doesn't fall foul of .net 4.5s requirement
    // that comparison of two items must be commuatative i.e. if comparing a and b gives -1, comparing b and a must give 1


    internal class ProteoAppointmentComparer : IComparer<Appointment>
    {
        public ProteoAppointmentComparer()
        {
        }

        public int Compare(Appointment first, Appointment second)
        {
            if (first == null || second == null)
            {
                throw new InvalidOperationException("Can't compare null object(s).");
            }
            if (first.Start < second.Start)
            {
                return -1;
            }
            if (first.Start > second.Start)
            {
                return 1;
            }
            if (first.End > second.End)
            {
                return -1;
            }
            // This is the missing case in teleriks implementation
            if (first.End < second.End)
            {
                return 1;
            }
            return 0;
        }
    }

}


