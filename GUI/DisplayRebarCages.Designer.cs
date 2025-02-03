namespace SET_MAIN_DETAIL
{
    partial class DisplayRebarCages
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
            this.treeView = new System.Windows.Forms.TreeView();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SetManeOneCage_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Location = new System.Drawing.Point(2, 3);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(481, 593);
            this.treeView.TabIndex = 0;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(489, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(416, 508);
            this.propertyGrid.TabIndex = 1;
            // 
            // SetManeOneCage_button
            // 
            this.SetManeOneCage_button.Location = new System.Drawing.Point(536, 529);
            this.SetManeOneCage_button.Name = "SetManeOneCage_button";
            this.SetManeOneCage_button.Size = new System.Drawing.Size(224, 50);
            this.SetManeOneCage_button.TabIndex = 2;
            this.SetManeOneCage_button.Text = "Указать каркас";
            this.SetManeOneCage_button.UseVisualStyleBackColor = true;
            this.SetManeOneCage_button.Click += new System.EventHandler(this.SetManeOneCage_button_Click);
            // 
            // DisplayRebarCages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 608);
            this.Controls.Add(this.SetManeOneCage_button);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.treeView);
            this.Name = "DisplayRebarCages";
            this.Text = "UI";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button SetManeOneCage_button;
    }
}