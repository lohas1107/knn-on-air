﻿namespace KNNonAir.Presentation
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tuning = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileToolStrip = new System.Windows.Forms.ToolStrip();
            this.readFileToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.addRoadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPoIsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRoadsPoIsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNVDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addEBTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNPITableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveRoadsAndPoIsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNVDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveEBTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNPITableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parameterToolStrip = new System.Windows.Forms.ToolStrip();
            this.locationToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.algorithmToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.partitionToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.packetToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.kToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.actionToolStrip = new System.Windows.Forms.ToolStrip();
            this.nvdToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.partitionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quadTreeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tableToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.searchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.gmapStatusStrip.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.fileToolStrip.SuspendLayout();
            this.parameterToolStrip.SuspendLayout();
            this.actionToolStrip.SuspendLayout();
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
            this.gmap.Size = new System.Drawing.Size(784, 515);
            this.gmap.TabIndex = 0;
            this.gmap.Zoom = 7D;
            this.gmap.MouseLeave += new System.EventHandler(this.MouseLeaveGMap);
            this.gmap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveGMap);
            // 
            // gmapStatusStrip
            // 
            this.gmapStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gmapToolStripStatusLabel});
            this.gmapStatusStrip.Location = new System.Drawing.Point(0, 540);
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
            this.toolStripContainer.ContentPanel.Controls.Add(this.dataGridView);
            this.toolStripContainer.ContentPanel.Controls.Add(this.gmap);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(784, 515);
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(784, 540);
            this.toolStripContainer.TabIndex = 2;
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.fileToolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.parameterToolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.actionToolStrip);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Tuning});
            this.dataGridView.Location = new System.Drawing.Point(392, 422);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidth = 55;
            this.dataGridView.RowTemplate.Height = 24;
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView.Size = new System.Drawing.Size(392, 93);
            this.dataGridView.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Index Tree";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 81;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Table";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 56;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Regions";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 68;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Latency";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 67;
            // 
            // Tuning
            // 
            this.Tuning.HeaderText = "Tuning";
            this.Tuning.Name = "Tuning";
            this.Tuning.ReadOnly = true;
            this.Tuning.Width = 64;
            // 
            // fileToolStrip
            // 
            this.fileToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.fileToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readFileToolStripSplitButton,
            this.saveFileToolStripSplitButton});
            this.fileToolStrip.Location = new System.Drawing.Point(3, 0);
            this.fileToolStrip.Name = "fileToolStrip";
            this.fileToolStrip.Size = new System.Drawing.Size(76, 25);
            this.fileToolStrip.TabIndex = 0;
            // 
            // readFileToolStripSplitButton
            // 
            this.readFileToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.readFileToolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRoadsToolStripMenuItem,
            this.addPoIsToolStripMenuItem,
            this.addRoadsPoIsToolStripMenuItem,
            this.addNVDToolStripMenuItem,
            this.addEBTableToolStripMenuItem,
            this.addNPITableToolStripMenuItem});
            this.readFileToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("readFileToolStripSplitButton.Image")));
            this.readFileToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.readFileToolStripSplitButton.Name = "readFileToolStripSplitButton";
            this.readFileToolStripSplitButton.Size = new System.Drawing.Size(32, 22);
            this.readFileToolStripSplitButton.ToolTipText = "Read File";
            this.readFileToolStripSplitButton.ButtonClick += new System.EventHandler(this.ClickReadFileToolStripSplitButton);
            // 
            // addRoadsToolStripMenuItem
            // 
            this.addRoadsToolStripMenuItem.Name = "addRoadsToolStripMenuItem";
            this.addRoadsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addRoadsToolStripMenuItem.Text = "Add Roads";
            this.addRoadsToolStripMenuItem.Click += new System.EventHandler(this.ClickAddRoadsToolStripMenuItem);
            // 
            // addPoIsToolStripMenuItem
            // 
            this.addPoIsToolStripMenuItem.Name = "addPoIsToolStripMenuItem";
            this.addPoIsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addPoIsToolStripMenuItem.Text = "Add PoIs";
            this.addPoIsToolStripMenuItem.Click += new System.EventHandler(this.ClickAddPoIToolStripMenuItem);
            // 
            // addRoadsPoIsToolStripMenuItem
            // 
            this.addRoadsPoIsToolStripMenuItem.Name = "addRoadsPoIsToolStripMenuItem";
            this.addRoadsPoIsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addRoadsPoIsToolStripMenuItem.Text = "Add Roads and PoIs";
            this.addRoadsPoIsToolStripMenuItem.Click += new System.EventHandler(this.ClickAddRoadsPoIsToolStripMenuItem);
            // 
            // addNVDToolStripMenuItem
            // 
            this.addNVDToolStripMenuItem.Name = "addNVDToolStripMenuItem";
            this.addNVDToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addNVDToolStripMenuItem.Text = "Add NVD";
            this.addNVDToolStripMenuItem.Click += new System.EventHandler(this.ClickAddNVDToolStripMenuItem);
            // 
            // addEBTableToolStripMenuItem
            // 
            this.addEBTableToolStripMenuItem.Name = "addEBTableToolStripMenuItem";
            this.addEBTableToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addEBTableToolStripMenuItem.Text = "Add EB Table";
            this.addEBTableToolStripMenuItem.Click += new System.EventHandler(this.ClickAddEBTableToolStripMenuItem);
            // 
            // addNPITableToolStripMenuItem
            // 
            this.addNPITableToolStripMenuItem.Name = "addNPITableToolStripMenuItem";
            this.addNPITableToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addNPITableToolStripMenuItem.Text = "Add NPI Table";
            this.addNPITableToolStripMenuItem.Click += new System.EventHandler(this.ClickAddNPITableToolStripMenuItem);
            // 
            // saveFileToolStripSplitButton
            // 
            this.saveFileToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveFileToolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveRoadsAndPoIsToolStripMenuItem,
            this.saveNVDToolStripMenuItem,
            this.saveEBTableToolStripMenuItem,
            this.saveNPITableToolStripMenuItem});
            this.saveFileToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("saveFileToolStripSplitButton.Image")));
            this.saveFileToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveFileToolStripSplitButton.Name = "saveFileToolStripSplitButton";
            this.saveFileToolStripSplitButton.Size = new System.Drawing.Size(32, 22);
            this.saveFileToolStripSplitButton.ToolTipText = "Save File";
            // 
            // saveRoadsAndPoIsToolStripMenuItem
            // 
            this.saveRoadsAndPoIsToolStripMenuItem.Name = "saveRoadsAndPoIsToolStripMenuItem";
            this.saveRoadsAndPoIsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveRoadsAndPoIsToolStripMenuItem.Text = "Save Roads and PoIs";
            this.saveRoadsAndPoIsToolStripMenuItem.Click += new System.EventHandler(this.ClickSaveRoadsAndPoIsToolStripMenuItem);
            // 
            // saveNVDToolStripMenuItem
            // 
            this.saveNVDToolStripMenuItem.Name = "saveNVDToolStripMenuItem";
            this.saveNVDToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveNVDToolStripMenuItem.Text = "Save NVD";
            this.saveNVDToolStripMenuItem.Click += new System.EventHandler(this.ClickSaveNVDToolStripMenuItem);
            // 
            // saveEBTableToolStripMenuItem
            // 
            this.saveEBTableToolStripMenuItem.Name = "saveEBTableToolStripMenuItem";
            this.saveEBTableToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveEBTableToolStripMenuItem.Text = "Save EB Table";
            this.saveEBTableToolStripMenuItem.Click += new System.EventHandler(this.ClickSaveEBTableToolStripMenuItem);
            // 
            // saveNPITableToolStripMenuItem
            // 
            this.saveNPITableToolStripMenuItem.Name = "saveNPITableToolStripMenuItem";
            this.saveNPITableToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveNPITableToolStripMenuItem.Text = "Save NPI Table";
            this.saveNPITableToolStripMenuItem.Click += new System.EventHandler(this.ClickSaveNPITableToolStripMenuItem);
            // 
            // parameterToolStrip
            // 
            this.parameterToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.parameterToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.locationToolStripComboBox,
            this.algorithmToolStripComboBox,
            this.partitionToolStripComboBox,
            this.packetToolStripComboBox,
            this.kToolStripComboBox});
            this.parameterToolStrip.Location = new System.Drawing.Point(79, 0);
            this.parameterToolStrip.Name = "parameterToolStrip";
            this.parameterToolStrip.Size = new System.Drawing.Size(241, 25);
            this.parameterToolStrip.TabIndex = 1;
            // 
            // locationToolStripComboBox
            // 
            this.locationToolStripComboBox.AutoSize = false;
            this.locationToolStripComboBox.DropDownWidth = 40;
            this.locationToolStripComboBox.Items.AddRange(new object[] {
            "GU",
            "NV",
            "WA",
            "MS",
            "FL",
            "NC",
            "TX",
            "CA"});
            this.locationToolStripComboBox.Name = "locationToolStripComboBox";
            this.locationToolStripComboBox.Size = new System.Drawing.Size(40, 24);
            this.locationToolStripComboBox.Text = "GU";
            this.locationToolStripComboBox.ToolTipText = "Locations";
            this.locationToolStripComboBox.TextChanged += new System.EventHandler(this.locationToolStripComboBox_TextChanged);
            // 
            // algorithmToolStripComboBox
            // 
            this.algorithmToolStripComboBox.AutoSize = false;
            this.algorithmToolStripComboBox.Items.AddRange(new object[] {
            "EB",
            "PA",
            "NPI"});
            this.algorithmToolStripComboBox.Name = "algorithmToolStripComboBox";
            this.algorithmToolStripComboBox.Size = new System.Drawing.Size(43, 24);
            this.algorithmToolStripComboBox.Text = "EB";
            this.algorithmToolStripComboBox.ToolTipText = "algorithms";
            this.algorithmToolStripComboBox.TextChanged += new System.EventHandler(this.AlgorithmToolStripComboBoxTextChanged);
            // 
            // partitionToolStripComboBox
            // 
            this.partitionToolStripComboBox.AutoSize = false;
            this.partitionToolStripComboBox.Items.AddRange(new object[] {
            "1",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256"});
            this.partitionToolStripComboBox.Name = "partitionToolStripComboBox";
            this.partitionToolStripComboBox.Size = new System.Drawing.Size(43, 24);
            this.partitionToolStripComboBox.Text = "16";
            this.partitionToolStripComboBox.ToolTipText = "regions";
            // 
            // packetToolStripComboBox
            // 
            this.packetToolStripComboBox.AutoSize = false;
            this.packetToolStripComboBox.Items.AddRange(new object[] {
            "128",
            "256",
            "384",
            "512",
            "640",
            "768",
            "896",
            "1024"});
            this.packetToolStripComboBox.Name = "packetToolStripComboBox";
            this.packetToolStripComboBox.Size = new System.Drawing.Size(50, 24);
            this.packetToolStripComboBox.Text = "128";
            this.packetToolStripComboBox.ToolTipText = "packet size";
            // 
            // kToolStripComboBox
            // 
            this.kToolStripComboBox.AutoSize = false;
            this.kToolStripComboBox.Items.AddRange(new object[] {
            "1",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100",
            "150",
            "200",
            "250",
            "300",
            "1000"});
            this.kToolStripComboBox.Name = "kToolStripComboBox";
            this.kToolStripComboBox.Size = new System.Drawing.Size(43, 24);
            this.kToolStripComboBox.Text = "10";
            this.kToolStripComboBox.ToolTipText = "k";
            // 
            // actionToolStrip
            // 
            this.actionToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.actionToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nvdToolStripButton,
            this.partitionToolStripButton,
            this.toolStripSeparator1,
            this.quadTreeToolStripButton,
            this.tableToolStripButton,
            this.searchToolStripButton});
            this.actionToolStrip.Location = new System.Drawing.Point(320, 0);
            this.actionToolStrip.Name = "actionToolStrip";
            this.actionToolStrip.Size = new System.Drawing.Size(164, 25);
            this.actionToolStrip.TabIndex = 2;
            // 
            // nvdToolStripButton
            // 
            this.nvdToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nvdToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nvdToolStripButton.Image")));
            this.nvdToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nvdToolStripButton.Name = "nvdToolStripButton";
            this.nvdToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nvdToolStripButton.Text = "Generate NVD";
            this.nvdToolStripButton.Click += new System.EventHandler(this.ClickNvdToolStripButton);
            // 
            // partitionToolStripButton
            // 
            this.partitionToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.partitionToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("partitionToolStripButton.Image")));
            this.partitionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.partitionToolStripButton.Name = "partitionToolStripButton";
            this.partitionToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.partitionToolStripButton.Text = "Partition";
            this.partitionToolStripButton.Click += new System.EventHandler(this.ClickPartitionToolStripButton);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // quadTreeToolStripButton
            // 
            this.quadTreeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.quadTreeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("quadTreeToolStripButton.Image")));
            this.quadTreeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.quadTreeToolStripButton.Name = "quadTreeToolStripButton";
            this.quadTreeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.quadTreeToolStripButton.Text = "Quad Tree Index";
            this.quadTreeToolStripButton.ToolTipText = "Generate Index";
            this.quadTreeToolStripButton.Click += new System.EventHandler(this.ClickIndexToolStripButton);
            // 
            // tableToolStripButton
            // 
            this.tableToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tableToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("tableToolStripButton.Image")));
            this.tableToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tableToolStripButton.Name = "tableToolStripButton";
            this.tableToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.tableToolStripButton.Text = "EB Table";
            this.tableToolStripButton.ToolTipText = "Generate Table";
            this.tableToolStripButton.Click += new System.EventHandler(this.ClickTableToolStripButton);
            // 
            // searchToolStripButton
            // 
            this.searchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("searchToolStripButton.Image")));
            this.searchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchToolStripButton.Name = "searchToolStripButton";
            this.searchToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.searchToolStripButton.Text = "EB Search";
            this.searchToolStripButton.ToolTipText = "Search KNN";
            this.searchToolStripButton.Click += new System.EventHandler(this.ClickSearchToolStripButton);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.fileToolStrip.ResumeLayout(false);
            this.fileToolStrip.PerformLayout();
            this.parameterToolStrip.ResumeLayout(false);
            this.parameterToolStrip.PerformLayout();
            this.actionToolStrip.ResumeLayout(false);
            this.actionToolStrip.PerformLayout();
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
        private System.Windows.Forms.ToolStrip actionToolStrip;
        private System.Windows.Forms.ToolStripButton nvdToolStripButton;
        private System.Windows.Forms.ToolStripSplitButton saveFileToolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem saveNVDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNVDToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton partitionToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton quadTreeToolStripButton;
        private System.Windows.Forms.ToolStripButton tableToolStripButton;
        private System.Windows.Forms.ToolStripButton searchToolStripButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tuning;
        private System.Windows.Forms.ToolStripMenuItem saveEBTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addEBTableToolStripMenuItem;
        private System.Windows.Forms.ToolStrip parameterToolStrip;
        private System.Windows.Forms.ToolStripComboBox partitionToolStripComboBox;
        private System.Windows.Forms.ToolStripComboBox packetToolStripComboBox;
        private System.Windows.Forms.ToolStripComboBox kToolStripComboBox;
        private System.Windows.Forms.ToolStripComboBox algorithmToolStripComboBox;
        private System.Windows.Forms.ToolStripMenuItem saveNPITableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNPITableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRoadsPoIsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveRoadsAndPoIsToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox locationToolStripComboBox;
    }
}

