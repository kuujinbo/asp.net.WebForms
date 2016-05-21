/* ###########################################################################
 * datebox control, use jQuery UI datepicker for nice UI;
 * http://jqueryui.com/demos/datepicker/
 * 
 * obviously you need to include jQuery base in <head>
 * ###########################################################################
*/
using System;
using System.ComponentModel;
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
    ,ToolboxData("<{0}:datebox ID='DateBoxID' runat='server' />")
  ]
  public class datebox : textbox {
// ===========================================================================    
/* path to CSS and JavaScript files */
    private static string _cssPath, _jsPath;
    static datebox() {
      _cssPath = WebConfigurationManager.AppSettings[ControlFactory.DATEBOX_CSS_PATH];
      _jsPath = WebConfigurationManager.AppSettings[ControlFactory.DATEBOX_JS_PATH];
    } 
    public datebox() {
      ValidationDataType = ValidationDataType.Date;
      this.MaxLength = 10;
    } 
/*
 * ###########################################################################
 * required by Icontrol
 * ###########################################################################
*/
    private DateTime _dt;
    [Browsable(false)]
    public override string val {
      get { return DateTime.TryParse(this.Text, out _dt)
        ? _dt.ToString("yyyy/MM/dd") : "";
      }
      set { this.Text = DateTime.TryParse(value, out _dt)
        ? _dt.ToString(@"yyyy/MM/dd") : "";
      }
    }  
    
/*
 * ###########################################################################
 * client-side processing
 * ###########################################################################
*/
    [
    Category("kuujinbo :: appearance"),
    Description("minimum selectable date; see jQuery UI documentation")
    ]    
    public string MinDate { get; set; }
    [
    Category("kuujinbo :: appearance"),
    Description("maximum selectable date; see jQuery UI documentation")
    ]    
    public string MaxDate { get; set; }
    [
    Category("kuujinbo :: appearance"),
    Description("selectable year range; see jQuery UI documentation")
    ]    
    public string YearRange { get; set; }
/*
 * ###########################################################################
 * override parent class methods
 * ###########################################################################
*/
// **MUST** use this event to properly order .css/.js include files
    protected override void OnPreRender(EventArgs e) {
      base.OnPreRender(e);
/*
 * hack to include the CSS file **ONCE** if there are multiple datebox 
 * controls on the page
 */
      Type cstype = this.GetType();
      ClientScriptManager cs = Page.ClientScript;
// verify web.config <appSettings> keys exist     
      if (!string.IsNullOrEmpty(_cssPath) && !string.IsNullOrEmpty(_jsPath))
      {
        if (!cs.IsClientScriptBlockRegistered(cstype, _cssPath)) {
          Literal l = new Literal() {
            Text = String.Format(
@"<link href='{0}' rel='stylesheet' type='text/css' />
<script src='{1}' type='text/javascript'></script>",
              _cssPath, _jsPath
            )
          };
          Page.Header.Controls.Add(l);           
        }
      }
      else {
        throw new Exception(string.Format(
          "datebox control configuration empty; set web.config <appSettings> '{0}' '{1}' keys",
          ControlFactory.DATEBOX_CSS_PATH, ControlFactory.DATEBOX_JS_PATH
        ));
      }
    }
/* ------------------------------------------------------------------------
 * second to last stage of Page processing; Page object calls method on
 * control to write out control's markup to sent to browser.
 * ------------------------------------------------------------------------
*/
    protected override void Render(HtmlTextWriter w) {
      // add 'class' attribute values for client-side validation
      classAttributes += " " + ControlFactory.DATEBOX_CLASS;
      base.Render(w); 

      string minDate = !string.IsNullOrEmpty(MinDate) ? MinDate : "-5y";
      string maxDate = !string.IsNullOrEmpty(MaxDate) ? MaxDate : "+5y";
      string yearRange = !string.IsNullOrEmpty(MaxDate) ? MaxDate : "c-10:c+20";
      w.Write(string.Format(
@"<script type='text/javascript'>
$(function() {{
  $('#{0}').datepicker({{
    dateFormat:'yy/mm/dd',
    minDate:'{1}',maxDate:'{2}',yearRange:'{3}',
    changeMonth:true,changeYear:true
  }});
}});
</script>",
        this.ClientID, minDate, maxDate, yearRange
      ));
    }
// ===========================================================================
  }
}