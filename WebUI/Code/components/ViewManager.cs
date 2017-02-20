using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.IO;

namespace Orchestrator.WebUI
{
    public class ViewManager
    {
        public static string RenderView(string path)
        {
            return RenderView(path, null);
        }
        public static string RenderView(string path, object data)
        {
            StringWriter output = new StringWriter();
            try
            {
                Page pageHolder = new Page();
                UserControl viewControl = (UserControl)pageHolder.LoadControl(path);

                if (data != null)
                {
                    Type viewControlType = viewControl.GetType();
                    FieldInfo field = viewControlType.GetField("Data");

                    if (field != null)
                        field.SetValue(viewControl, data);
                    else
                        throw new Exception("View file: " + path + " does not have a public Data proeprty");

                }

                pageHolder.Controls.Add(viewControl);

                try
                {
                    HttpContext.Current.Server.Execute(pageHolder, output, false);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return output.ToString();
        }
    }
}
