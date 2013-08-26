namespace BuddhaBot
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ConnectBTN = new System.Windows.Forms.Button();
            this.CommandsList = new System.Windows.Forms.ListBox();
            this.CurrentCommandLbl = new System.Windows.Forms.Label();
            this.ListTimer = new System.Windows.Forms.Timer(this.components);
            this.EventList = new System.Windows.Forms.ListBox();
            this.CurrentEventsLbl = new System.Windows.Forms.Label();
            this.DisconnectBTN = new System.Windows.Forms.Button();
            this.ModsList = new System.Windows.Forms.ListBox();
            this.CurrentModeratorsLbl = new System.Windows.Forms.Label();
            this.CreatorLbl = new System.Windows.Forms.Label();
            this.CurrentCensorsLbl = new System.Windows.Forms.Label();
            this.CensorsList = new System.Windows.Forms.ListBox();
            this.ConfigureBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConnectBTN
            // 
            this.ConnectBTN.Location = new System.Drawing.Point(12, 227);
            this.ConnectBTN.Name = "ConnectBTN";
            this.ConnectBTN.Size = new System.Drawing.Size(75, 23);
            this.ConnectBTN.TabIndex = 0;
            this.ConnectBTN.Text = "&Connect";
            this.ConnectBTN.UseVisualStyleBackColor = true;
            this.ConnectBTN.Click += new System.EventHandler(this.ConnectBTN_Click);
            // 
            // CommandsList
            // 
            this.CommandsList.FormattingEnabled = true;
            this.CommandsList.Location = new System.Drawing.Point(276, 23);
            this.CommandsList.Name = "CommandsList";
            this.CommandsList.Size = new System.Drawing.Size(176, 108);
            this.CommandsList.TabIndex = 1;
            // 
            // CurrentCommandLbl
            // 
            this.CurrentCommandLbl.AutoSize = true;
            this.CurrentCommandLbl.Location = new System.Drawing.Point(273, 7);
            this.CurrentCommandLbl.Name = "CurrentCommandLbl";
            this.CurrentCommandLbl.Size = new System.Drawing.Size(99, 13);
            this.CurrentCommandLbl.TabIndex = 2;
            this.CurrentCommandLbl.Text = "Current Commands:";
            // 
            // ListTimer
            // 
            this.ListTimer.Interval = 1000;
            this.ListTimer.Tick += new System.EventHandler(this.ListTimerTick);
            // 
            // EventList
            // 
            this.EventList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.EventList.FormattingEnabled = true;
            this.EventList.Location = new System.Drawing.Point(12, 23);
            this.EventList.Name = "EventList";
            this.EventList.Size = new System.Drawing.Size(258, 199);
            this.EventList.TabIndex = 3;
            this.EventList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.EventListDrawItem);
            // 
            // CurrentEventsLbl
            // 
            this.CurrentEventsLbl.AutoSize = true;
            this.CurrentEventsLbl.Location = new System.Drawing.Point(12, 7);
            this.CurrentEventsLbl.Name = "CurrentEventsLbl";
            this.CurrentEventsLbl.Size = new System.Drawing.Size(59, 13);
            this.CurrentEventsLbl.TabIndex = 4;
            this.CurrentEventsLbl.Text = "Event Log:";
            // 
            // DisconnectBTN
            // 
            this.DisconnectBTN.Location = new System.Drawing.Point(93, 227);
            this.DisconnectBTN.Name = "DisconnectBTN";
            this.DisconnectBTN.Size = new System.Drawing.Size(75, 23);
            this.DisconnectBTN.TabIndex = 5;
            this.DisconnectBTN.Text = "&Disconnect";
            this.DisconnectBTN.UseVisualStyleBackColor = true;
            this.DisconnectBTN.Click += new System.EventHandler(this.DisconnectBTN_Click);
            // 
            // ModsList
            // 
            this.ModsList.FormattingEnabled = true;
            this.ModsList.Location = new System.Drawing.Point(459, 23);
            this.ModsList.Name = "ModsList";
            this.ModsList.Size = new System.Drawing.Size(131, 199);
            this.ModsList.TabIndex = 6;
            // 
            // CurrentModeratorsLbl
            // 
            this.CurrentModeratorsLbl.AutoSize = true;
            this.CurrentModeratorsLbl.Location = new System.Drawing.Point(456, 9);
            this.CurrentModeratorsLbl.Name = "CurrentModeratorsLbl";
            this.CurrentModeratorsLbl.Size = new System.Drawing.Size(96, 13);
            this.CurrentModeratorsLbl.TabIndex = 7;
            this.CurrentModeratorsLbl.Text = "Online Moderators:";
            // 
            // CreatorLbl
            // 
            this.CreatorLbl.AutoSize = true;
            this.CreatorLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreatorLbl.Location = new System.Drawing.Point(467, 233);
            this.CreatorLbl.Name = "CreatorLbl";
            this.CreatorLbl.Size = new System.Drawing.Size(117, 13);
            this.CreatorLbl.TabIndex = 8;
            this.CreatorLbl.Text = "Created by Redback93";
            // 
            // CurrentCensorsLbl
            // 
            this.CurrentCensorsLbl.AutoSize = true;
            this.CurrentCensorsLbl.Location = new System.Drawing.Point(276, 134);
            this.CurrentCensorsLbl.Name = "CurrentCensorsLbl";
            this.CurrentCensorsLbl.Size = new System.Drawing.Size(81, 13);
            this.CurrentCensorsLbl.TabIndex = 9;
            this.CurrentCensorsLbl.Text = "Active Censors:";
            // 
            // CensorsList
            // 
            this.CensorsList.FormattingEnabled = true;
            this.CensorsList.Location = new System.Drawing.Point(277, 151);
            this.CensorsList.Name = "CensorsList";
            this.CensorsList.Size = new System.Drawing.Size(176, 95);
            this.CensorsList.TabIndex = 10;
            // 
            // ConfigureBTN
            // 
            this.ConfigureBTN.Location = new System.Drawing.Point(174, 227);
            this.ConfigureBTN.Name = "ConfigureBTN";
            this.ConfigureBTN.Size = new System.Drawing.Size(75, 23);
            this.ConfigureBTN.TabIndex = 11;
            this.ConfigureBTN.Text = "C&onfigure";
            this.ConfigureBTN.UseVisualStyleBackColor = true;
            this.ConfigureBTN.Click += new System.EventHandler(this.ConfigureBTN_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 262);
            this.Controls.Add(this.ConfigureBTN);
            this.Controls.Add(this.CensorsList);
            this.Controls.Add(this.CurrentCensorsLbl);
            this.Controls.Add(this.CreatorLbl);
            this.Controls.Add(this.CurrentModeratorsLbl);
            this.Controls.Add(this.ModsList);
            this.Controls.Add(this.DisconnectBTN);
            this.Controls.Add(this.CurrentEventsLbl);
            this.Controls.Add(this.EventList);
            this.Controls.Add(this.CurrentCommandLbl);
            this.Controls.Add(this.CommandsList);
            this.Controls.Add(this.ConnectBTN);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "BuddhaBot";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConnectBTN;
        private System.Windows.Forms.ListBox CommandsList;
        private System.Windows.Forms.Label CurrentCommandLbl;
        private System.Windows.Forms.Timer ListTimer;
        private System.Windows.Forms.ListBox EventList;
        private System.Windows.Forms.Label CurrentEventsLbl;
        private System.Windows.Forms.Button DisconnectBTN;
        private System.Windows.Forms.ListBox ModsList;
        private System.Windows.Forms.Label CurrentModeratorsLbl;
        private System.Windows.Forms.Label CreatorLbl;
        private System.Windows.Forms.Label CurrentCensorsLbl;
        private System.Windows.Forms.ListBox CensorsList;
        private System.Windows.Forms.Button ConfigureBTN;
    }
}

