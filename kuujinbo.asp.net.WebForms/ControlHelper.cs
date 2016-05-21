// ###########################################################################
// custom control helpers
// ###########################################################################
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using kuujinbo.asp.net.WebForms.controls;

namespace kuujinbo.asp.net.WebForms {
  public class ControlHelper {
// ===========================================================================
/*
 * ###########################################################################
 * static methods
 * ###########################################################################
 */
// ***************************************************************************
// maintain consistency over our web application...
    public static ListItem EmptyListItem {
     get { return new ListItem("...", "");; }
    }      
// ***************************************************************************
// dump a simple SQL snippet for save operation using the existing custom
// controls on the page for __development__
    public static string DbSaveSnippet(ControlCollection cc) {
      ControlHelper ch = new ControlHelper();
      List<Icontrol> customControls = ch.GetCustomControls(cc);
      string fields = "(";
      string values = "VALUES(";
      string update = "";
// get last index, and stop at second to the last index in for loop 
      int iterateTo = customControls.Count - 1;
      for (int i = 0; i < iterateTo; ++i) {
        string name = customControls[i].controlID;
        fields += string.Format("{0},", name);
        values += string.Format("@{0},", name);
        update += string.Format("{0}=@{0},", name);
      }
      fields += string.Format("{0})", customControls[iterateTo].controlID);
      values += string.Format("@{0})", customControls[iterateTo].controlID);
      update += string.Format("{0}=@{0}", customControls[iterateTo].controlID);
      
      return fields + Environment.NewLine + values
        + Environment.NewLine + Environment.NewLine + update
      ;
    }

/*
 * ###########################################################################
 * instance methods
 * ###########################################################################
 */
// recursively get all custom controls from control collection
    private List<Icontrol> _customControls;
    public List<Icontrol> GetCustomControls(ControlCollection cc) {
      if (_customControls == null) _customControls = new List<Icontrol>();
      foreach (Control c in cc) {
        if (c is Icontrol) {
          _customControls.Add( (Icontrol)c );
        }
        if (c.Controls != null) {
          GetCustomControls(c.Controls);
        }
      }
      return _customControls;
    }
// ***************************************************************************
// recursively get all custom controls from control collection
    private Dictionary<string, Icontrol> _controlHash;
    public Dictionary<string, Icontrol> MapCustomControls(ControlCollection cc)
    {
      if (_controlHash == null) {
        _controlHash = new Dictionary<string, Icontrol>();
      }
      foreach (Control c in cc) {
        Icontrol i = c as Icontrol;
        if (i != null) {
          string id = i.controlID;
          if (!_controlHash.ContainsKey(id)) {
            _controlHash.Add(id, i);
          }
          else {
            throw new InvalidOperationException(string.Format(
              "died in {0}: multiple custom server control IDs",
              MethodBase.GetCurrentMethod().Name
            ));
          }
        }
        if (c.Controls != null) {
          MapCustomControls(c.Controls);
        }
      }
      return _controlHash;
    }    
// ===========================================================================  
  }
}