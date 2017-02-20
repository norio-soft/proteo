using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Orchestrator.SystemFramework;

namespace Orchestrator.WebUI
{

    public static class CsvExport
    {

        #region Helper Classes

        public struct ColumnMapping
        {
            public string SourceColumnName { get; set; }
            public string TargetColumnName { get; set; }
            public Func<DataRow, string, string> Formatter { get; set; }

            public ColumnMapping(string sourceColumnName, string targetColumnName, Func<DataRow, string, string> formatter)
                : this()
            {
                this.SourceColumnName = sourceColumnName;
                this.TargetColumnName = targetColumnName;
                this.Formatter = formatter;
            }

            public Type TargetColumnType(DataTable sourceTable)
            {
                return this.Formatter == null ? sourceTable.Columns[this.SourceColumnName].DataType : typeof(string);
            }
        }

        public class ColumnMappings : List<ColumnMapping>
        {
            public virtual void Add(string sourceColumnName, string targetColumnName, Func<DataRow, string, string> formatter)
            {
                base.Add(new ColumnMapping(sourceColumnName, targetColumnName, formatter));
            }

            public virtual void Add(string sourceColumnName, string targetColumnName)
            {
                this.Add(sourceColumnName, targetColumnName, null);
            }
        }

        public struct PropertyMapping<T>
        {
            public string TargetColumnName { get; set; }
            public Func<T, string> RetriverFormatter { get; set; }

            public PropertyMapping(string targetColumnName, Func<T, string> retrieverFormatter)
                : this()
            {
                this.TargetColumnName = targetColumnName;
                this.RetriverFormatter = retrieverFormatter;
            }

            internal object GetValue(T item)
            {
                return this.RetriverFormatter(item);
            }
        }

        public class PropertyMappings<T> : List<PropertyMapping<T>>
        {
            public virtual void Add(string targetColumnName, Func<T, string> retrieverFormatter)
            {
                base.Add(new PropertyMapping<T>(targetColumnName, retrieverFormatter));
            }
        }

        #endregion Helper Classes

        public static void Export(DataTable dtSource, string fileName, IEnumerable<ColumnMapping> columnMappings)
        {
            var dt = new DataTable();

            foreach (var cm in columnMappings)
            {
                dt.Columns.Add(new DataColumn(cm.TargetColumnName, cm.TargetColumnType(dtSource)));
            }

            foreach (DataRow drSource in dtSource.Rows)
            {
                var dr = dt.NewRow();

                foreach (var cm in columnMappings)
                {
                    if (cm.Formatter == null)
                        dr.CopyField(cm.TargetColumnName, drSource, cm.SourceColumnName);
                    else
                        dr.SetField(cm.TargetColumnName, cm.Formatter(drSource, cm.SourceColumnName));
                }

                dt.Rows.Add(dr);
            }

            Export(dt, fileName);
        }

        public static void Export<T>(IEnumerable<T> source, string fileName, IEnumerable<PropertyMapping<T>> propertyMappings)
        {
            var dt = new DataTable();

            foreach (var pm in propertyMappings)
            {
                dt.Columns.Add(new DataColumn(pm.TargetColumnName, typeof(string)));
            }

            foreach (T item in source)
            {
                var dr = dt.NewRow();

                foreach (var pm in propertyMappings)
                {
                    dr.SetField(pm.TargetColumnName, pm.GetValue(item));
                }

                dt.Rows.Add(dr);
            }

            Export(dt, fileName);
        }

        private static void Export(DataTable dt, string fileName)
        {
            var context = HttpContext.Current;
            context.Session["__ExportDS"] = dt;
            context.Response.Redirect("/reports/csvexport.aspx?filename=" + fileName);
        }

    }

}