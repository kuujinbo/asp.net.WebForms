using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;

using Newtonsoft.Json;
using kuujinbo.asp.net.WebForms;

namespace ControlExamples {
    public partial class DatatablesServer : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PrependCssToHead("/Content/DataTables/css/jquery.dataTables.min.css");
            this.AppendJavaScript("/Scripts/DataTables/jquery.dataTables.min.js");
            
            // this.PrependCssToHead("/Scripts/DataTables/media/css/jquery.dataTables.min.css");
            //this.AppendJavaScript("/Scripts/DataTables/media/js/jquery.dataTables.min.js");
        }

        public static List<List<string>> AofA(int rows, int columns)
        {
            List<List<string>> result = new List<List<string>>();
            for (int r = 0; r < rows; ++r)
            {
                List<string> row = new List<string>();
                for (int f = 0; f < columns; ++f)
                {
                    row.Add(r.ToString());
                }
                result.Add(row);
            }
            return result;
        }

    }
}