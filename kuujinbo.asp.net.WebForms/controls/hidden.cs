/* ###########################################################################
 * add functionality to m$ HiddenField control
 * ###########################################################################
 */

using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
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
    ,ToolboxData("<{0}:hidden ID='HiddenFieldID' runat='server' />")
  ]
  public class hidden : HiddenField, Icontrol {
// ===========================================================================
/*
 * ###########################################################################
 * required by Icontrol
 * ###########################################################################
*/
/*
 * Interface implementation that allows us to use reflection to get 
 * control's **STRINGIFIED** value
*/
    [Browsable(false)]
    public virtual string val {
      get { return this.Value.Trim(); }
      set { this.Value = value.Trim(); }
    }
/*
 * to meet Interface implementation contract from here to __END__
*/
    [Browsable(false)]
    public string label { get; set; }
    [Browsable(false)]
    public bool required { get; set; }
    [Browsable(false)]
    public bool enabled { get; set; }
// ---------------------------------------------------------------------------
// get/set 'ID' Property from 
    [Browsable(false)]
    public string controlID {
      get { return this.ID; }
      set { this.ID = value; }
    }
// ===========================================================================
  }
}