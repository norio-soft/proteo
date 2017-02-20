
    using System;
    using System.Web.Caching;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Xml;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;

namespace Orchestrator.WebUI.WebParts
{
    /// <summary>
    /// Catalog for reading WebParts from an Xml Document
    /// </summary>
    public class XmlCatalogPart : CatalogPart
    {
        XmlDocument document;
        /// <summary>
        /// Overrides the Title to display Xml Catalog Part by default
        /// </summary>
        public override string Title
        {
            get
            {
                string title = base.Title;
                return string.IsNullOrEmpty(title) ? "Xml Catalog Part" : title;
            }
            set
            {
                base.Title = value;
            }
        }

        /// <summary>
        /// Specifies the Path for the Xml File that contains the declaration of the WebParts, 
        ///     more specifically the WebPartDescriptions
        /// </summary>
        [
        UrlProperty(),
        DefaultValue(""),
        Editor(typeof(System.Web.UI.Design.XmlUrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
        ]
        public string DataFile
        {
            get
            {
                object o = ViewState["DataFile"];
                return o == null ? "" : (string)o;
            }
            set
            {
                ViewState["DataFile"] = value;
            }
        }

        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
        public XmlCatalogPart()
        {

        }

        /// <summary>
        /// Returns the WebPartDescriptions
        /// </summary>
        public override WebPartDescriptionCollection GetAvailableWebPartDescriptions()
        {
            if (this.DesignMode)
            {
                return new WebPartDescriptionCollection(new object[] {
                    new WebPartDescription("1", "Xml WebPart 1","", null),
                    new WebPartDescription("2", "Xml WebPart 2","", null),
                        new WebPartDescription("3", "Xml WebPart 3","", null)});
            }

            XmlDocument document = GetDocument();
            List<WebPartDescription> list = new List<WebPartDescription>();
            foreach (XmlElement element in document.SelectNodes("/parts/part"))
            {
                list.Add(
                    new WebPartDescription(
                        element.GetAttribute("id"),
                        element.GetAttribute("title"),
                        element.GetAttribute("description"),
                        element.GetAttribute("imageUrl")));
            }
            return new WebPartDescriptionCollection(list);
        }

        /// <summary>
        /// Returns a new instance of the WebPart specified by the description
        /// </summary>
        public override WebPart GetWebPart(WebPartDescription description)
        {
            string typeName = this.GetTypeNameFromXml(description.ID);
            Type type = Type.GetType(typeName);
            return Activator.CreateInstance(type, null) as WebPart;
        }


        /// <summary>
        /// private function to load the document and cache it
        /// </summary>
        private XmlDocument GetDocument()
        {
            string file = Context.Server.MapPath(this.DataFile);
            string key = "__xmlCatalog:" + file.ToLower();
            XmlDocument document = Context.Cache[key] as XmlDocument;
            if (document == null)
            {
                using (CacheDependency dependency = new CacheDependency(file))
                {
                    document = new XmlDocument();
                    document.Load(file);
                    Context.Cache.Add(key, document, dependency,
                        Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
            }
            return document;
        }

        /// <summary>
        /// Returns the type
        /// </summary>
        private string GetTypeNameFromXml(string webPartID)
        {
            XmlDocument document = GetDocument();
            XmlElement element = (XmlElement)document.SelectSingleNode("/parts/part[@id='" + webPartID + "']");
            return element.GetAttribute("type");
        }
    }
}