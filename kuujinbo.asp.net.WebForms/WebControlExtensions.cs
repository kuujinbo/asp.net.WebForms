/* ###########################################################################
 * web control extension methods
 * ###########################################################################
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace kuujinbo.asp.net.WebForms {
  public static class WebControlExtensions {
// ===========================================================================

// ***************************************************************************
// BIG ommision by m$ - you have to jump through rings of fire to add an 
// attribute to a listitem control.
// ***************************************************************************
// add an attribute. we take the Perl approach and __overwrite__ an existing
// key-pair value
    public static void ListItemAddAttribute(
      this ListItem liWTF, string key, string value
    ) {
      liWTF.Attributes[key] = value;
    }
// ---------------------------------------------------------------------------
// add class attribute from listitem
    public static void ListItemAddClass(this ListItem liWTF, string name) {
      string className = liWTF.Attributes["class"];
      liWTF.Attributes["class"] = string.IsNullOrEmpty(className)
        ? name : " " + name
      ;      
    } 
// ---------------------------------------------------------------------------
// remove class attribute from listitem
    public static void ListItemRemoveClass(this ListItem liWTF, string name) {
      string className = liWTF.Attributes["class"];
      if ( !string.IsNullOrEmpty(className) ) {
        liWTF.Attributes["class"] = className
          .Replace(name, "")
          .Replace("  ", " ")
        ;
      }
    }
// ===========================================================================  
  }
}