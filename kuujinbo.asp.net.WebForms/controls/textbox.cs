/* ###########################################################################
 * add functionality to m$ TextBox control
 * ###########################################################################
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly:TagPrefix("kuujinbo.asp.net.WebForms.controls", "kuujinbo")]
namespace kuujinbo.asp.net.WebForms.controls {
  [
    ValidationProperty("Text"),
    AspNetHostingPermissionAttribute(
      SecurityAction.InheritanceDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    )
    ,AspNetHostingPermissionAttribute(
      SecurityAction.LinkDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    )
    ,ToolboxData("<{0}:textbox ID='TextBoxID' runat='server' />")
  ]
  public class textbox : TextBox, Icontrol {
    /* path to CSS and JavaScript files */
    private static string _jsPath;
    static textbox() {
      _jsPath = WebConfigurationManager.AppSettings[ControlFactory.TEXTAREA_CHAR_COUNTER_JS_PATH];
    } 
/* ###########################################################################
 * required by Icontrol
 * ###########################################################################
*/
/*
 * Interface implementation that allows us to use reflection to get 
 * control's **STRINGIFIED** value
*/
    [Browsable(false)]
    public virtual string val {
      get { return this.Text.Trim(); }
      set {
        if ( ValidationDataType != ValidationDataType.Currency ) {
          this.Text = value.Trim();
        }
        else {
          Decimal dt;
          this.Text = Decimal.TryParse( value.Trim(), out dt )
           ? dt.ToString("F2")
           : ""
           ;
        }
      }
    }
// ---------------------------------------------------------------------------
// **OPTIONAL** => control's HTML <label>, block-level
    private string _label;
    [
      Category("kuujinbo :: appearance"),
      Description("control's <label> text description contained in <div>")
    ]
    public string label {
      get { return _label; }
      set { _label = value.Trim(); }
    } 
// **OPTIONAL** => control's HTML <label>, inline
    private string _inlineLabel;
    [
      Category("kuujinbo :: appearance"),
      Description("control's <label> text description contained in <div>")
    ]
    public string InlineLabel {
      get { return _inlineLabel; }
      set { _inlineLabel = value.Trim(); }
    }

    public string PlaceholderAttr { get; set; } 
// ---------------------------------------------------------------------------
// server-side/client flag to force user input
    [
      Category("kuujinbo :: validation"),
      Description("add/remove RequiredFieldValidator"),
      TypeConverter(typeof(BooleanConverter))
    ]
    public bool required {
      get { return ViewState[ControlFactory.REQUIRED_ATTR] != null
        ? (bool) ViewState[ControlFactory.REQUIRED_ATTR]
        : false
        ;
      }
      set { ViewState[ControlFactory.REQUIRED_ATTR] = value; }
    }
/* ---------------------------------------------------------------------------
 * control is-a WebControl
 * ---------------------------------------------------------------------------
*/    
// get/set 'ID' Property from 
    [Browsable(false)]
    public string controlID {
      get { return this.ID; }
      set { this.ID = value; }
    }
// ---------------------------------------------------------------------------
// allow access to parent 'Enabled' Property
    [Browsable(false)]
    public bool enabled {
      get { return this.Enabled; }
      set { this.Enabled = value; }
    }
/*
 * ###########################################################################
 * override parent class methods
 * ###########################################################################
 */
    private RequiredFieldValidator _rfv;

    protected override void OnInit(EventArgs e) {
      base.OnInit(e);
      Controls.Clear();
/* ignore validation if control **NOT** enabled */
      if (!this.Enabled) {
        return;
      }
/* add RequiredFieldValidator */
      if (required) {
        _rfv = ControlFactory.GetRequiredValidator(
          this.ID, this.ValidationGroup
        );
        Controls.Add(_rfv);      
      }
/* add RegularExpressionValidator */
      if (!String.IsNullOrEmpty(regex)) {
        addRegexValidator();
      }
/* 
 * client-side JavaScript character counter; **MUST** edit plugin file 
 * to call on all server control(s)
*/
      if (TextMode == TextBoxMode.MultiLine && MaxLength > 0) {
        Attributes.Add(ControlFactory.MAXLENGTH_ATTR, MaxLength.ToString());
        Type cstype = this.GetType();
        ClientScriptManager cs = Page.ClientScript;
// verify web.config <appSettings> keys exist     
        if (!string.IsNullOrEmpty(_jsPath)) {
          if (!cs.IsClientScriptBlockRegistered(cstype, _jsPath)) {
            Literal l = new Literal() {
              Text = String.Format(StringFormat.TAG_SCRIPT, _jsPath)
            };
            Page.Header.Controls.Add(l);
            cs.RegisterClientScriptBlock(cstype, _jsPath, "");
          }
        }
      } 
           
/* add CompareValidator */
      AddCompareValidator();
/* client-side validation => ValidationGroup */
      if (ValidationGroup != String.Empty) {
        Attributes.Add(ControlFactory.VALIDATION_GROUP_ATTR, ValidationGroup);
      }
    }
