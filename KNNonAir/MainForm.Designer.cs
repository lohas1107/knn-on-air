namespace KNNonAir
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.gmap = new GMap.NET.WindowsForms.GMapControl();
            this.gmapStatusStrip = new System.Windows.Forms.StatusStrip();
            this.gmapToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.fileToolStrip = new System.Windows.Forms.ToolStrip();
            this.readFileToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.addRoadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPoIsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gmapStatusStrip.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.fileToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // gmap
            // 
            this.gmap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gmap.Bearing = 0F;
            this.gmap.CanDragMap = true;
            this.gmap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gmap.GrayScaleMode = false;
            this.gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gmap.LevelsKeepInMemmory = 5;
            this.gmap.Location = new System.Drawing.Point(0, 0);
            this.gmap.MarkersEnabled = true;
            this.gmap.MaxZoom = 18;
            this.gmap.MinZoom = 2;
            this.gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gmap.Name = "gmap";
            this.gmap.NegativeMode = false;
            this.gmap.PolygonsEnabled = true;
            this.gmap.RetryLoadTile = 0;
            this.gmap.RoutesEnabled = true;
            this.gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gmap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gmap.ShowTileGridLines = false;
            this.gmap.Size = new System.Drawing.Size(784, 514);
            this.gmap.TabIndex = 0;
            this.gmap.Zoom = 7D;
            this.gmap.MouseLeave += new System.EventHandler(this.MouseLeaveGMap);
            this.gmap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveGMap);
            // 
            // gmapStatusStrip
            // 
            this.gmapStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gmapToolStripStatusLabel});
            this.gmapStatusStrip.Location = new System.Drawing.Point(0, 539);
            this.gmapStatusStrip.Name = "gmapStatusStrip";
            this.gmapStatusStrip.Size = new System.Drawing.Size(784, 22);
            this.gmapStatusStrip.TabIndex = 1;
            // 
            // gmapToolStripStatusLabel
            // 
            this.gmapToolStripStatusLabel.Name = "gmapToolStripStatusLabel";
            this.gmapToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripContainer
            // 
            this.toolStripContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.gmap);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(784, 514);
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(784, 539);
            this.toolStripContainer.TabIndex = 2;
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.fileToolStrip);
            // 
            // fileToolStrip
            // 
            this.fileToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.fileToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readFileToolStripSplitButton});
            this.fileToolStrip.Location = new System.Drawing.Point(3, 0);
            this.fileToolStrip.Name = "fileToolStrip";
            this.fileToolStrip.Size = new System.Drawing.Size(75, 25);
            this.fileToolStrip.TabIndex = 0;
            // 
            // readFileToolStripSplitButton
            // 
            this.readFileToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.readFileToolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRoadsToolStripMenuItem,
            this.addPoIsToolStripMenuItem});
            this.readFileToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("readFileToolStripSplitButton.Image")));
            this.readFileToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.readFileToolStripSplitButton.Name = "readFileToolStripSplitButton";
            this.readFileToolStripSplitButton.Size = new System.Drawing.Size(32, 22);
            // 
            // addRoadsToolStripMenuItem
            // 
            this.addRoadsToolStripMenuItem.Name = "addRoadsToolStripMenuItem";
            this.addRoadsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addRoadsToolStripMenuItem.Text = "Add Roads";
            this.addRoadsToolStripMenuItem.Click += new System.EventHandler(this.ClickAddRoadsToolStripMenuItem);
            // 
            // addPoIsToolStripMenuItem
            // 
            this.addPoIsToolStripMenuItem.Name = "addPoIsToolStripMenuItem";
            this.addPoIsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addPoIsToolStripMenuItem.Text = "Add PoIs";
            this.addPoIsToolStripMenuItem.Click += new System.EventHandler(this.ClickAddLandMarkToolStripMenuItem);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.toolStripContainer);
            this.Controls.Add(this.gmapStatusStrip);
            this.Name = "MainForm";
            this.Text = "kNN Search";
            this.gmapStatusStrip.ResumeLayout(false);
            this.gmapStatusStrip.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.fileToolStrip.ResumeLayout(false);
            this.fileToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gmap;
        private System.Windows.Forms.StatusStrip gmapStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel gmapToolStripStatusLabel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStrip fileToolStrip;
        private System.Windows.Forms.ToolStripSplitButton readFileToolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem addRoadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPoIsToolStripMenuItem;
    }
}

