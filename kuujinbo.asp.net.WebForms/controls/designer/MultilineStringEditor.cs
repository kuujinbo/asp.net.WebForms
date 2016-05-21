/* ########################################################################
 * System.ComponentModel.Design.MultilineStringEditor is a sealed class.
 * you CANNOT reuse the code and have to roll your own!! not only that, 
 * GetEditStyle property is READONLY!! 
 * ########################################################################
*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;    
using System.Windows.Forms.Design;

namespace kuujinbo.asp.net.WebForms.controls.design {
  public class MultilineStringEditor : UITypeEditor  {
    public override object EditValue(ITypeDescriptorContext context,
      IServiceProvider serviceProvider, object value
    ) 
    {
      if (
        (context != null) &&
        (serviceProvider != null)
        )
      {
        IWindowsFormsEditorService edSvc =
            (IWindowsFormsEditorService)serviceProvider.GetService(
            typeof(IWindowsFormsEditorService)
        );

        if (edSvc != null) {
          StringEditorForm form = new StringEditorForm();
          form.Value = (string)value;
          DialogResult result = edSvc.ShowDialog(form);
          if (result == DialogResult.OK) value = form.Value;
        }
      }
      return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(
        ITypeDescriptorContext context
      ) 
    {
      return context != null ?
        UITypeEditorEditStyle.Modal : base.GetEditStyle(context)
      ;
    } 
/*
 * ###########################################################################
 * form UI
 * ###########################################################################
*/
    public class StringEditorForm : Form {
      private TextBox textBox1;
      private Button okButton;
      private Button cancelButton;

      public StringEditorForm() {
        InitializeComponent();
      }

      public string Value {
        get { return textBox1.Text; }
        set { textBox1.Text = value; }
      }

      private void InitializeComponent() {
        textBox1 = new TextBox();
        okButton = new Button();
        cancelButton = new Button();
        SuspendLayout();
        // textBox1
        textBox1.AcceptsReturn = true;
        textBox1.Anchor = (
          (
            (AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left
          ) 
          | AnchorStyles.Right
        );
        textBox1.Location = new Point(12, 11);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.ScrollBars = ScrollBars.Both;
        textBox1.Size = new Size(521, 233);
        textBox1.TabIndex = 0;
        textBox1.Text = "";
        textBox1.WordWrap = false;
        // okButton
        okButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
        okButton.DialogResult = DialogResult.OK;
        okButton.Location = new Point(302, 255);
        okButton.Name = "okButton";
        okButton.Size = new Size(111, 32);
        okButton.TabIndex = 1;
        okButton.Text = "OK";
        // cancelButton
        cancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(426, 255);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(111, 32);
        cancelButton.TabIndex = 2;
        cancelButton.Text = "Cancel";
        // StringEditorForm
        AcceptButton = okButton;
        AutoScaleBaseSize = new Size(7, 17);
        CancelButton = cancelButton;
        ClientSize = new Size(544, 295);
        Controls.AddRange(new Control[] {
          cancelButton, okButton, textBox1
        });
        Font = new Font("Tahoma", 8F);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "kuujinbo.asp.net.WebForms.controls Multiline String Editor";
        ShowInTaskbar = false;
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterParent;
        Text = "kuujinbo.asp.net.WebForms.controls Multiline String Editor";
        ResumeLayout(false);
      }
    }  
  }
}