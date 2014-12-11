namespace FAT
{
    partial class FATMainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FATMainForm));
            this.inputFolderTextBox = new System.Windows.Forms.TextBox();
            this.inputFolderButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.calculateChecksumCb = new System.Windows.Forms.CheckBox();
            this.doPlausibilityCheckCb = new System.Windows.Forms.CheckBox();
            this.recheckCb = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.nThreadsComboBox = new System.Windows.Forms.ComboBox();
            this.jpg2000Cb = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.jpgCb = new System.Windows.Forms.CheckBox();
            this.tifCb = new System.Windows.Forms.CheckBox();
            this.stopOnFirstError = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.messageInfoLabel = new System.Windows.Forms.Label();
            this.directoryTreeView = new System.Windows.Forms.TreeView();
            this.iconList = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.reloadButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.errorLogTabPage = new System.Windows.Forms.TabPage();
            this.errorLogTextBox = new System.Windows.Forms.TextBox();
            this.metadataTabPage = new System.Windows.Forms.TabPage();
            this.editorGroupBox = new System.Windows.Forms.GroupBox();
            this.applyMetadataFromMasterlist = new System.Windows.Forms.Button();
            this.setMetadataAndPopulateButton = new System.Windows.Forms.Button();
            this.languageClb = new System.Windows.Forms.CheckedListBox();
            this.texttypeComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.setMetadataButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nodeInfoTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.expandIssue = new System.Windows.Forms.Button();
            this.backgroundWorkerLoadDirectory = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerLoadXML = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerSave = new System.ComponentModel.BackgroundWorker();
            this.fileStatusesLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.fileCheckStatusLabel = new System.Windows.Forms.Label();
            this.metadataStatusLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.hasYearCb = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.loadedDataLabel = new System.Windows.Forms.Label();
            this.masterListInfoLabel = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.viewingFilesStatusLabel = new System.Windows.Forms.Label();
            this.showMissingViewingFiles = new System.Windows.Forms.Button();
            this.reloadMasterListButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.minDpiUpDown = new System.Windows.Forms.NumericUpDown();
            this.maxDpiUpDown = new System.Windows.Forms.NumericUpDown();
            this.maxDpiBwUpDown = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.overwriteDpiUpDown = new System.Windows.Forms.NumericUpDown();
            this.checkForLengthCb = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.errorLogTabPage.SuspendLayout();
            this.metadataTabPage.SuspendLayout();
            this.editorGroupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minDpiUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDpiUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDpiBwUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overwriteDpiUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // inputFolderTextBox
            // 
            this.inputFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFolderTextBox.Location = new System.Drawing.Point(97, 15);
            this.inputFolderTextBox.Name = "inputFolderTextBox";
            this.inputFolderTextBox.Size = new System.Drawing.Size(676, 20);
            this.inputFolderTextBox.TabIndex = 0;
            // 
            // inputFolderButton
            // 
            this.inputFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFolderButton.AutoSize = true;
            this.inputFolderButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.inputFolderButton.Location = new System.Drawing.Point(878, 13);
            this.inputFolderButton.Name = "inputFolderButton";
            this.inputFolderButton.Size = new System.Drawing.Size(26, 23);
            this.inputFolderButton.TabIndex = 1;
            this.inputFolderButton.Text = "...";
            this.inputFolderButton.UseVisualStyleBackColor = true;
            this.inputFolderButton.Click += new System.EventHandler(this.inputFolderButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input Folder:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkForLengthCb);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.overwriteDpiUpDown);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.maxDpiBwUpDown);
            this.groupBox1.Controls.Add(this.maxDpiUpDown);
            this.groupBox1.Controls.Add(this.minDpiUpDown);
            this.groupBox1.Controls.Add(this.calculateChecksumCb);
            this.groupBox1.Controls.Add(this.doPlausibilityCheckCb);
            this.groupBox1.Controls.Add(this.recheckCb);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.nThreadsComboBox);
            this.groupBox1.Controls.Add(this.jpg2000Cb);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.jpgCb);
            this.groupBox1.Controls.Add(this.tifCb);
            this.groupBox1.Controls.Add(this.stopOnFirstError);
            this.groupBox1.Location = new System.Drawing.Point(15, 279);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(157, 353);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filecheck options";
            // 
            // calculateChecksumCb
            // 
            this.calculateChecksumCb.AutoSize = true;
            this.calculateChecksumCb.Checked = true;
            this.calculateChecksumCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.calculateChecksumCb.Location = new System.Drawing.Point(15, 89);
            this.calculateChecksumCb.Name = "calculateChecksumCb";
            this.calculateChecksumCb.Size = new System.Drawing.Size(123, 17);
            this.calculateChecksumCb.TabIndex = 29;
            this.calculateChecksumCb.Text = "Calculate Checksum";
            this.toolTip1.SetToolTip(this.calculateChecksumCb, "If checked, a checksum is calculated for each file. For large files this can be t" +
        "ime consuming!");
            this.calculateChecksumCb.UseVisualStyleBackColor = true;
            // 
            // doPlausibilityCheckCb
            // 
            this.doPlausibilityCheckCb.AutoSize = true;
            this.doPlausibilityCheckCb.Location = new System.Drawing.Point(15, 66);
            this.doPlausibilityCheckCb.Name = "doPlausibilityCheckCb";
            this.doPlausibilityCheckCb.Size = new System.Drawing.Size(123, 17);
            this.doPlausibilityCheckCb.TabIndex = 28;
            this.doPlausibilityCheckCb.Text = "Do plausibility check";
            this.toolTip1.SetToolTip(this.doPlausibilityCheckCb, "Determines if a plausibility check for files with a low resolution (<300) is perf" +
        "ormed and the resolution is overwritten to 300 dpi");
            this.doPlausibilityCheckCb.UseVisualStyleBackColor = true;
            // 
            // recheckCb
            // 
            this.recheckCb.AutoSize = true;
            this.recheckCb.Location = new System.Drawing.Point(15, 43);
            this.recheckCb.Name = "recheckCb";
            this.recheckCb.Size = new System.Drawing.Size(94, 17);
            this.recheckCb.TabIndex = 27;
            this.recheckCb.Text = "Re-check files";
            this.toolTip1.SetToolTip(this.recheckCb, "Re-checks files that have already been successfully checked");
            this.recheckCb.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "File formats:";
            // 
            // nThreadsComboBox
            // 
            this.nThreadsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nThreadsComboBox.FormattingEnabled = true;
            this.nThreadsComboBox.Location = new System.Drawing.Point(88, 323);
            this.nThreadsComboBox.Name = "nThreadsComboBox";
            this.nThreadsComboBox.Size = new System.Drawing.Size(57, 21);
            this.nThreadsComboBox.TabIndex = 26;
            this.toolTip1.SetToolTip(this.nThreadsComboBox, "Specifies the number of threads that are used for the filecheck ");
            this.nThreadsComboBox.Visible = false;
            // 
            // jpg2000Cb
            // 
            this.jpg2000Cb.AutoSize = true;
            this.jpg2000Cb.Checked = true;
            this.jpg2000Cb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.jpg2000Cb.Location = new System.Drawing.Point(15, 196);
            this.jpg2000Cb.Name = "jpg2000Cb";
            this.jpg2000Cb.Size = new System.Drawing.Size(80, 17);
            this.jpg2000Cb.TabIndex = 2;
            this.jpg2000Cb.Text = "JPEG 2000";
            this.jpg2000Cb.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 327);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "n-threads = ";
            this.label8.Visible = false;
            // 
            // jpgCb
            // 
            this.jpgCb.AutoSize = true;
            this.jpgCb.Checked = true;
            this.jpgCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.jpgCb.Location = new System.Drawing.Point(15, 173);
            this.jpgCb.Name = "jpgCb";
            this.jpgCb.Size = new System.Drawing.Size(53, 17);
            this.jpgCb.TabIndex = 1;
            this.jpgCb.Text = "JPEG";
            this.jpgCb.UseVisualStyleBackColor = true;
            // 
            // tifCb
            // 
            this.tifCb.AutoSize = true;
            this.tifCb.Checked = true;
            this.tifCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tifCb.Location = new System.Drawing.Point(15, 150);
            this.tifCb.Name = "tifCb";
            this.tifCb.Size = new System.Drawing.Size(42, 17);
            this.tifCb.TabIndex = 0;
            this.tifCb.Text = "TIF";
            this.tifCb.UseVisualStyleBackColor = true;
            // 
            // stopOnFirstError
            // 
            this.stopOnFirstError.AutoSize = true;
            this.stopOnFirstError.Location = new System.Drawing.Point(15, 19);
            this.stopOnFirstError.Name = "stopOnFirstError";
            this.stopOnFirstError.Size = new System.Drawing.Size(106, 17);
            this.stopOnFirstError.TabIndex = 11;
            this.stopOnFirstError.Text = "Stop on first error";
            this.toolTip1.SetToolTip(this.stopOnFirstError, "Stops the filecheck process after the first error occurs");
            this.stopOnFirstError.UseVisualStyleBackColor = true;
            // 
            // startButton
            // 
            this.startButton.AutoSize = true;
            this.startButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.startButton.Location = new System.Drawing.Point(15, 73);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(88, 23);
            this.startButton.TabIndex = 6;
            this.startButton.Text = "Start file check";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // stopButton
            // 
            this.stopButton.AutoSize = true;
            this.stopButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stopButton.Location = new System.Drawing.Point(114, 73);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(39, 23);
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.StopButtonClick);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(117, 42);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(689, 23);
            this.progressBar.TabIndex = 8;
            // 
            // messageInfoLabel
            // 
            this.messageInfoLabel.AutoSize = true;
            this.messageInfoLabel.Location = new System.Drawing.Point(234, 73);
            this.messageInfoLabel.Name = "messageInfoLabel";
            this.messageInfoLabel.Size = new System.Drawing.Size(50, 13);
            this.messageInfoLabel.TabIndex = 9;
            this.messageInfoLabel.Text = "infoLabel";
            // 
            // directoryTreeView
            // 
            this.directoryTreeView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.directoryTreeView.HideSelection = false;
            this.directoryTreeView.ImageIndex = 0;
            this.directoryTreeView.ImageList = this.iconList;
            this.directoryTreeView.Location = new System.Drawing.Point(0, 29);
            this.directoryTreeView.Name = "directoryTreeView";
            this.directoryTreeView.SelectedImageIndex = 0;
            this.directoryTreeView.Size = new System.Drawing.Size(377, 432);
            this.directoryTreeView.TabIndex = 10;
            // 
            // iconList
            // 
            this.iconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconList.ImageStream")));
            this.iconList.TransparentColor = System.Drawing.Color.Transparent;
            this.iconList.Images.SetKeyName(0, "folder.png");
            this.iconList.Images.SetKeyName(1, "llibrary.png");
            this.iconList.Images.SetKeyName(2, "newspaper.png");
            this.iconList.Images.SetKeyName(3, "root_hdd.png");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(169, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Message:";
            // 
            // reloadButton
            // 
            this.reloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.reloadButton.AutoSize = true;
            this.reloadButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.reloadButton.Location = new System.Drawing.Point(910, 13);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(94, 23);
            this.reloadButton.TabIndex = 14;
            this.reloadButton.Text = "Reload directory";
            this.toolTip1.SetToolTip(this.reloadButton, "Reloads the current directory specified in the input folder text area");
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.errorLogTabPage);
            this.tabControl1.Controls.Add(this.metadataTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(445, 461);
            this.tabControl1.TabIndex = 17;
            // 
            // errorLogTabPage
            // 
            this.errorLogTabPage.Controls.Add(this.errorLogTextBox);
            this.errorLogTabPage.Location = new System.Drawing.Point(4, 22);
            this.errorLogTabPage.Name = "errorLogTabPage";
            this.errorLogTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.errorLogTabPage.Size = new System.Drawing.Size(437, 435);
            this.errorLogTabPage.TabIndex = 1;
            this.errorLogTabPage.Text = "Error Log";
            this.errorLogTabPage.UseVisualStyleBackColor = true;
            // 
            // errorLogTextBox
            // 
            this.errorLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorLogTextBox.Location = new System.Drawing.Point(3, 3);
            this.errorLogTextBox.Multiline = true;
            this.errorLogTextBox.Name = "errorLogTextBox";
            this.errorLogTextBox.ReadOnly = true;
            this.errorLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorLogTextBox.Size = new System.Drawing.Size(431, 429);
            this.errorLogTextBox.TabIndex = 0;
            // 
            // metadataTabPage
            // 
            this.metadataTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.metadataTabPage.Controls.Add(this.editorGroupBox);
            this.metadataTabPage.Controls.Add(this.groupBox2);
            this.metadataTabPage.Location = new System.Drawing.Point(4, 22);
            this.metadataTabPage.Name = "metadataTabPage";
            this.metadataTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.metadataTabPage.Size = new System.Drawing.Size(437, 435);
            this.metadataTabPage.TabIndex = 2;
            this.metadataTabPage.Text = "Metadata Editor";
            // 
            // editorGroupBox
            // 
            this.editorGroupBox.Controls.Add(this.applyMetadataFromMasterlist);
            this.editorGroupBox.Controls.Add(this.setMetadataAndPopulateButton);
            this.editorGroupBox.Controls.Add(this.languageClb);
            this.editorGroupBox.Controls.Add(this.texttypeComboBox);
            this.editorGroupBox.Controls.Add(this.label5);
            this.editorGroupBox.Controls.Add(this.label4);
            this.editorGroupBox.Controls.Add(this.setMetadataButton);
            this.editorGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.editorGroupBox.Location = new System.Drawing.Point(3, 175);
            this.editorGroupBox.Name = "editorGroupBox";
            this.editorGroupBox.Size = new System.Drawing.Size(431, 257);
            this.editorGroupBox.TabIndex = 4;
            this.editorGroupBox.TabStop = false;
            this.editorGroupBox.Text = "Editor";
            // 
            // applyMetadataFromMasterlist
            // 
            this.applyMetadataFromMasterlist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.applyMetadataFromMasterlist.AutoSize = true;
            this.applyMetadataFromMasterlist.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.applyMetadataFromMasterlist.Location = new System.Drawing.Point(11, 228);
            this.applyMetadataFromMasterlist.Name = "applyMetadataFromMasterlist";
            this.applyMetadataFromMasterlist.Size = new System.Drawing.Size(159, 23);
            this.applyMetadataFromMasterlist.TabIndex = 8;
            this.applyMetadataFromMasterlist.Text = "Apply metadata from masterlist";
            this.applyMetadataFromMasterlist.UseVisualStyleBackColor = true;
            this.applyMetadataFromMasterlist.Click += new System.EventHandler(this.applyMetadataFromMasterlist_Click);
            // 
            // setMetadataAndPopulateButton
            // 
            this.setMetadataAndPopulateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.setMetadataAndPopulateButton.AutoSize = true;
            this.setMetadataAndPopulateButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.setMetadataAndPopulateButton.Location = new System.Drawing.Point(99, 203);
            this.setMetadataAndPopulateButton.Name = "setMetadataAndPopulateButton";
            this.setMetadataAndPopulateButton.Size = new System.Drawing.Size(174, 23);
            this.setMetadataAndPopulateButton.TabIndex = 7;
            this.setMetadataAndPopulateButton.Text = "Set metadata and populate down";
            this.toolTip1.SetToolTip(this.setMetadataAndPopulateButton, "Sets the metadata for the selected folder and applies the information to all entr" +
        "ies below on this hierarchy level");
            this.setMetadataAndPopulateButton.UseVisualStyleBackColor = true;
            this.setMetadataAndPopulateButton.Click += new System.EventHandler(this.setMetadataAndPopulateButton_Click);
            // 
            // languageClb
            // 
            this.languageClb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.languageClb.FormattingEnabled = true;
            this.languageClb.Items.AddRange(new object[] {
            "Arabic",
            "Czech",
            "Dutch",
            "English",
            "Estonian",
            "Finnish",
            "French",
            "German",
            "Italian",
            "Latvian",
            "Polish",
            "Russian",
            "SerbianCyrillic",
            "SerbianLatin",
            "Turkish",
            "OldGerman",
            "OldEnglish",
            "OldFrench",
            "LatvianGothic",
            "RussianOldSpelling",
            "Ukrainian",
            "Yiddish",
            "Swedish",
            "Latin",
            "OldItalian"});
            this.languageClb.Location = new System.Drawing.Point(73, 16);
            this.languageClb.Name = "languageClb";
            this.languageClb.Size = new System.Drawing.Size(355, 154);
            this.languageClb.TabIndex = 5;
            // 
            // texttypeComboBox
            // 
            this.texttypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.texttypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.texttypeComboBox.FormattingEnabled = true;
            this.texttypeComboBox.Items.AddRange(new object[] {
            "Normal",
            "Gothic",
            "Normal,Gothic"});
            this.texttypeComboBox.Location = new System.Drawing.Point(73, 176);
            this.texttypeComboBox.Name = "texttypeComboBox";
            this.texttypeComboBox.Size = new System.Drawing.Size(355, 21);
            this.texttypeComboBox.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Texttype:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Language:";
            // 
            // setMetadataButton
            // 
            this.setMetadataButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.setMetadataButton.AutoSize = true;
            this.setMetadataButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.setMetadataButton.Location = new System.Drawing.Point(11, 203);
            this.setMetadataButton.Name = "setMetadataButton";
            this.setMetadataButton.Size = new System.Drawing.Size(81, 23);
            this.setMetadataButton.TabIndex = 1;
            this.setMetadataButton.Text = "Set Metadata";
            this.toolTip1.SetToolTip(this.setMetadataButton, "Sets the metadata for the selected folder - if a folder on a higher hierarchy lev" +
        "el is seleted, the metadata will be applied recursively to the subdirectories");
            this.setMetadataButton.UseVisualStyleBackColor = true;
            this.setMetadataButton.Click += new System.EventHandler(this.okSetMetadataButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.nodeInfoTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(431, 163);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Info";
            // 
            // nodeInfoTextBox
            // 
            this.nodeInfoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeInfoTextBox.Location = new System.Drawing.Point(3, 16);
            this.nodeInfoTextBox.Multiline = true;
            this.nodeInfoTextBox.Name = "nodeInfoTextBox";
            this.nodeInfoTextBox.ReadOnly = true;
            this.nodeInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.nodeInfoTextBox.Size = new System.Drawing.Size(425, 144);
            this.nodeInfoTextBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 18;
            this.label3.Text = "Loaded data:";
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadButton.AutoSize = true;
            this.loadButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loadButton.Location = new System.Drawing.Point(913, 42);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(91, 23);
            this.loadButton.TabIndex = 19;
            this.loadButton.Text = "Load XML file...";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Visible = false;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.AutoSize = true;
            this.saveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveButton.Location = new System.Drawing.Point(812, 42);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(92, 23);
            this.saveButton.TabIndex = 20;
            this.saveButton.Text = "Save XML file...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // expandIssue
            // 
            this.expandIssue.Location = new System.Drawing.Point(129, 3);
            this.expandIssue.Name = "expandIssue";
            this.expandIssue.Size = new System.Drawing.Size(82, 20);
            this.expandIssue.TabIndex = 21;
            this.expandIssue.Text = "Expand";
            this.expandIssue.UseVisualStyleBackColor = true;
            this.expandIssue.Click += new System.EventHandler(this.expandIssue_Click);
            // 
            // backgroundWorkerLoadDirectory
            // 
            this.backgroundWorkerLoadDirectory.WorkerSupportsCancellation = true;
            // 
            // fileStatusesLabel
            // 
            this.fileStatusesLabel.AutoSize = true;
            this.fileStatusesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileStatusesLabel.Location = new System.Drawing.Point(127, 167);
            this.fileStatusesLabel.Name = "fileStatusesLabel";
            this.fileStatusesLabel.Size = new System.Drawing.Size(45, 16);
            this.fileStatusesLabel.TabIndex = 27;
            this.fileStatusesLabel.Text = "label7";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(21, 167);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 16);
            this.label9.TabIndex = 28;
            this.label9.Text = "File statuses:";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(234, 86);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(50, 13);
            this.timeLabel.TabIndex = 30;
            this.timeLabel.Text = "infoLabel";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(21, 193);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(128, 16);
            this.label10.TabIndex = 31;
            this.label10.Text = "File check status:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(21, 218);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(122, 16);
            this.label11.TabIndex = 32;
            this.label11.Text = "Metadata status:";
            // 
            // fileCheckStatusLabel
            // 
            this.fileCheckStatusLabel.AutoSize = true;
            this.fileCheckStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileCheckStatusLabel.Location = new System.Drawing.Point(169, 193);
            this.fileCheckStatusLabel.Name = "fileCheckStatusLabel";
            this.fileCheckStatusLabel.Size = new System.Drawing.Size(51, 16);
            this.fileCheckStatusLabel.TabIndex = 33;
            this.fileCheckStatusLabel.Text = "label7";
            // 
            // metadataStatusLabel
            // 
            this.metadataStatusLabel.AutoSize = true;
            this.metadataStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.metadataStatusLabel.Location = new System.Drawing.Point(169, 218);
            this.metadataStatusLabel.Name = "metadataStatusLabel";
            this.metadataStatusLabel.Size = new System.Drawing.Size(51, 16);
            this.metadataStatusLabel.TabIndex = 34;
            this.metadataStatusLabel.Text = "label7";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(3, 5);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 16);
            this.label12.TabIndex = 35;
            this.label12.Text = "Folder structure:";
            // 
            // hasYearCb
            // 
            this.hasYearCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hasYearCb.AutoSize = true;
            this.hasYearCb.Checked = true;
            this.hasYearCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasYearCb.Location = new System.Drawing.Point(779, 17);
            this.hasYearCb.Name = "hasYearCb";
            this.hasYearCb.Size = new System.Drawing.Size(93, 17);
            this.hasYearCb.TabIndex = 37;
            this.hasYearCb.Text = "Has year level";
            this.toolTip1.SetToolTip(this.hasYearCb, "Uncheck if there are no year level folders in your folder structure");
            this.hasYearCb.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 13);
            this.label13.TabIndex = 36;
            this.label13.Text = "Filecheck progress:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(21, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(212, 16);
            this.label6.TabIndex = 38;
            this.label6.Text = "Last edit of loaded masterlist:";
            // 
            // loadedDataLabel
            // 
            this.loadedDataLabel.AutoSize = true;
            this.loadedDataLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadedDataLabel.Location = new System.Drawing.Point(127, 142);
            this.loadedDataLabel.Name = "loadedDataLabel";
            this.loadedDataLabel.Size = new System.Drawing.Size(45, 16);
            this.loadedDataLabel.TabIndex = 23;
            this.loadedDataLabel.Text = "label7";
            // 
            // masterListInfoLabel
            // 
            this.masterListInfoLabel.AutoSize = true;
            this.masterListInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.masterListInfoLabel.Location = new System.Drawing.Point(239, 116);
            this.masterListInfoLabel.Name = "masterListInfoLabel";
            this.masterListInfoLabel.Size = new System.Drawing.Size(45, 16);
            this.masterListInfoLabel.TabIndex = 39;
            this.masterListInfoLabel.Text = "label7";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(21, 244);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(144, 16);
            this.label14.TabIndex = 40;
            this.label14.Text = "Viewing files status:";
            // 
            // viewingFilesStatusLabel
            // 
            this.viewingFilesStatusLabel.AutoSize = true;
            this.viewingFilesStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewingFilesStatusLabel.Location = new System.Drawing.Point(169, 244);
            this.viewingFilesStatusLabel.Name = "viewingFilesStatusLabel";
            this.viewingFilesStatusLabel.Size = new System.Drawing.Size(51, 16);
            this.viewingFilesStatusLabel.TabIndex = 41;
            this.viewingFilesStatusLabel.Text = "label7";
            // 
            // showMissingViewingFiles
            // 
            this.showMissingViewingFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.showMissingViewingFiles.AutoSize = true;
            this.showMissingViewingFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.showMissingViewingFiles.Location = new System.Drawing.Point(520, 236);
            this.showMissingViewingFiles.Name = "showMissingViewingFiles";
            this.showMissingViewingFiles.Size = new System.Drawing.Size(193, 23);
            this.showMissingViewingFiles.TabIndex = 42;
            this.showMissingViewingFiles.Text = "Show missing viewing files in error log";
            this.showMissingViewingFiles.UseVisualStyleBackColor = true;
            this.showMissingViewingFiles.Click += new System.EventHandler(this.showMissingViewingFiles_Click);
            // 
            // reloadMasterListButton
            // 
            this.reloadMasterListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reloadMasterListButton.AutoSize = true;
            this.reloadMasterListButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.reloadMasterListButton.Location = new System.Drawing.Point(326, 113);
            this.reloadMasterListButton.Name = "reloadMasterListButton";
            this.reloadMasterListButton.Size = new System.Drawing.Size(98, 23);
            this.reloadMasterListButton.TabIndex = 43;
            this.reloadMasterListButton.Text = "Reload Masterlist";
            this.reloadMasterListButton.UseVisualStyleBackColor = true;
            this.reloadMasterListButton.Visible = false;
            this.reloadMasterListButton.Click += new System.EventHandler(this.reloadMasterListButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(178, 279);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.directoryTreeView);
            this.splitContainer1.Panel1.Controls.Add(this.label12);
            this.splitContainer1.Panel1.Controls.Add(this.expandIssue);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(826, 461);
            this.splitContainer1.SplitterDistance = 377;
            this.splitContainer1.TabIndex = 44;
            // 
            // minDpiUpDown
            // 
            this.minDpiUpDown.Location = new System.Drawing.Point(91, 248);
            this.minDpiUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.minDpiUpDown.Name = "minDpiUpDown";
            this.minDpiUpDown.Size = new System.Drawing.Size(57, 20);
            this.minDpiUpDown.TabIndex = 30;
            this.minDpiUpDown.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // maxDpiUpDown
            // 
            this.maxDpiUpDown.Location = new System.Drawing.Point(91, 274);
            this.maxDpiUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.maxDpiUpDown.Name = "maxDpiUpDown";
            this.maxDpiUpDown.Size = new System.Drawing.Size(57, 20);
            this.maxDpiUpDown.TabIndex = 31;
            this.maxDpiUpDown.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            // 
            // maxDpiBwUpDown
            // 
            this.maxDpiBwUpDown.Location = new System.Drawing.Point(91, 300);
            this.maxDpiBwUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.maxDpiBwUpDown.Name = "maxDpiBwUpDown";
            this.maxDpiBwUpDown.Size = new System.Drawing.Size(57, 20);
            this.maxDpiBwUpDown.TabIndex = 32;
            this.maxDpiBwUpDown.Value = new decimal(new int[] {
            610,
            0,
            0,
            0});
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 250);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 33;
            this.label15.Text = "Min Dpi";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 276);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(46, 13);
            this.label16.TabIndex = 34;
            this.label16.Text = "Max Dpi";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 302);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 35;
            this.label17.Text = "Max Dpi (Bw)";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(12, 225);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(71, 13);
            this.label18.TabIndex = 37;
            this.label18.Text = "Overwrite Dpi";
            // 
            // overwriteDpiUpDown
            // 
            this.overwriteDpiUpDown.Location = new System.Drawing.Point(90, 223);
            this.overwriteDpiUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.overwriteDpiUpDown.Name = "overwriteDpiUpDown";
            this.overwriteDpiUpDown.Size = new System.Drawing.Size(57, 20);
            this.overwriteDpiUpDown.TabIndex = 36;
            this.overwriteDpiUpDown.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // checkForLengthCb
            // 
            this.checkForLengthCb.AutoSize = true;
            this.checkForLengthCb.Checked = true;
            this.checkForLengthCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkForLengthCb.Location = new System.Drawing.Point(15, 112);
            this.checkForLengthCb.Name = "checkForLengthCb";
            this.checkForLengthCb.Size = new System.Drawing.Size(131, 17);
            this.checkForLengthCb.TabIndex = 38;
            this.checkForLengthCb.Text = "Check filename length";
            this.toolTip1.SetToolTip(this.checkForLengthCb, "Checks if all filenames in an issue have equal length");
            this.checkForLengthCb.UseVisualStyleBackColor = true;
            // 
            // FATMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 741);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.reloadMasterListButton);
            this.Controls.Add(this.showMissingViewingFiles);
            this.Controls.Add(this.viewingFilesStatusLabel);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.masterListInfoLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.hasYearCb);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.metadataStatusLabel);
            this.Controls.Add(this.fileCheckStatusLabel);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.fileStatusesLabel);
            this.Controls.Add(this.loadedDataLabel);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.messageInfoLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputFolderButton);
            this.Controls.Add(this.inputFolderTextBox);
            this.Name = "FATMainForm";
            this.Text = "FAT - File Analyzer Tool";
            this.Load += new System.EventHandler(this.FATMainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.errorLogTabPage.ResumeLayout(false);
            this.errorLogTabPage.PerformLayout();
            this.metadataTabPage.ResumeLayout(false);
            this.editorGroupBox.ResumeLayout(false);
            this.editorGroupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.minDpiUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDpiUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDpiBwUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overwriteDpiUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputFolderTextBox;
        private System.Windows.Forms.Button inputFolderButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox jpg2000Cb;
        private System.Windows.Forms.CheckBox jpgCb;
        private System.Windows.Forms.CheckBox tifCb;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label messageInfoLabel;
        private System.Windows.Forms.TreeView directoryTreeView;
        private System.Windows.Forms.CheckBox stopOnFirstError;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage errorLogTabPage;
        private System.Windows.Forms.TextBox errorLogTextBox;
        private System.Windows.Forms.TabPage metadataTabPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox nodeInfoTextBox;
        private System.Windows.Forms.GroupBox editorGroupBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button setMetadataButton;
        private System.Windows.Forms.ComboBox texttypeComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button expandIssue;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoadDirectory;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoadXML;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSave;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox nThreadsComboBox;
        private System.Windows.Forms.Label fileStatusesLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckedListBox languageClb;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.CheckBox recheckCb;
        private System.Windows.Forms.Button setMetadataAndPopulateButton;
        private System.Windows.Forms.ImageList iconList;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label fileCheckStatusLabel;
        private System.Windows.Forms.Label metadataStatusLabel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox hasYearCb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label loadedDataLabel;
        private System.Windows.Forms.Label masterListInfoLabel;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label viewingFilesStatusLabel;
        private System.Windows.Forms.Button showMissingViewingFiles;
        private System.Windows.Forms.Button reloadMasterListButton;
        private System.Windows.Forms.Button applyMetadataFromMasterlist;
        private System.Windows.Forms.CheckBox doPlausibilityCheckCb;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox calculateChecksumCb;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.NumericUpDown maxDpiBwUpDown;
        public System.Windows.Forms.NumericUpDown maxDpiUpDown;
        public System.Windows.Forms.NumericUpDown minDpiUpDown;
        private System.Windows.Forms.Label label18;
        public System.Windows.Forms.NumericUpDown overwriteDpiUpDown;
        public System.Windows.Forms.CheckBox checkForLengthCb;
    }
}

