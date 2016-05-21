/* ###########################################################################
 * library format strings
 * ###########################################################################
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kuujinbo.asp.net.WebForms {
  public class StringFormat {
// ============================================================================
// <script> format
    public const string TAG_SCRIPT = @"<script src='{0}' type='text/javascript'></script>
";  
// <link> format
    public const string TAG_LINK = @"<link rel='stylesheet' href='{0}' type='text/css' />
"; 
// <label> format
    public const string TAG_LABEL = "<label for='{0}'>{1}</label>";
// ============================================================================  
  }
}