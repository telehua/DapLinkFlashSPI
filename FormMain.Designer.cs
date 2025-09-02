namespace DapLinkFlashSPI
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            buttonErase = new Button();
            buttonCheckEmpty = new Button();
            buttonProgram = new Button();
            buttonMatch = new Button();
            buttonAuto = new Button();
            comboBoxDeviceSelect = new ComboBox();
            comboBoxTargetNameSelect = new ComboBox();
            textBoxTargetManufacturer = new TextBox();
            buttonDump = new Button();
            menuStripTitle = new MenuStrip();
            ToolStripMenuItemFile = new ToolStripMenuItem();
            ToolStripMenuItemFileOpen = new ToolStripMenuItem();
            ToolStripMenuItemFileSave = new ToolStripMenuItem();
            ToolStripMenuItemHelp = new ToolStripMenuItem();
            ToolStripMenuItemHelpHelp = new ToolStripMenuItem();
            ToolStripMenuItemHelpAbout = new ToolStripMenuItem();
            groupBoxOperation = new GroupBox();
            groupDeviceSelect = new GroupBox();
            labelTextSpeed = new Label();
            comboBoxDeviceSpeed = new ComboBox();
            buttonDeviceFind = new Button();
            groupBoxTargetSelect = new GroupBox();
            labelTextTargetManufacturer = new Label();
            labelTextTargetName = new Label();
            toolTipMain = new ToolTip(components);
            statusStripMain = new StatusStrip();
            toolStripProgressBarMain = new ToolStripProgressBar();
            toolStripStatusLabelMain = new ToolStripStatusLabel();
            groupBoxLogView = new GroupBox();
            textBoxLogView = new TextBox();
            menuStripTitle.SuspendLayout();
            groupBoxOperation.SuspendLayout();
            groupDeviceSelect.SuspendLayout();
            groupBoxTargetSelect.SuspendLayout();
            statusStripMain.SuspendLayout();
            groupBoxLogView.SuspendLayout();
            SuspendLayout();
            // 
            // buttonErase
            // 
            buttonErase.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            buttonErase.Image = (Image)resources.GetObject("buttonErase.Image");
            buttonErase.ImageAlign = ContentAlignment.MiddleLeft;
            buttonErase.Location = new Point(10, 75);
            buttonErase.Margin = new Padding(5);
            buttonErase.Name = "buttonErase";
            buttonErase.Size = new Size(126, 40);
            buttonErase.TabIndex = 2;
            buttonErase.Text = "擦除";
            buttonErase.TextAlign = ContentAlignment.MiddleRight;
            toolTipMain.SetToolTip(buttonErase, "擦除所有数据");
            buttonErase.UseVisualStyleBackColor = true;
            buttonErase.Click += buttonErase_Click;
            // 
            // buttonCheckEmpty
            // 
            buttonCheckEmpty.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            buttonCheckEmpty.Image = (Image)resources.GetObject("buttonCheckEmpty.Image");
            buttonCheckEmpty.ImageAlign = ContentAlignment.MiddleLeft;
            buttonCheckEmpty.Location = new Point(10, 125);
            buttonCheckEmpty.Margin = new Padding(5);
            buttonCheckEmpty.Name = "buttonCheckEmpty";
            buttonCheckEmpty.Size = new Size(126, 40);
            buttonCheckEmpty.TabIndex = 3;
            buttonCheckEmpty.Text = "查空";
            buttonCheckEmpty.TextAlign = ContentAlignment.MiddleRight;
            toolTipMain.SetToolTip(buttonCheckEmpty, "检查擦除是否成功");
            buttonCheckEmpty.UseVisualStyleBackColor = true;
            buttonCheckEmpty.Click += buttonCheckEmpty_Click;
            // 
            // buttonProgram
            // 
            buttonProgram.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            buttonProgram.Image = (Image)resources.GetObject("buttonProgram.Image");
            buttonProgram.ImageAlign = ContentAlignment.MiddleLeft;
            buttonProgram.Location = new Point(10, 175);
            buttonProgram.Margin = new Padding(5);
            buttonProgram.Name = "buttonProgram";
            buttonProgram.Size = new Size(126, 40);
            buttonProgram.TabIndex = 4;
            buttonProgram.Text = "写入";
            buttonProgram.TextAlign = ContentAlignment.MiddleRight;
            toolTipMain.SetToolTip(buttonProgram, "写入数据");
            buttonProgram.UseVisualStyleBackColor = true;
            buttonProgram.Click += buttonProgram_Click;
            // 
            // buttonMatch
            // 
            buttonMatch.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            buttonMatch.Image = (Image)resources.GetObject("buttonMatch.Image");
            buttonMatch.ImageAlign = ContentAlignment.MiddleLeft;
            buttonMatch.Location = new Point(10, 225);
            buttonMatch.Margin = new Padding(5);
            buttonMatch.Name = "buttonMatch";
            buttonMatch.Size = new Size(126, 40);
            buttonMatch.TabIndex = 5;
            buttonMatch.Text = "校验";
            buttonMatch.TextAlign = ContentAlignment.MiddleRight;
            toolTipMain.SetToolTip(buttonMatch, "检查写入的数据是否正确");
            buttonMatch.UseVisualStyleBackColor = true;
            buttonMatch.Click += buttonMatch_Click;
            // 
            // buttonAuto
            // 
            buttonAuto.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            buttonAuto.Image = (Image)resources.GetObject("buttonAuto.Image");
            buttonAuto.ImageAlign = ContentAlignment.MiddleLeft;
            buttonAuto.Location = new Point(10, 275);
            buttonAuto.Margin = new Padding(5);
            buttonAuto.Name = "buttonAuto";
            buttonAuto.Size = new Size(126, 40);
            buttonAuto.TabIndex = 6;
            buttonAuto.Text = "自动";
            buttonAuto.TextAlign = ContentAlignment.MiddleRight;
            toolTipMain.SetToolTip(buttonAuto, "自动进行擦除、查空、写入、校验");
            buttonAuto.UseVisualStyleBackColor = true;
            // 
            // comboBoxDeviceSelect
            // 
            comboBoxDeviceSelect.DropDownWidth = 500;
            comboBoxDeviceSelect.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBoxDeviceSelect.FormattingEnabled = true;
            comboBoxDeviceSelect.ItemHeight = 21;
            comboBoxDeviceSelect.Location = new Point(10, 25);
            comboBoxDeviceSelect.Margin = new Padding(5);
            comboBoxDeviceSelect.Name = "comboBoxDeviceSelect";
            comboBoxDeviceSelect.Size = new Size(365, 29);
            comboBoxDeviceSelect.TabIndex = 8;
            // 
            // comboBoxTargetNameSelect
            // 
            comboBoxTargetNameSelect.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBoxTargetNameSelect.FormattingEnabled = true;
            comboBoxTargetNameSelect.IntegralHeight = false;
            comboBoxTargetNameSelect.ItemHeight = 27;
            comboBoxTargetNameSelect.Items.AddRange(new object[] { "Default" });
            comboBoxTargetNameSelect.Location = new Point(10, 55);
            comboBoxTargetNameSelect.Margin = new Padding(5);
            comboBoxTargetNameSelect.Name = "comboBoxTargetNameSelect";
            comboBoxTargetNameSelect.Size = new Size(194, 35);
            comboBoxTargetNameSelect.TabIndex = 12;
            comboBoxTargetNameSelect.SelectedIndexChanged += ComboBoxTargetNameSelect_SelectedIndexChanged;
            // 
            // textBoxTargetManufacturer
            // 
            textBoxTargetManufacturer.BackColor = SystemColors.Window;
            textBoxTargetManufacturer.Font = new Font("Microsoft YaHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBoxTargetManufacturer.Location = new Point(10, 135);
            textBoxTargetManufacturer.Margin = new Padding(5);
            textBoxTargetManufacturer.Name = "textBoxTargetManufacturer";
            textBoxTargetManufacturer.ReadOnly = true;
            textBoxTargetManufacturer.Size = new Size(194, 32);
            textBoxTargetManufacturer.TabIndex = 14;
            textBoxTargetManufacturer.Text = "Unknow";
            // 
            // buttonDump
            // 
            buttonDump.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            buttonDump.Image = (Image)resources.GetObject("buttonDump.Image");
            buttonDump.ImageAlign = ContentAlignment.MiddleLeft;
            buttonDump.Location = new Point(10, 25);
            buttonDump.Margin = new Padding(5);
            buttonDump.Name = "buttonDump";
            buttonDump.Size = new Size(126, 40);
            buttonDump.TabIndex = 1;
            buttonDump.Text = "读取";
            buttonDump.TextAlign = ContentAlignment.MiddleRight;
            toolTipMain.SetToolTip(buttonDump, "读取芯片数据");
            buttonDump.UseVisualStyleBackColor = true;
            buttonDump.Click += ButtonDump_Click;
            // 
            // menuStripTitle
            // 
            menuStripTitle.AutoSize = false;
            menuStripTitle.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            menuStripTitle.GripMargin = new Padding(0);
            menuStripTitle.Items.AddRange(new ToolStripItem[] { ToolStripMenuItemFile, ToolStripMenuItemHelp });
            menuStripTitle.Location = new Point(0, 0);
            menuStripTitle.Name = "menuStripTitle";
            menuStripTitle.Padding = new Padding(0);
            menuStripTitle.Size = new Size(984, 25);
            menuStripTitle.TabIndex = 21;
            menuStripTitle.Text = "menuStripTitle";
            // 
            // ToolStripMenuItemFile
            // 
            ToolStripMenuItemFile.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemFileOpen, ToolStripMenuItemFileSave });
            ToolStripMenuItemFile.Name = "ToolStripMenuItemFile";
            ToolStripMenuItemFile.Size = new Size(49, 25);
            ToolStripMenuItemFile.Text = "文件";
            // 
            // ToolStripMenuItemFileOpen
            // 
            ToolStripMenuItemFileOpen.Name = "ToolStripMenuItemFileOpen";
            ToolStripMenuItemFileOpen.Size = new Size(106, 24);
            ToolStripMenuItemFileOpen.Text = "打开";
            ToolStripMenuItemFileOpen.Click += ToolStripMenuItemFileOpen_Click;
            // 
            // ToolStripMenuItemFileSave
            // 
            ToolStripMenuItemFileSave.Name = "ToolStripMenuItemFileSave";
            ToolStripMenuItemFileSave.Size = new Size(106, 24);
            ToolStripMenuItemFileSave.Text = "保存";
            ToolStripMenuItemFileSave.Click += ToolStripMenuItemFileSave_Click;
            // 
            // ToolStripMenuItemHelp
            // 
            ToolStripMenuItemHelp.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemHelpHelp, ToolStripMenuItemHelpAbout });
            ToolStripMenuItemHelp.Name = "ToolStripMenuItemHelp";
            ToolStripMenuItemHelp.Size = new Size(49, 25);
            ToolStripMenuItemHelp.Text = "帮助";
            // 
            // ToolStripMenuItemHelpHelp
            // 
            ToolStripMenuItemHelpHelp.Name = "ToolStripMenuItemHelpHelp";
            ToolStripMenuItemHelpHelp.Size = new Size(106, 24);
            ToolStripMenuItemHelpHelp.Text = "帮助";
            // 
            // ToolStripMenuItemHelpAbout
            // 
            ToolStripMenuItemHelpAbout.Name = "ToolStripMenuItemHelpAbout";
            ToolStripMenuItemHelpAbout.Size = new Size(106, 24);
            ToolStripMenuItemHelpAbout.Text = "关于";
            ToolStripMenuItemHelpAbout.Click += ToolStripMenuItemHelpAbout_Click;
            // 
            // groupBoxOperation
            // 
            groupBoxOperation.Controls.Add(buttonDump);
            groupBoxOperation.Controls.Add(buttonErase);
            groupBoxOperation.Controls.Add(buttonCheckEmpty);
            groupBoxOperation.Controls.Add(buttonMatch);
            groupBoxOperation.Controls.Add(buttonProgram);
            groupBoxOperation.Controls.Add(buttonAuto);
            groupBoxOperation.Location = new Point(10, 30);
            groupBoxOperation.Margin = new Padding(5);
            groupBoxOperation.Name = "groupBoxOperation";
            groupBoxOperation.Size = new Size(150, 330);
            groupBoxOperation.TabIndex = 0;
            groupBoxOperation.TabStop = false;
            groupBoxOperation.Text = "操作";
            // 
            // groupDeviceSelect
            // 
            groupDeviceSelect.Controls.Add(labelTextSpeed);
            groupDeviceSelect.Controls.Add(comboBoxDeviceSpeed);
            groupDeviceSelect.Controls.Add(comboBoxDeviceSelect);
            groupDeviceSelect.Controls.Add(buttonDeviceFind);
            groupDeviceSelect.Location = new Point(10, 365);
            groupDeviceSelect.Margin = new Padding(5);
            groupDeviceSelect.Name = "groupDeviceSelect";
            groupDeviceSelect.Size = new Size(390, 150);
            groupDeviceSelect.TabIndex = 7;
            groupDeviceSelect.TabStop = false;
            groupDeviceSelect.Text = "设备";
            // 
            // labelTextSpeed
            // 
            labelTextSpeed.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 134);
            labelTextSpeed.ImageAlign = ContentAlignment.TopLeft;
            labelTextSpeed.Location = new Point(10, 70);
            labelTextSpeed.Name = "labelTextSpeed";
            labelTextSpeed.Size = new Size(60, 25);
            labelTextSpeed.TabIndex = 10;
            labelTextSpeed.Text = "速度：";
            // 
            // comboBoxDeviceSpeed
            // 
            comboBoxDeviceSpeed.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            comboBoxDeviceSpeed.FormattingEnabled = true;
            comboBoxDeviceSpeed.IntegralHeight = false;
            comboBoxDeviceSpeed.ItemHeight = 21;
            comboBoxDeviceSpeed.Items.AddRange(new object[] { "500KHz", "1MHz", "2MHz", "5MHz", "10MHz", "20MHz", "30MHz" });
            comboBoxDeviceSpeed.Location = new Point(70, 67);
            comboBoxDeviceSpeed.Margin = new Padding(5);
            comboBoxDeviceSpeed.Name = "comboBoxDeviceSpeed";
            comboBoxDeviceSpeed.Size = new Size(90, 29);
            comboBoxDeviceSpeed.TabIndex = 9;
            // 
            // buttonDeviceFind
            // 
            buttonDeviceFind.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            buttonDeviceFind.ImageAlign = ContentAlignment.MiddleLeft;
            buttonDeviceFind.Location = new Point(275, 67);
            buttonDeviceFind.Margin = new Padding(5);
            buttonDeviceFind.Name = "buttonDeviceFind";
            buttonDeviceFind.Size = new Size(100, 30);
            buttonDeviceFind.TabIndex = 0;
            buttonDeviceFind.Text = "刷新列表";
            buttonDeviceFind.UseVisualStyleBackColor = true;
            buttonDeviceFind.Click += ButtonDeviceFind_Click;
            // 
            // groupBoxTargetSelect
            // 
            groupBoxTargetSelect.Controls.Add(labelTextTargetManufacturer);
            groupBoxTargetSelect.Controls.Add(labelTextTargetName);
            groupBoxTargetSelect.Controls.Add(comboBoxTargetNameSelect);
            groupBoxTargetSelect.Controls.Add(textBoxTargetManufacturer);
            groupBoxTargetSelect.Location = new Point(170, 30);
            groupBoxTargetSelect.Margin = new Padding(5);
            groupBoxTargetSelect.Name = "groupBoxTargetSelect";
            groupBoxTargetSelect.Size = new Size(230, 330);
            groupBoxTargetSelect.TabIndex = 10;
            groupBoxTargetSelect.TabStop = false;
            groupBoxTargetSelect.Text = "芯片";
            // 
            // labelTextTargetManufacturer
            // 
            labelTextTargetManufacturer.AutoSize = true;
            labelTextTargetManufacturer.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            labelTextTargetManufacturer.ImageAlign = ContentAlignment.TopLeft;
            labelTextTargetManufacturer.Location = new Point(10, 100);
            labelTextTargetManufacturer.Margin = new Padding(5, 0, 5, 0);
            labelTextTargetManufacturer.Name = "labelTextTargetManufacturer";
            labelTextTargetManufacturer.Size = new Size(72, 27);
            labelTextTargetManufacturer.TabIndex = 13;
            labelTextTargetManufacturer.Text = "厂家：";
            // 
            // labelTextTargetName
            // 
            labelTextTargetName.AutoSize = true;
            labelTextTargetName.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            labelTextTargetName.ImageAlign = ContentAlignment.TopLeft;
            labelTextTargetName.Location = new Point(10, 20);
            labelTextTargetName.Margin = new Padding(5, 0, 5, 0);
            labelTextTargetName.Name = "labelTextTargetName";
            labelTextTargetName.Size = new Size(72, 27);
            labelTextTargetName.TabIndex = 11;
            labelTextTargetName.Text = "型号：";
            // 
            // statusStripMain
            // 
            statusStripMain.AutoSize = false;
            statusStripMain.GripMargin = new Padding(0);
            statusStripMain.Items.AddRange(new ToolStripItem[] { toolStripProgressBarMain, toolStripStatusLabelMain });
            statusStripMain.Location = new Point(0, 536);
            statusStripMain.Name = "statusStripMain";
            statusStripMain.Padding = new Padding(1, 0, 16, 0);
            statusStripMain.Size = new Size(984, 25);
            statusStripMain.SizingGrip = false;
            statusStripMain.TabIndex = 25;
            statusStripMain.Text = "statusStripMain";
            // 
            // toolStripProgressBarMain
            // 
            toolStripProgressBarMain.Name = "toolStripProgressBarMain";
            toolStripProgressBarMain.Size = new Size(800, 19);
            toolStripProgressBarMain.Value = 100;
            // 
            // toolStripStatusLabelMain
            // 
            toolStripStatusLabelMain.BorderSides = ToolStripStatusLabelBorderSides.Right;
            toolStripStatusLabelMain.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            toolStripStatusLabelMain.Name = "toolStripStatusLabelMain";
            toolStripStatusLabelMain.Size = new Size(165, 20);
            toolStripStatusLabelMain.Spring = true;
            toolStripStatusLabelMain.Text = "就绪";
            toolStripStatusLabelMain.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // groupBoxLogView
            // 
            groupBoxLogView.Controls.Add(textBoxLogView);
            groupBoxLogView.Location = new Point(410, 30);
            groupBoxLogView.Margin = new Padding(5);
            groupBoxLogView.Name = "groupBoxLogView";
            groupBoxLogView.Size = new Size(565, 485);
            groupBoxLogView.TabIndex = 15;
            groupBoxLogView.TabStop = false;
            groupBoxLogView.Text = "日志";
            // 
            // textBoxLogView
            // 
            textBoxLogView.BackColor = SystemColors.Window;
            textBoxLogView.Font = new Font("Consolas", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxLogView.Location = new Point(10, 20);
            textBoxLogView.Margin = new Padding(5);
            textBoxLogView.Multiline = true;
            textBoxLogView.Name = "textBoxLogView";
            textBoxLogView.ReadOnly = true;
            textBoxLogView.ScrollBars = ScrollBars.Vertical;
            textBoxLogView.Size = new Size(545, 455);
            textBoxLogView.TabIndex = 0;
            // 
            // FormMain
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(groupBoxLogView);
            Controls.Add(statusStripMain);
            Controls.Add(groupBoxTargetSelect);
            Controls.Add(groupDeviceSelect);
            Controls.Add(groupBoxOperation);
            Controls.Add(menuStripTitle);
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStripTitle;
            Name = "FormMain";
            Text = "DapLinkFlashSPI";
            Load += FormMain_Load;
            menuStripTitle.ResumeLayout(false);
            menuStripTitle.PerformLayout();
            groupBoxOperation.ResumeLayout(false);
            groupDeviceSelect.ResumeLayout(false);
            groupBoxTargetSelect.ResumeLayout(false);
            groupBoxTargetSelect.PerformLayout();
            statusStripMain.ResumeLayout(false);
            statusStripMain.PerformLayout();
            groupBoxLogView.ResumeLayout(false);
            groupBoxLogView.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button buttonErase;
        private Button buttonCheckEmpty;
        private Button buttonProgram;
        private Button buttonMatch;
        private Button buttonAuto;
        private ComboBox comboBoxDeviceSelect;
        private ComboBox comboBoxTargetNameSelect;
        private TextBox textBoxTargetManufacturer;
        private Button buttonDump;
        private MenuStrip menuStripTitle;
        private ToolStripMenuItem ToolStripMenuItemFile;
        private ToolStripMenuItem ToolStripMenuItemFileOpen;
        private ToolStripMenuItem ToolStripMenuItemFileSave;
        private ToolStripMenuItem ToolStripMenuItemHelp;
        private ToolStripMenuItem ToolStripMenuItemHelpHelp;
        private ToolStripMenuItem ToolStripMenuItemHelpAbout;
        private GroupBox groupBoxOperation;
        private GroupBox groupDeviceSelect;
        private GroupBox groupBoxTargetSelect;
        private ToolTip toolTipMain;
        private StatusStrip statusStripMain;
        private ToolStripStatusLabel toolStripStatusLabelMain;
        private ToolStripProgressBar toolStripProgressBarMain;
        private Button buttonDeviceFind;
        private Label labelTextTargetName;
        private ComboBox comboBoxDeviceSpeed;
        private Label labelTextTargetManufacturer;
        private GroupBox groupBoxLogView;
        private TextBox textBoxLogView;
        private Label labelTextSpeed;
    }
}
