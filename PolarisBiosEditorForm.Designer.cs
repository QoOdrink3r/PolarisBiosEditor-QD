namespace PolarisBiosEditor
{
    partial class PolarisBiosEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UserControl = new PolarisBiosEditor.PolarisBiosEditorUserControl();
            this.SuspendLayout();
            // 
            // UserControl
            // 
            this.UserControl.AutoSize = true;
            this.UserControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.UserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserControl.Location = new System.Drawing.Point(0, 0);
            this.UserControl.Name = "UserControl";
            this.UserControl.Size = new System.Drawing.Size(1294, 671);
            this.UserControl.TabIndex = 0;
            // 
            // PolarisBiosEditorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1294, 671);
            this.Controls.Add(this.UserControl);
            this.Name = "PolarisBiosEditorForm";
            this.Text = "PolarisBiosEdtiorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PolarisBiosEditorUserControl UserControl;
    }
}