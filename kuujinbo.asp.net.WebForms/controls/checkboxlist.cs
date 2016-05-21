/* ###########################################################################
 * add functionality to m$ CheckBoxList control
 * ###########################################################################
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly:TagPrefix("kuujinbo.asp.net.WebForms.controls", "kuujinbo")]
namespace kuujinbo.asp.net.WebForms.controls {
  [
// m$ CheckBoxList does NOT support validation with RequiredFieldValidator   
    ValidationProperty("val"),
    AspNetHostingPermissionAttribute(
      SecurityAction.InheritanceDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    )
    ,AspNetHostingPermissionAttribute(
      SecurityAction.LinkDemand, 
      Level = AspNetHostingPermissionLevel.Minimal
    )
    ,ToolboxData("<{0}:checkboxlist ID='CheckBoxListID' runat='server' />")
  ] 
  public class checkboxlist : CheckBoxList, Icontrol {
// ===========================================================================  
/*
 * Interface implementation that allows us to use reflection to get 
 * control's **STRINGIFIED** value
*/
    [Browsable(false)]
    public string val {
      get {
        List<string> l = new List<string>();
        ListItemCollection lic = this.Items;
        for (int i = 0; i < lic.Count; ++i) {
          if (lic[i].Selected) l.Add(lic[i].Value);
        }
        return String.Join(",", l.ToArray());
      }
      set {
        List<string> l = new List<string>(value.Split(new Char [] {','}));
        ListItemCollection lic = this.Items;
        for (int i = 0; i < lic.Count; ++i) {
          if (l.Contains(lic[i].Value)) lic[i].Selected = true;
        }
      }
    }
// ---------------------------------------------------------------------------- 
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
 * client-side upgrades
 * ###########################################################################
*/
    private bool _enableJavaScriptCheckAll;
    [
    Category("kuujinbo :: client-side script"),
    Description("add JavaScript 'Check All' and 'Clear All' functions")
    ]
    public bool EnableJavaScriptCheckAll {
      get { return _enableJavaScriptCheckAll; }
      set { _enableJavaScriptCheckAll = value; }
    }
/*
 * ###########################################################################
 * convenience methods
 * ###########################################################################
*/
// get all checked listitem values
    public List<string> SelectedValues() {
      List<string> l = new List<string>();
      foreach (ListItem li in Items) {
        if (li.Selected ) l.Add(li.Value);
      }
      return l;
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
          ? classAttributes.Trim() : "";
      }
    }    
    
/*
 * ###########################################################################
 * override parent class methods
 * ###########################################################################
*/
// ----------------------------------------------------------------------------
// **MUST** be called in this event!!
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
/* ------------------------------------------------------------------------
 * second to last stage of Page processing; Page object calls method on
 * control to write out control's markup to sent to browser.
 * ------------------------------------------------------------------------
*/
    protected override void Render(HtmlTextWriter w) {
      if (_label != null) {
        if (required) {
          _label = ControlFactory.REQUIRED_FLAG + _label;
        }
        w.Write(ControlFactory.DIV_LABEL_FORMAT, ClientID, _label);
      }
// for client-side validation
      w.Write("<span>");
/*
 * must add at this stage in the Page life cycle to allow
 * setting 'required' flag on/off!
 * 
 * HTML 'class' attribute, for client-side validation;  
*/

/* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 * control has extra class attributes compared to OTHER CONTROLS IN PROJECT
 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
*/
      if (required) {
        classAttributes += ControlFactory.CHECKBOX_CLASS;
        //classAttributes += " " + ControlFactory.LIST_MULTIPLE_REQUIRED_CLASS
          // + " " + ControlFactory.REQUIRED_ATTR
        //;
        w.AddAttribute(ControlFactory.REQUIRED_ATTR, null);
      }
      if (EnableJavaScriptCheckAll) {
        classAttributes += " " + ControlFactory.CHECKBOXLIST_CHECKALL_CLASS;
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
      
// server-side validator      
      if (required && _rfv != null) {
        _rfv.RenderControl(w);
      }
      w.Write("</span>");
    } 

/* ###########################################################################
 * m$ bug present since .net 1.XX; listitem collection does **NOT**
 * persist viewstate
 * http://www.4guysfromrolla.com/articles/110205-1.aspx
 * ###########################################################################
*/
/* ---------------------------------------------------------------------------
 * [1] persist state if **ANY** listitem has attribute collection
 */
    protected override object SaveViewState() {
// first element => control viewstate, and one element for each listitem
      object [] state = new object[this.Items.Count + 1];

      object baseState = base.SaveViewState();
      state[0] = baseState;

// flag default case => **NO** listitem has attributes
      bool useBaseState = true;
      for (int i = 0; i < this.Items.Count; i++) {
        if (this.Items[i].Attributes.Count > 0) {
          useBaseState = false;
// save listitem atribute key/value pairs
          object [] attribKV = new object[this.Items[i].Attributes.Count * 2];
          int k = 0;
          foreach(string key in this.Items[i].Attributes.Keys) {
            attribKV[k++] = key;
            attribKV[k++] = this.Items[i].Attributes[key];
          }
          state[i+1] = attribKV;
        }
      }
      return useBaseState ? baseState : state;
    }
/* ---------------------------------------------------------------------------
 * [2] load from viewstate if saved
 */
    protected override void LoadViewState(object savedState) {
      if (savedState == null) return;

// load itemcollection viewstate 
      if (savedState is object[]) {
        object [] state = (object[]) savedState;
        base.LoadViewState(state[0]);   // load the base state
        for (int i = 1; i < state.Length; i++) {
          if (state[i] != null) {
            object [] attribKV = (object[]) state[i];
            for (int k = 0; k < attribKV.Length; k += 2) {
              this.Items[i-1].Attributes.Add(
                attribKV[k].ToString(), attribKV[k+1].ToString()
              );
            }
          }
        }
      }
// load default control viewstate
      else {
        base.LoadViewState(savedState);
      }
    }    
// ===========================================================================
  }
}