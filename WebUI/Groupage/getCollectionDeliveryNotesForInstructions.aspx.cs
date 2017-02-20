using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI
{
    public partial class getCollectionDeliveryNotesForInstructions : Orchestrator.Base.BasePage
    {
        private static string note = @"<div><div style=""font-weight:bold;"">{0} : {1}</div><div>{2}</div></div><br/>";

        protected void Page_Load(object sender, EventArgs e)
        {
            int startInstructionID = 0, endInstructionID = 0;
            bool sIsCollection = false, eIsCollection = false;
            int.TryParse(Request.QueryString["sID"].ToString(), out startInstructionID);
            int.TryParse(Request.QueryString["eID"].ToString(), out endInstructionID);

            StringBuilder siSB = new StringBuilder();
            StringBuilder eiSB = new StringBuilder();
            StringBuilder displayString = new StringBuilder();

            Entities.Instruction foundSInstruction = null, foundEInstruction = null;

            if (startInstructionID > 0 && endInstructionID > 0)
            {
                Facade.IInstruction facIns = new Orchestrator.Facade.Instruction();
                List<Entities.CollectDrop> sCollectionDrops = new List<Entities.CollectDrop>();
                List<Entities.CollectDrop> eCollectionDrops = new List<Entities.CollectDrop>();

                string cacheName = "_InstructionCollectionDeliveryNotes" + startInstructionID.ToString();

                if (Cache[cacheName] == null)
                {
                    foundSInstruction = facIns.GetInstruction(startInstructionID);
                    if (foundSInstruction != null)
                        Cache.Add(cacheName, foundSInstruction, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                    foundSInstruction = (Entities.Instruction)Cache[cacheName];

                cacheName = "_InstructionCollectionDeliveryNotes" + endInstructionID.ToString();

                if (Cache[cacheName] == null)
                {
                    foundEInstruction = facIns.GetInstruction(endInstructionID);
                    if (foundEInstruction != null)
                        Cache.Add(cacheName, foundEInstruction, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                    foundEInstruction = (Entities.Instruction)Cache[cacheName];

                sIsCollection = foundSInstruction.InstructionTypeId == (int)eInstructionType.Load ? true : false;
                eIsCollection = foundEInstruction.InstructionTypeId == (int)eInstructionType.Load ? true : false;

                if (foundSInstruction.CollectDrops.Exists(cd => cd.OrderAction == eOrderAction.Default && cd.Order.CollectionPointID == foundSInstruction.PointID))
                    sCollectionDrops = foundSInstruction.CollectDrops.FindAll(cd => cd.OrderAction == eOrderAction.Default && cd.Order.CollectionPointID == foundSInstruction.PointID).ToList();

                if (foundEInstruction.CollectDrops.Exists(cd => cd.OrderAction == eOrderAction.Default && cd.Order.DeliveryPointID == foundEInstruction.PointID))
                    eCollectionDrops = foundEInstruction.CollectDrops.FindAll(cd => cd.OrderAction == eOrderAction.Default && cd.Order.DeliveryPointID == foundEInstruction.PointID).ToList();
                
                foreach(Entities.CollectDrop cd in sCollectionDrops)
                    if((sIsCollection && !string.IsNullOrEmpty(cd.Order.CollectionNotes)) || (!sIsCollection && !string.IsNullOrEmpty(cd.Order.DeliveryNotes)))
                        siSB.Append(string.Format(note,
                                                  sIsCollection ? "Collection" : "Delivery",
                                                  cd.Order.OrderID,
                                                  sIsCollection ? cd.Order.CollectionNotes : cd.Order.DeliveryNotes));

                foreach (Entities.CollectDrop cd in eCollectionDrops)
                    if ((eIsCollection && !string.IsNullOrEmpty(cd.Order.CollectionNotes)) || (!eIsCollection && !string.IsNullOrEmpty(cd.Order.DeliveryNotes)))
                        eiSB.Append(string.Format(note,
                                                  eIsCollection ? "Collection" : "Delivery",
                                                  cd.Order.OrderID,
                                                  eIsCollection ? cd.Order.CollectionNotes : cd.Order.DeliveryNotes));

                if (siSB.Length > 0)
                    displayString.Append(siSB.ToString());

                if (eiSB.Length > 0)
                    displayString.Append(eiSB.ToString());

                Response.Write(displayString.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }
}
