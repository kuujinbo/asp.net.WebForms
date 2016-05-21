using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using kuujinbo.asp.net.WebForms;

namespace ControlExamples {
  public partial class DataTables : System.Web.UI.Page {
    protected override void OnInit(EventArgs e) {
      base.OnInit(e);
      this.AppendCssToHead("/Scripts/DataTables/media/css/jquery.dataTables.min.css");
      this.AppendJavaScript("/Scripts/DataTables/jquery.dataTables.min.js");
      // this.AppendJavaScript("/Scripts/DataTables/media/js/jquery.dataTables.min.js");
    }
  }
}