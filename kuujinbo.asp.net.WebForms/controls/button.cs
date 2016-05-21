/* ########################################################################
 * add functionality to m$ Button control
 * ########################################################################
*/ 
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly:TagPrefix("kuujinbo.asp.net.WebForms.controls", "kuujinbo")]
namespace kuujinbo.asp.net.WebForms.controls {
  [
    AspNetHostingPermissionAttribute(
      SecurityAction.InheritanceDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    )
    ,AspNetHostingPermissionAttribute(
      SecurityAction.LinkDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    )
    ,ToolboxData("<{0}:button ID='ButtonID' text='Button' runat='server' />")
  ]
  public class button : Button {
// ===========================================================================
/* 
 * path to JavaScript file; 
 * custom server controls use jQuery __NOT__ m$ client-side validation 
 */
    private static string _validationJsPath;
/* path to CSS and JavaScript files */
    private static string _fancyboxJsPath, _fancyboxCssPath;
    static button() {
      _validationJsPath = WebConfigurationManager
        .AppSettings[ControlFactory.KUUJINBO_JS];
      _fancyboxJsPath = WebConfigurationManager
        .AppSettings[ControlFactory.JQUERY_FANCYBOX];
      _fancyboxCssPath = WebConfigurationManager
        .AppSettings[ControlFactory.JQUERY_FANCYBOX_CSS];
    } 
// ---------------------------------------------------------------------------
// facebox instead of alert() => client-side validation errors
    public const string ERROR_FORMAT = @"
<link href='{0}' rel='stylesheet' type='text/css' media='all' />
<script src='{1}' type='text/javascript'></script>
";
// ---------------------------------------------------------------------------
// html/other page with validation error message/instructions
    private string _ValidationErrorMessage = 
      "<h1>Please verify all highlighted item(s) are filled in.</h1>";
    [
      Category("kuujinbo :: validation"),
      Description(
        "CLIENT-SIDE page validation error message"
      ),
      Editor(
        "kuujinbo.asp.net.WebForms.controls.design.MultilineStringEditor", 
        typeof(UITypeEditor)
      )
    ]    
    public string ValidationErrorMessage {
      get { return _ValidationErrorMessage; }
      set { _ValidationErrorMessage = value; }
    }
// ---------------------------------------------------------------------------
    protected override void OnInit(EventArgs e) {
      base.OnInit(e);
      if (CausesValidation) {
        Type csType = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
/*
 * hack to include server javascript validation & CSS files **ONCE** if there
 * are multiple button controls on the page
 */    
// our custom jQuery client-side validation
        if (!string.IsNullOrEmpty(_validationJsPath)) {
          if (!cs.IsClientScriptBlockRegistered(csType, _validationJsPath)) {
            Literal l = new Literal();
            l.Text = String.Format(StringFormat.TAG_SCRIPT, _validationJsPath);
            Page.Header.Controls.Add(l);         
            cs.RegisterClientScriptBlock(csType, _validationJsPath, "");
          }
        }
// fancybox modal window validation
        if (
            !string.IsNullOrEmpty(_fancyboxJsPath)
            && !string.IsNullOrEmpty(_fancyboxCssPath)
            ) 
        {
          if (!cs.IsClientScriptBlockRegistered(csType, _fancyboxJsPath)) {
            cs.RegisterClientScriptBlock(csType, _fancyboxJsPath,
              string.Format(ERROR_FORMAT, _fancyboxCssPath, _fancyboxJsPath)
            );
          }
        }
      }      
    }
// ---------------------------------------------------------------------------
    protected override void OnPreRender(EventArgs e) {
/* client-side validation => ValidationGroup */
      if (ValidationGroup != String.Empty) {
        Attributes.Add(ControlFactory.VALIDATION_GROUP_ATTR, ValidationGroup);
      }     
      if (Text == String.Empty) Text = "Submit";
    }
// ---------------------------------------------------------------------------
    protected override void Render(HtmlTextWriter w) {
      //w.Write("<span class='button'>");
      Attributes.Add("class", "btn btn-primary");
      base.Render(w);
      //w.Write("</span>");
      w.Write("<div id='__{0}__' style='display:none;'>{1}</div>",
        ClientID, _ValidationErrorMessage
      );
    }
  }
}