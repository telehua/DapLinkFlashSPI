namespace DapLinkFlashSPI
{
    partial class FormAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            textBoxName = new TextBox();
            textBoxAuthor = new TextBox();
            textBoxVersion = new TextBox();
            textBoxFreeIcon = new TextBox();
            textBoxCmsisDap = new TextBox();
            SuspendLayout();
            // 
            // textBoxName
            // 
            textBoxName.BackColor = SystemColors.Control;
            textBoxName.BorderStyle = BorderStyle.None;
            textBoxName.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBoxName.Location = new Point(30, 30);
            textBoxName.Name = "textBoxName";
            textBoxName.ReadOnly = true;
            textBoxName.Size = new Size(152, 21);
            textBoxName.TabIndex = 5;
            textBoxName.Text = "DapLinkFlashSPI";
            // 
            // textBoxAuthor
            // 
            textBoxAuthor.BackColor = SystemColors.Control;
            textBoxAuthor.BorderStyle = BorderStyle.None;
            textBoxAuthor.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBoxAuthor.Location = new Point(30, 60);
            textBoxAuthor.Name = "textBoxAuthor";
            textBoxAuthor.ReadOnly = true;
            textBoxAuthor.Size = new Size(152, 21);
            textBoxAuthor.TabIndex = 6;
            textBoxAuthor.Text = "作者：lihua";
            // 
            // textBoxVersion
            // 
            textBoxVersion.BackColor = SystemColors.Control;
            textBoxVersion.BorderStyle = BorderStyle.None;
            textBoxVersion.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBoxVersion.Location = new Point(30, 90);
            textBoxVersion.Name = "textBoxVersion";
            textBoxVersion.ReadOnly = true;
            textBoxVersion.Size = new Size(152, 21);
            textBoxVersion.TabIndex = 7;
            textBoxVersion.Text = "版本：0.0.1";
            // 
            // textBoxFreeIcon
            // 
            textBoxFreeIcon.BackColor = SystemColors.Control;
            textBoxFreeIcon.BorderStyle = BorderStyle.None;
            textBoxFreeIcon.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBoxFreeIcon.Location = new Point(30, 120);
            textBoxFreeIcon.Name = "textBoxFreeIcon";
            textBoxFreeIcon.ReadOnly = true;
            textBoxFreeIcon.Size = new Size(300, 21);
            textBoxFreeIcon.TabIndex = 8;
            textBoxFreeIcon.Text = "开源图标：Lucide https://lucide.dev";
            // 
            // textBoxCmsisDap
            // 
            textBoxCmsisDap.BackColor = SystemColors.Control;
            textBoxCmsisDap.BorderStyle = BorderStyle.None;
            textBoxCmsisDap.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBoxCmsisDap.Location = new Point(30, 145);
            textBoxCmsisDap.Name = "textBoxCmsisDap";
            textBoxCmsisDap.ReadOnly = true;
            textBoxCmsisDap.Size = new Size(550, 21);
            textBoxCmsisDap.TabIndex = 9;
            textBoxCmsisDap.Text = "协议标准：CMSIS-DAP https://github.com/ARM-software/CMSIS-DAP";
            // 
            // FormAbout
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(634, 261);
            Controls.Add(textBoxCmsisDap);
            Controls.Add(textBoxFreeIcon);
            Controls.Add(textBoxVersion);
            Controls.Add(textBoxAuthor);
            Controls.Add(textBoxName);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormAbout";
            Text = "FormAbout";
            Load += FormAbout_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBoxName;
        private TextBox textBoxAuthor;
        private TextBox textBoxVersion;
        private TextBox textBoxFreeIcon;
        private TextBox textBoxCmsisDap;
    }
}