using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace kuujinbo.asp.net.WebForms {
  public class BasePageCsrf : Page {
// protect against CSRF
// TODO: https://www.owasp.org/index.php/.NET_Security_Cheat_Sheet
    protected override void OnInit(EventArgs e) {
// there __MUST__ be at __LEAST__ one session variable set, or this 
// does __NOT__ work!!
      Session["__CSRF-throwaway__"] = 0;
      ViewStateUserKey = Session.SessionID;
      base.OnInit(e);
    }
/*
    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);
      Response.Write(Session.SessionID);
    }
*/
  }
}