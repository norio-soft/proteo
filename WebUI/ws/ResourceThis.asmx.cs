using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using AjaxControlToolkit;
using System.Collections.Specialized;

namespace Orchestrator.WebUI.ws
{
    /// <summary>
    /// Summary description for ResourceThis
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ResourceThis : System.Web.Services.WebService
    {
        [WebMethod]
        public CascadingDropDownNameValue[] GetControlAreas(string knownCategoryValues, string category)
        {
            Facade.IControlArea facTrafficArea = new Facade.Traffic();
            DataSet dsControlAreas = facTrafficArea.GetAll();
            int selectedControlAreaId = 0;
            int.TryParse(category, out selectedControlAreaId);
            

                var ret = (from dr in dsControlAreas.Tables[0].AsEnumerable()
                           select new CascadingDropDownNameValue
                           {
                               name = dr.Field<string>("Description"),
                               value = (dr.Field<int>("ControlAreaId")).ToString()
                           }).ToArray();

                
            return ret;
        }

        [WebMethod]
        public CascadingDropDownNameValue[] GetTrafficAreas(string knownCategoryValues, string category, string contextKey)
        {
            int controlAreaID;
            //this stringdictionary contains has table with key value
            //pair of cooountry and countryID
            StringDictionary countryValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
            controlAreaID = Convert.ToInt32(countryValues["ControlArea"]);

            Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
            DataSet dsTrafficAreas = facTrafficArea.GetForControlAreaId(controlAreaID);

            var ret = (from dr in dsTrafficAreas.Tables[0].AsEnumerable()
                       select new CascadingDropDownNameValue
                       {
                           name = dr.Field<string>("Description"),
                           value = (dr.Field<int>("TrafficAreaId")).ToString()
                       }).ToArray();

            foreach (var v in ret)
            {
                if (v.value == contextKey)
                {
                    v.isDefaultValue = true;
                    break;
                }
            }
            return ret;
        }
    }
}
