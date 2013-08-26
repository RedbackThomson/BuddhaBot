namespace BuddhaBot
{
    partial class Configure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configure));
            this.ChannelLbl = new System.Windows.Forms.Label();
            this.ChannelBox = new System.Windows.Forms.TextBox();
            this.UsernameLbl = new System.Windows.Forms.Label();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ConfigureBTN = new System.Windows.Forms.Button();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ChannelLbl
            // 
            this.ChannelLbl.AutoSize = true;
            this.ChannelLbl.Location = new System.Drawing.Point(13, 13);
            this.ChannelLbl.Name = "ChannelLbl";
            this.ChannelLbl.Size = new System.Drawing.Size(49, 13);
            this.ChannelLbl.TabIndex = 0;
            this.ChannelLbl.Text = "Channel:";
            // 
            // ChannelBox
            // 
            this.ChannelBox.Location = new System.Drawing.Point(12, 29);
            this.ChannelBox.Name = "ChannelBox";
            this.ChannelBox.Size = new System.Drawing.Size(197, 20);
            this.ChannelBox.TabIndex = 1;
            // 
            // UsernameLbl
            // 
            this.UsernameLbl.AutoSize = true;
            this.UsernameLbl.Location = new System.Drawing.Point(12, 62);
            this.UsernameLbl.Name = "UsernameLbl";
            this.UsernameLbl.Size = new System.Drawing.Size(77, 13);
            this.UsernameLbl.TabIndex = 2;
            this.UsernameLbl.Text = "Bot Username:";
            // 
            // UsernameBox
            // 
            this.UsernameBox.Location = new System.Drawing.Point(12, 78);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(197, 20);
            this.UsernameBox.TabIndex = 3;
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(12, 114);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.Size = new System.Drawing.Size(197, 20);
            this.PasswordBox.TabIndex = 5;
            this.PasswordBox.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Bot Password:";
            // 
            // ConfigureBTN
            // 
            this.ConfigureBTN.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ConfigureBTN.Location = new System.Drawing.Point(51, 140);
            this.ConfigureBTN.Name = "ConfigureBTN";
            this.ConfigureBTN.Size = new System.Drawing.Size(75, 23);
            this.ConfigureBTN.TabIndex = 6;
            this.ConfigureBTN.Text = "&Configure";
            this.ConfigureBTN.UseVisualStyleBackColor = true;
            // 
            // CancelBTN
            // 
            this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBTN.Location = new System.Drawing.Point(132, 140);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 7;
            this.CancelBTN.Text = "Cancel";
            this.CancelBTN.UseVisualStyleBackColor = true;
            // 
            // Configure
            // 
            this.AcceptButton = this.ConfigureBTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBTN;
            this.ClientSize = new System.Drawing.Size(219, 171);
            this.Controls.Add(this.CancelBTN);
            this.Controls.Add(this.ConfigureBTN);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UsernameBox);
            this.Controls.Add(this.UsernameLbl);
            this.Controls.Add(this.ChannelBox);
            this.Controls.Add(this.ChannelLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Configure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ChannelLbl;
        private System.Windows.Forms.Label UsernameLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ConfigureBTN;
        private System.Windows.Forms.Button CancelBTN;
        public System.Windows.Forms.TextBox ChannelBox;
        public System.Windows.Forms.TextBox UsernameBox;
        public System.Windows.Forms.TextBox PasswordBox;
    }
}