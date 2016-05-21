/* ###########################################################################
 * custom server controls => common members
 * ###########################################################################
 */
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace kuujinbo.asp.net.WebForms.controls {
  public sealed class ControlFactory {
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 * web.config <appSettings> key-value pairs for kuujinbo.asp.net.WebForms.controls;
 * client-side/UI enhancements. CSS/JavaScript FILES ARE NOT INLUDED AS
 * RESOURCES IN THIS PROJECT to allow a little more flexibility if you
 * want to upgrade CSS/JavaScript without rebuilding this project.
 * @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
*/

// virtual paths from web.config - JavaScript framework libraries
    public const string TEXTAREA_CHAR_COUNTER_JS_PATH = "TextareaJavaScriptPath";
    public const string DATEBOX_JS_PATH = "DateBoxJavaScriptPath";
    public const string DATEBOX_CSS_PATH = "DateBoxCssPath";
/*
 * virtual paths from web.config - local development
 * TODO: clean up file
*/
    public const string KUUJINBO_JS = "KuujinboControlsJavaScriptPath";

// kuujinbo.asp.net.WebForms.controls.button - custom server controls use jQuery
// __NOT__ m$ client-side validation

// kuujinbo.asp.net.WebForms.controls.textbox
    public const string REGEX = "regex";
    public const string REGEX_ERROR_MSG = "regexErrorMessage";
    public const string REGEX_ID_SUFFIX = "-regex-validator";
    
    public const string DATEBOX_CLASS = "date";

// kuujinbo.asp.net.WebForms.controls.textbox; HTML 5 compliant class attribute => textarea
    public const string MAXLENGTH_ATTR = "maxlength";
    
// kuujinbo.asp.net.WebForms.controls.checkboxlist
    public const string CHECKBOXLIST_CHECKALL_CLASS = "checkboxlist-checkall";
   //  public const string LIST_MULTIPLE_REQUIRED_CLASS = "required-list";
    
// kuujinbo.asp.net.WebForms.controls.radiobuttonlist
    public const string RADIOBUTTONLIST_CLASS = "radio";
    public const string CHECKBOX_CLASS = "checkbox";

// kuujinbo.asp.net.WebForms.controls.button 
    public const string JQUERY_FANCYBOX = "jQueryFancyBox";
    public const string JQUERY_FANCYBOX_CSS = "jQueryFancyBoxCSS";

// custom control flag using HTML class attribute
    public const string CONTROL_FLAG = "kuujinbo";
// custom HTML attribute for client-side JavaScript validation    
    public const string VALIDATION_GROUP_ATTR = "validation-group";
// HTML class attribute => mark control required using 
    public const string REQUIRED_ATTR = "required";
    public const string REQUIRED_FLAG = "* ";
// HTML class attribute => invalid control data
    public const string ERROR_CLASS = "input-validation-error";
// HTML5  attribute
    // public const string PLACEHOLDER_ATTR = "placeholder";
// ---------------------------------------------------------------------------
// bootstrap classes - http://getbootstrap.com/css/
    public const string BOOTSTRAP_FORM_CLASS = "form-control";

// ---------------------------------------------------------------------------
// 'block' format - <label> contained in <div>
    public static readonly string DIV_LABEL_FORMAT = string.Format(
      "<div style='margin-top:4px'>{0}</div>", StringFormat.TAG_LABEL
    );
/*
      "<div class='{0}'>{1}</div>", 
      CONTROL_FLAG, StringFormat.TAG_LABEL
*/
      
// 'inline' format - <label> contained in <span>
    public static readonly string SPAN_LABEL_FORMAT = 
      "<span>" + StringFormat.TAG_LABEL + "</span> ";
// ---------------------------------------------------------------------------
    public static void AddServerRequiredStyle(HtmlTextWriter w) {
      w.AddStyleAttribute("border", "2px solid red");
    }
// ---------------------------------------------------------------------------
    public static RequiredFieldValidator GetRequiredValidator(
      string ID, string validationGroup
      ) 
    {
      RequiredFieldValidator rfv = new RequiredFieldValidator();
      rfv.ControlToValidate = ID;
      rfv.ID = ID + "_required-validator";
      rfv.Display = ValidatorDisplay.Dynamic;
      rfv.ErrorMessage = " Required";
      rfv.EnableClientScript = false;    
      if (validationGroup != String.Empty)
        rfv.ValidationGroup = validationGroup;
      return rfv;
    }    
// ===========================================================================    
  }
}