/* ------------------------------------------------------------------------
 * second to last stage of Page processing; Page object calls method on
 * control to write out control's markup to sent to browser.
 * ------------------------------------------------------------------------
*/
    protected override void Render(HtmlTextWriter w) {
// block label takes precendence
      if (_label != null) {
        if (required) {
          _label = ControlFactory.REQUIRED_FLAG + _label;
        }
        w.Write(ControlFactory.DIV_LABEL_FORMAT, ClientID, _label);
      }
      else if (_inlineLabel != null) {
        if (required) {
          _inlineLabel = ControlFactory.REQUIRED_FLAG + _inlineLabel;
        }
        w.Write(ControlFactory.SPAN_LABEL_FORMAT, ClientID, _inlineLabel);
      }

/*
      if (!string.IsNullOrEmpty(PlaceholderAttr)) {
        w.AddAttribute(ControlFactory.REQUIRED_ATTR, PlaceholderAttr);
      }
 */

      classAttributes += " " + ControlFactory.BOOTSTRAP_FORM_CLASS;
/* ------------------------------------------------------------------------
 * must add at this stage in the Page life cycle to allow
 * setting 'required' flag on/off!
 * 
 * HTML 'class' attribute, for client-side validation;
 * ------------------------------------------------------------------------
*/
      if (required) {
        w.AddAttribute(ControlFactory.REQUIRED_ATTR, null);
      }
// server-side error highlighting
 	    if (Page.IsPostBack) {
 	      if (_rfv != null && !_rfv.IsValid
 	        || _rev != null && !_rev.IsValid
 	        || _cv != null && !_cv.IsValid
 	      ) 
 	      {
   	      classAttributes += " " + ControlFactory.ERROR_CLASS;
          ControlFactory.AddServerRequiredStyle(w);
 	      }
 	    }
// HTML class attribute(s) 	          
      if (!string.IsNullOrEmpty(classAttributes)) {
        w.AddAttribute(HtmlTextWriterAttribute.Class, ClassAttributes);
      }

      base.Render(w);
// server-side validator
      foreach (Control c in Controls) {
        if (c is BaseValidator) c.RenderControl(w);
      }
    }
/*
 * ###########################################################################
 * client / server side validation
 * ###########################################################################
*/
// client-side; HTML tag 'class' attribute
    protected string classAttributes;
    public string ClassAttributes {
      get { 
        return classAttributes != null 
          ? classAttributes.Trim() : "";
      }
    } 
// ---------------------------------------------------------------------------
// server-side AND client; String / Integer / Double / Date / Currency
    public virtual ValidationDataType ValidationDataType {
      get; set;
    }
/*
 * ###########################################################################
 * add m$ validators
 * ###########################################################################
*/
// server-side AND client;
    private string _regex;
    public virtual string regex {
      get { return _regex; }
      set { _regex = value; }
    }    
// ---------------------------------------------------------------------------
// server-side AND client;
    private string _regexErrorMessage;
    public virtual string RegexErrorMessage {
      get { return _regexErrorMessage; }
      set { _regexErrorMessage = value; }
    }
    private RegularExpressionValidator _rev;
    protected virtual void addRegexValidator() {
// HTML attributes, client-side validation    
      // classAttributes += " " + ControlFactory.REGEX;
      Attributes.Add(ControlFactory.REGEX, regex);
      Attributes.Add(ControlFactory.REGEX_ERROR_MSG, 
        string.IsNullOrEmpty(RegexErrorMessage)
          ? regex : RegexErrorMessage
      );    
      _rev = new RegularExpressionValidator() {
        ControlToValidate = this.ID, 
        ID = this.ID + ControlFactory.REGEX_ID_SUFFIX,
        ValidationExpression = regex, 
        EnableClientScript = false, 
        Display = ValidatorDisplay.Dynamic,
        ErrorMessage = " " + RegexErrorMessage,
      };
      if (this.ValidationGroup != String.Empty) {
        _rev.ValidationGroup = this.ValidationGroup;
      }
      Controls.Add(_rev);
    }
/*
 * ###########################################################################
 * CompareValidator => datebox server-side/client validation
 * ###########################################################################
*/
    private CompareValidator _cv;
    protected virtual void AddCompareValidator() {
// ignore default    
      if ( ValidationDataType.Equals(ValidationDataType.String) ) {
        return;
      }
      
      if ( ValidationDataType == ValidationDataType.Date ) {
        CompareValidatorErrorMessage = " Invalid date";
      }
      _cv = new CompareValidator() {
        ControlToValidate = this.ID, 
        ID = this.ID + "_compareValidator",
        Type = ValidationDataType, 
        Operator = ValidationCompareOperator.DataTypeCheck,
        Display = ValidatorDisplay.Dynamic, 
        EnableClientScript = false,
        ErrorMessage = " " + CompareValidatorErrorMessage
      };
      if (this.ValidationGroup != String.Empty) {
        _cv.ValidationGroup = this.ValidationGroup;
      }
      Controls.Add(_cv);
    }
// ---------------------------------------------------------------------------
// server-side AND client;
    private string _compareValidatorErrorMessage;
    public string CompareValidatorErrorMessage {
      get { return _compareValidatorErrorMessage; }
      set { _compareValidatorErrorMessage = value; }
    }    
// ===========================================================================
  }
}