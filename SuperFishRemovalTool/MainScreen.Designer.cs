namespace SuperFishRemovalTool
{
    partial class MainScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.RemoveButton = new System.Windows.Forms.Button();
            this.AboutSuperfishLabel = new System.Windows.Forms.Label();
            this.MoreInfoLabel = new System.Windows.Forms.Label();
            this.ManualRemovalLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SecurityAdvisoryLinkLabel = new System.Windows.Forms.LinkLabel();
            this.OverallResultLabel = new System.Windows.Forms.Label();
            this.LenovoStatementLinkLabel = new System.Windows.Forms.LinkLabel();
            this.MainProgressBar = new System.Windows.Forms.ProgressBar();
            this.ResultsFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.RestartNowButton = new System.Windows.Forms.Button();
            this.RestartLaterButton = new System.Windows.Forms.Button();
            this.LicenseAgreementLinkLabel = new System.Windows.Forms.LinkLabel();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.AboutThisToolLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RemoveButton
            // 
            this.RemoveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveButton.Location = new System.Drawing.Point(98, 117);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(501, 52);
            this.RemoveButton.TabIndex = 5;
            this.RemoveButton.Text = "AnalyzeAndRemoveSuperFish Now";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // AboutSuperfishLabel
            // 
            this.AboutSuperfishLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutSuperfishLabel.Location = new System.Drawing.Point(13, 13);
            this.AboutSuperfishLabel.Name = "AboutSuperfishLabel";
            this.AboutSuperfishLabel.Size = new System.Drawing.Size(659, 46);
            this.AboutSuperfishLabel.TabIndex = 7;
            this.AboutSuperfishLabel.Text = "AboutSuperFish";
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MoreInfoLabel.Location = new System.Drawing.Point(12, 485);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(107, 16);
            this.MoreInfoLabel.TabIndex = 8;
            this.MoreInfoLabel.Text = "MoreInformation:";
            // 
            // ManualRemovalLinkLabel
            // 
            this.ManualRemovalLinkLabel.AutoSize = true;
            this.ManualRemovalLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManualRemovalLinkLabel.Location = new System.Drawing.Point(168, 486);
            this.ManualRemovalLinkLabel.Name = "ManualRemovalLinkLabel";
            this.ManualRemovalLinkLabel.Size = new System.Drawing.Size(173, 16);
            this.ManualRemovalLinkLabel.TabIndex = 9;
            this.ManualRemovalLinkLabel.TabStop = true;
            this.ManualRemovalLinkLabel.Text = "ManualRemovalInstructions";
            this.ManualRemovalLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ManualRemovalLinkLabel_LinkClicked);
            // 
            // SecurityAdvisoryLinkLabel
            // 
            this.SecurityAdvisoryLinkLabel.AutoSize = true;
            this.SecurityAdvisoryLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SecurityAdvisoryLinkLabel.Location = new System.Drawing.Point(423, 486);
            this.SecurityAdvisoryLinkLabel.Name = "SecurityAdvisoryLinkLabel";
            this.SecurityAdvisoryLinkLabel.Size = new System.Drawing.Size(154, 16);
            this.SecurityAdvisoryLinkLabel.TabIndex = 11;
            this.SecurityAdvisoryLinkLabel.TabStop = true;
            this.SecurityAdvisoryLinkLabel.Text = "LenovoSecurityAdvisory";
            this.SecurityAdvisoryLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SecurityAdvisoryLinkLabel_LinkClicked);
            // 
            // OverallResultLabel
            // 
            this.OverallResultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverallResultLabel.Location = new System.Drawing.Point(14, 375);
            this.OverallResultLabel.Name = "OverallResultLabel";
            this.OverallResultLabel.Size = new System.Drawing.Size(658, 37);
            this.OverallResultLabel.TabIndex = 12;
            this.OverallResultLabel.Text = "AnalysisResult";
            // 
            // LenovoStatementLinkLabel
            // 
            this.LenovoStatementLinkLabel.AutoSize = true;
            this.LenovoStatementLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LenovoStatementLinkLabel.Location = new System.Drawing.Point(168, 510);
            this.LenovoStatementLinkLabel.Name = "LenovoStatementLinkLabel";
            this.LenovoStatementLinkLabel.Size = new System.Drawing.Size(113, 16);
            this.LenovoStatementLinkLabel.TabIndex = 13;
            this.LenovoStatementLinkLabel.TabStop = true;
            this.LenovoStatementLinkLabel.Text = "LenovoStatement";
            this.LenovoStatementLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LenovoStatementLinkLabel_LinkClicked);
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Location = new System.Drawing.Point(190, 175);
            this.MainProgressBar.MarqueeAnimationSpeed = 30;
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(320, 34);
            this.MainProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.MainProgressBar.TabIndex = 14;
            // 
            // ResultsFlowPanel
            // 
            this.ResultsFlowPanel.AutoScroll = true;
            this.ResultsFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ResultsFlowPanel.Location = new System.Drawing.Point(131, 215);
            this.ResultsFlowPanel.Name = "ResultsFlowPanel";
            this.ResultsFlowPanel.Size = new System.Drawing.Size(448, 145);
            this.ResultsFlowPanel.TabIndex = 15;
            // 
            // RestartNowButton
            // 
            this.RestartNowButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RestartNowButton.Location = new System.Drawing.Point(98, 422);
            this.RestartNowButton.Name = "RestartNowButton";
            this.RestartNowButton.Size = new System.Drawing.Size(222, 52);
            this.RestartNowButton.TabIndex = 16;
            this.RestartNowButton.Text = "RestartNow";
            this.RestartNowButton.UseVisualStyleBackColor = true;
            this.RestartNowButton.Click += new System.EventHandler(this.RestartNowButton_Click);
            // 
            // RestartLaterButton
            // 
            this.RestartLaterButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RestartLaterButton.Location = new System.Drawing.Point(377, 422);
            this.RestartLaterButton.Name = "RestartLaterButton";
            this.RestartLaterButton.Size = new System.Drawing.Size(222, 52);
            this.RestartLaterButton.TabIndex = 17;
            this.RestartLaterButton.Text = "RestartLater";
            this.RestartLaterButton.UseVisualStyleBackColor = true;
            this.RestartLaterButton.Click += new System.EventHandler(this.RestartLaterButton_Click);
            // 
            // LicenseAgreementLinkLabel
            // 
            this.LicenseAgreementLinkLabel.AutoSize = true;
            this.LicenseAgreementLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LicenseAgreementLinkLabel.Location = new System.Drawing.Point(423, 510);
            this.LicenseAgreementLinkLabel.Name = "LicenseAgreementLinkLabel";
            this.LicenseAgreementLinkLabel.Size = new System.Drawing.Size(166, 16);
            this.LicenseAgreementLinkLabel.TabIndex = 18;
            this.LicenseAgreementLinkLabel.TabStop = true;
            this.LicenseAgreementLinkLabel.Text = "LenovoLicenseAgreement";
            this.LicenseAgreementLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LicenseAgreementLinkLabel_LinkClicked);
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.Location = new System.Drawing.Point(12, 510);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(54, 16);
            this.VersionLabel.TabIndex = 19;
            this.VersionLabel.Text = "Version";
            // 
            // AboutThisToolLabel
            // 
            this.AboutThisToolLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutThisToolLabel.Location = new System.Drawing.Point(13, 68);
            this.AboutThisToolLabel.Name = "AboutThisToolLabel";
            this.AboutThisToolLabel.Size = new System.Drawing.Size(659, 46);
            this.AboutThisToolLabel.TabIndex = 20;
            this.AboutThisToolLabel.Text = "AboutThisTool";
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 535);
            this.Controls.Add(this.AboutThisToolLabel);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.LicenseAgreementLinkLabel);
            this.Controls.Add(this.RestartLaterButton);
            this.Controls.Add(this.RestartNowButton);
            this.Controls.Add(this.ResultsFlowPanel);
            this.Controls.Add(this.MainProgressBar);
            this.Controls.Add(this.LenovoStatementLinkLabel);
            this.Controls.Add(this.OverallResultLabel);
            this.Controls.Add(this.SecurityAdvisoryLinkLabel);
            this.Controls.Add(this.ManualRemovalLinkLabel);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.AboutSuperfishLabel);
            this.Controls.Add(this.RemoveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainScreen";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SuperFishRemovalUtility";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Label AboutSuperfishLabel;
        private System.Windows.Forms.Label MoreInfoLabel;
        private System.Windows.Forms.LinkLabel ManualRemovalLinkLabel;
        private System.Windows.Forms.LinkLabel SecurityAdvisoryLinkLabel;
        private System.Windows.Forms.Label OverallResultLabel;
        private System.Windows.Forms.LinkLabel LenovoStatementLinkLabel;
        private System.Windows.Forms.ProgressBar MainProgressBar;
        private System.Windows.Forms.FlowLayoutPanel ResultsFlowPanel;
        private System.Windows.Forms.Button RestartNowButton;
        private System.Windows.Forms.Button RestartLaterButton;
        private System.Windows.Forms.LinkLabel LicenseAgreementLinkLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label AboutThisToolLabel;
    }
}