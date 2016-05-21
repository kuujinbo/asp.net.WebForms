/* ###########################################################################
 * add functionality to m$  m$ CheckBox control
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
    ValidationProperty("val"),
    AspNetHostingPermissionAttribute(
      SecurityAction.InheritanceDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    ),
    AspNetHostingPermissionAttribute(
      SecurityAction.LinkDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    )
  ]
  [ToolboxData("<{0}:checkbox ID='CheckBoxID' runat='server' />")]
  public class checkbox : CheckBox, Icontrol {
// ===========================================================================  
/*
 * Interface implementation that allows us to use reflection to get 
 * control's **STRINGIFIED** value
*/
    [Browsable(false)]
    public string val {
// we can't just return this.Checked for __validation__ purposes,
// because any __non-empty__ string will be flagged as valid;
// i.e. 'false'
      get { return this.Checked ? this.Checked.ToString() : ""; }
      set {
        bool _val_flag;
        if ( Boolean.TryParse(value, out _val_flag) ) {
          this.Checked = _val_flag;
        }
        else {
          throw new ArgumentException(string.Format(
            "died in {0}.val: not Boolean.TrueString or Boolean.FalseString",
            this.ToString()
          ));
        }
      }
    }
// ---------------------------------------------------------------------------   
// **OPTIONAL** => control's HTML <label>
    private string _label;
    [
      Category("kuujinbo :: appearance"),
      Description("control's <label> text description; contained in <div>")
    ]
    public string label {
      get { return _label; }
      set { _label = value.Trim(); }
    }    
// ---------------------------------------------------------------------------   
// TODO: placeholder for now to comply w/ISES_entry_field;
    private bool _required;
      [
      Category("kuujinbo :: validation"),
      Description("add RequiredFieldValidator"),
      TypeConverter(typeof(BooleanConverter))
    ]
    public bool required {
      get { return _required; }
      set { _required = value; }
    }
/* ---------------------------------------------------------------------------
 * control is a WebControl
 * ---------------------------------------------------------------------------
*/    
// get/set 'ID' Property
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
 * server-side/client validation
 * ###########################################################################
*/
    private RequiredFieldValidator _rfv;
    
// client-side; HTML tag 'class' attribute
    protected string classAttributes;
    public string ClassAttributes {
      get { 
        return classAttributes != null 
          ? ControlFactory.CONTROL_FLAG + " " + classAttributes.Trim() 
          : ControlFactory.CONTROL_FLAG; 
      }
    }    
/*
 * ###########################################################################
 * override parent class methods
 * ###########################################################################
*/
    protected override void OnInit(EventArgs e) {
      base.OnInit(e);
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
/* client-side validation => ValidationGroup */
      if (ValidationGroup != String.Empty) {
        Attributes.Add(ControlFactory.VALIDATION_GROUP_ATTR, ValidationGroup);
      }           
    }
// ---------------------------------------------------------------------------
// second to last stage of Page processing; Page object calls method on 
// control to write out control's markup to sent to browser.
    protected override void Render(HtmlTextWriter w) {      
/*
 * must add at this stage in the Page life cycle to allow
 * setting 'required' flag on/off!
 * 
 * HTML 'class' attribute, for client-side validation;  
*/
      if (required) {
        classAttributes += " " + ControlFactory.CHECKBOX_CLASS;
        w.AddAttribute(ControlFactory.REQUIRED_ATTR, null);
      }
// server-side error highlighting
 	    if (Page.IsPostBack && _rfv != null && !_rfv.IsValid) {
 	      classAttributes += " " + ControlFactory.ERROR_CLASS;
        ControlFactory.AddServerRequiredStyle(w);
 	    }
// HTML class attribute(s) 	          
      if (!string.IsNullOrEmpty(classAttributes)) {
        w.AddAttribute(HtmlTextWriterAttribute.Class, ClassAttributes);
      }
      
      base.Render(w);
      
/*
      if (_label != null) {
        if (required) {
          _label = ControlFactory.REQUIRED_FLAG + _label;
        }
        w.Write(ControlFactory.SPAN_LABEL_FORMAT, ClientID, _label);
      }  
*/      
  
// server-side validator      
      if (required && _rfv != null) {
        _rfv.RenderControl(w);
      }
    }
// ===========================================================================
  }
}