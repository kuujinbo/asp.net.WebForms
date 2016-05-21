/* ###########################################################################
 * common members used by project custom server controls
 * ###########################################################################
*/
namespace kuujinbo.asp.net.WebForms.controls {
  public interface Icontrol {
// ===========================================================================
    string val  { get; set; } 
    string label  { get; set; }
    bool required { get; set; }
/* ---------------------------------------------------------------------------
 * for controls that inherit from System.Web.UI.WebControls.WebControl
 * ---------------------------------------------------------------------------
*/
// get/set 'ID' Property
    string controlID { get; set; }
// get/set 'Enabled' Property
    bool enabled { get; set; }
// ===========================================================================    
  }
}