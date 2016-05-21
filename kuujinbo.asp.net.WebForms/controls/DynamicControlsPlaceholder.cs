/* ###########################################################################
 * DynamicControlsPlaceholder solves the problem that dynamically added
 * controls are not automatically recreated on subsequent requests.
 *
 * control uses ViewState to store the types of child controls
 * recursively and recreates them automatically.
 *
 * Please note that property values that are set before "TrackViewState"
 * is called (usually in Controls.Add) are not persisted.
 *
 * modified from original source code, for asp.net >= 2.0:
 * http://www.denisbauer.com/ASPNETControls/DynamicControlsPlaceholder.aspx
 * ###########################################################################
*/
using System;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace kuujinbo.asp.net.WebForms.controls {
  [System.ComponentModel.ToolboxItem(false)] 
	public class DynamicControlsPlaceholder : PlaceHolder {
// ============================================================================	
//  Occurs when a control has been restored from ViewState
    public event DynamicControlEventHandler ControlRestored;
//  Occurs when the DynamicControlsPlaceholder is about to restore the child controls from ViewState
    public event EventHandler PreRestore;
//  Occurs after the DynamicControlsPlaceholder has restored the child controls from ViewState
    public event EventHandler PostRestore;
// ----------------------------------------------------------------------------
//  Raises the <see cref="ControlRestored">ControlRestored</see> event.
//  <param name="e">The <see cref="DynamicControlEventArgs">DynamicControlEventArgs</see> object that contains the event data.</param>
		protected virtual void OnControlRestored(DynamicControlEventArgs e) {
			if (ControlRestored != null) ControlRestored(this, e);
		}
// ----------------------------------------------------------------------------
//  Raises the <see cref="PreRestore">PreRestore</see> event.
//  <param name="e">The <see cref="System.EventArgs">EventArgs</see> object that contains the event data.</param>
		protected virtual void OnPreRestore(EventArgs e) {
			if (PreRestore != null) PreRestore(this, e);
		}
// ----------------------------------------------------------------------------
//  Raises the <see cref="PostRestore">PostRestore</see> event.
//  <param name="e">The <see cref="System.EventArgs">EventArgs</see> object that contains the event data.</param>
		protected virtual void OnPostRestore(EventArgs e) {
			if (PostRestore != null) PostRestore(this, e);
		}
// ----------------------------------------------------------------------------
//  Specifies whether Controls without IDs shall be persisted or if an exception shall be thrown
    [DefaultValue(HandleDynamicControls.DontPersist)]
		public HandleDynamicControls ControlsWithoutIDs {
			get {
			  return ViewState["ControlsWithoutIDs"] == null
			    ? HandleDynamicControls.DontPersist
			    : (HandleDynamicControls) ViewState["ControlsWithoutIDs"]
			  ;
			}
			set { ViewState["ControlsWithoutIDs"] = value; }
		}
// ----------------------------------------------------------------------------
//  Recreates all dynamically added child controls of the Placeholder and then calls the default 
//  LoadViewState mechanism
//  <param name="savedState">Array of objects that contains the child structure in the first item, 
//  and the base ViewState in the second item</param>
		protected override void LoadViewState(object savedState) {
			object[] viewState = (object[]) savedState;

			//Raise PreRestore event
			OnPreRestore(EventArgs.Empty);

			//recreate the child controls recursively
			Pair persistInfo = (Pair) viewState[0];
			foreach (Pair pair in (ArrayList) persistInfo.Second) {
				RestoreChildStructure(pair, this);
			}

			//Raise PostRestore event
			OnPostRestore(EventArgs.Empty);

			base.LoadViewState(viewState[1]);
		}
// ----------------------------------------------------------------------------
//  Walks recursively through all child controls and stores their type in ViewState and then calls the default 
//  SaveViewState mechanism
//  <returns>Array of objects that contains the child structure in the first item, 
//  and the base ViewState in the second item</returns>
		protected override object SaveViewState() {
			if (HttpContext.Current == null) return null;

			object[] viewState = new object[2];
			viewState[0] = PersistChildStructure(this, "C");
			viewState[1] = base.SaveViewState();
			return viewState;
		}
// ----------------------------------------------------------------------------
//  Recreates a single control and recursively calls itself for all child controls
//  <param name="persistInfo">A pair that contains the controls persisted information in the first property,
//  and an ArrayList with the child's persisted information in the second property</param>
//  <param name="parent">The parent control to which Controls collection it is added</param>
		private void RestoreChildStructure(Pair persistInfo, Control parent) {
			Control control;
			string[] persistedString = persistInfo.First.ToString().Split(';');
			string[] typeName = persistedString[1].Split(':');
  		Type type = Type.GetType(typeName[1], true, true);
			
			switch (typeName[0]) {
				// restore the UserControl by calling Page.LoadControl
				case "UC":
					try {
// calling the overload Page.LoadControl(type, null) via reflection 
// (which is not very nice but necessary when compiled against 1.0)
						MethodInfo mi = typeof(Page).GetMethod("LoadControl", new Type[2] { typeof(Type), typeof(object[]) });
							control = (Control) mi.Invoke(this.Page, new object[2] { type, null });
					}
					catch (Exception e) {
						throw new ArgumentException(
						  String.Format(
						    "The type '{0}' cannot be recreated from ViewState", 
						    type.ToString() 
						  ), 
						  e
						);
					}
					break;
				case "C":
					try {
						control = (Control) Activator.CreateInstance(type);
					}
					catch(Exception e) {
						throw new ArgumentException(
						  String.Format(
						    "The type '{0}' cannot be recreated from ViewState", type.ToString()
						  ), 
						  e
						);
					}
					break;
				default:
					throw new ArgumentException("Unknown type - cannot recreate from ViewState");
			}
			control.ID = persistedString[2];

			switch(persistedString[0]) {
				case "C":
					parent.Controls.Add(control);
					break;
			}

			// Raise OnControlRestoredEvent
			OnControlRestored(new DynamicControlEventArgs(control));

			// recreate all the child controls
			foreach(Pair pair in (ArrayList) persistInfo.Second) {
				RestoreChildStructure(pair, control);
			}
    }
// ----------------------------------------------------------------------------
//  Saves a single control and recursively calls itself to save all child controls
//  <param name="control">reference to the control</param>
//  <param name="controlCollectionName">contains an abbreviation to indicate to which control collection the control belongs</param>
//  <returns>A pair that contains the controls persisted information in the first property,
//  and an ArrayList with the child's persisted information in the second property</returns>
		private Pair PersistChildStructure(Control control, string controlCollectionName)	{
			// verify control has ID
      if (control.ID == null) {
				if (ControlsWithoutIDs == HandleDynamicControls.ThrowException) {
					throw new NotSupportedException(
					  "DynamicControlsPlaceholder does not support child controls whose ID is not set, as this may have unintended side effects: " 
					  + control.GetType().ToString()
					);
				}
				else if (ControlsWithoutIDs == HandleDynamicControls.DontPersist) {
					return null;
				}
			}
			
			string typeName = control is UserControl ? "UC:" : "C:";
			typeName += control.GetType().AssemblyQualifiedName;

			ArrayList childPersistInfo = new ArrayList();
			string persistedString = controlCollectionName + ";" + typeName + ";" + control.ID;

			// UserControl children need not be saved; recreated on Page.LoadControl, same for CheckBoxList
			if ( !(control is UserControl) && !(control is CheckBoxList) ) {
				// save all child controls from "Controls" collection
				for (int counter = 0; counter < control.Controls.Count; counter++) {
					Control child = control.Controls[counter];
					Pair pair = PersistChildStructure(child, "C");
					if (pair != null) childPersistInfo.Add(pair);
				}
			}

			return new Pair(persistedString, childPersistInfo);
		}
// ============================================================================
// END DynamicControlsPlaceholder class
// ============================================================================
  }
// ----------------------------------------------------------------------------
//  Specifies the possibilities if controls shall be persisted or not
  public enum HandleDynamicControls {
	  //  DynamicControl shall not be persisted
	  DontPersist,
	  //  DynamicControl shall be persisted
	  Persist,
	  //  An Exception shall be thrown
	  ThrowException
  }
// ----------------------------------------------------------------------------
//  Represents the method that will handle any DynamicControl event.
  [Serializable]
  public delegate void DynamicControlEventHandler(object sender, DynamicControlEventArgs e);
// ----------------------------------------------------------------------------
//  Provides data for the ControlRestored event
  public class DynamicControlEventArgs : EventArgs {
		private Control _dynamicControl;
// ----------------------------------------------------------------------------
//  Gets the referenced Control when the event is raised
    public Control DynamicControl {
			get { return _dynamicControl; }
		}
// ----------------------------------------------------------------------------
//  Initializes a new instance of DynamicControlEventArgs class.
//  <param name="dynamicControl">The control that was just restored.</param>
		public DynamicControlEventArgs(Control dynamicControl) {
			_dynamicControl = dynamicControl;
		}	
	}
// ============================================================================
// END DynamicControlEventArgs class
// ============================================================================	
}