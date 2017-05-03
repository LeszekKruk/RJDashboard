namespace RJDashboard
{
    partial class ProblemSolution
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProblemSolution));
            this.mPanelTitle = new MetroFramework.Controls.MetroPanel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.lblErrorSolution = new MetroFramework.Controls.MetroLabel();
            this.lblErrorType = new MetroFramework.Controls.MetroLabel();
            this.tileClose = new MetroFramework.Controls.MetroTile();
            this.SuspendLayout();
            // 
            // mPanelTitle
            // 
            this.mPanelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mPanelTitle.BackColor = System.Drawing.Color.Red;
            this.mPanelTitle.BorderStyle = MetroFramework.Drawing.MetroBorderStyle.FixedSingle;
            this.mPanelTitle.CustomBackground = true;
            this.mPanelTitle.HorizontalScrollbarBarColor = true;
            this.mPanelTitle.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanelTitle.HorizontalScrollbarSize = 8;
            this.mPanelTitle.Location = new System.Drawing.Point(0, 56);
            this.mPanelTitle.Margin = new System.Windows.Forms.Padding(2);
            this.mPanelTitle.Name = "mPanelTitle";
            this.mPanelTitle.Size = new System.Drawing.Size(683, 17);
            this.mPanelTitle.Style = MetroFramework.MetroColorStyle.Red;
            this.mPanelTitle.TabIndex = 9;
            this.mPanelTitle.VerticalScrollbarBarColor = false;
            this.mPanelTitle.VerticalScrollbarHighlightOnWheel = false;
            this.mPanelTitle.VerticalScrollbarSize = 8;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel2.Location = new System.Drawing.Point(10, 191);
            this.metroLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(173, 19);
            this.metroLabel2.TabIndex = 13;
            this.metroLabel2.Text = "Propozycja rozwiązania:";
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.Location = new System.Drawing.Point(10, 82);
            this.metroLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(71, 19);
            this.metroLabel1.TabIndex = 12;
            this.metroLabel1.Text = "Problem:";
            // 
            // lblErrorSolution
            // 
            this.lblErrorSolution.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblErrorSolution.Location = new System.Drawing.Point(10, 216);
            this.lblErrorSolution.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblErrorSolution.Name = "lblErrorSolution";
            this.lblErrorSolution.Size = new System.Drawing.Size(657, 130);
            this.lblErrorSolution.TabIndex = 11;
            this.lblErrorSolution.Text = "Propozycja rozwiązania";
            // 
            // lblErrorType
            // 
            this.lblErrorType.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblErrorType.Location = new System.Drawing.Point(10, 106);
            this.lblErrorType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblErrorType.Name = "lblErrorType";
            this.lblErrorType.Size = new System.Drawing.Size(657, 78);
            this.lblErrorType.TabIndex = 10;
            this.lblErrorType.Text = "Typ błędu";
            // 
            // tileClose
            // 
            this.tileClose.BackColor = System.Drawing.Color.Red;
            this.tileClose.CustomBackground = true;
            this.tileClose.CustomForeColor = true;
            this.tileClose.ForeColor = System.Drawing.SystemColors.Control;
            this.tileClose.Location = new System.Drawing.Point(302, 358);
            this.tileClose.Margin = new System.Windows.Forms.Padding(2);
            this.tileClose.Name = "tileClose";
            this.tileClose.Size = new System.Drawing.Size(90, 55);
            this.tileClose.Style = MetroFramework.MetroColorStyle.Red;
            this.tileClose.TabIndex = 14;
            this.tileClose.Text = "Zamknij";
            this.tileClose.TileImage = ((System.Drawing.Image)(resources.GetObject("tileClose.TileImage")));
            this.tileClose.TileImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tileClose.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.tileClose.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.tileClose.UseTileImage = true;
            this.tileClose.Click += new System.EventHandler(this.tileClose_Click);
            // 
            // Information
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 423);
            this.Controls.Add(this.tileClose);
            this.Controls.Add(this.mPanelTitle);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.lblErrorSolution);
            this.Controls.Add(this.lblErrorType);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Information";
            this.Resizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Text = "Komunikat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel mPanelTitle;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel lblErrorSolution;
        private MetroFramework.Controls.MetroLabel lblErrorType;
        private MetroFramework.Controls.MetroTile tileClose;
    }
}