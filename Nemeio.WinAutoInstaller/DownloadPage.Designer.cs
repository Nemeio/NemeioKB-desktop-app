namespace Nemeio.WinAutoInstaller
{
    partial class DownloadPage
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadPage));
            this.Lab_Header = new System.Windows.Forms.Label();
            this.PB_DownloadProgress = new System.Windows.Forms.ProgressBar();
            this.But_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Lab_Header
            // 
            this.Lab_Header.AutoSize = true;
            this.Lab_Header.Location = new System.Drawing.Point(17, 15);
            this.Lab_Header.Name = "Lab_Header";
            this.Lab_Header.Size = new System.Drawing.Size(81, 13);
            this.Lab_Header.TabIndex = 0;
            this.Lab_Header.Text = "Downloading ...";
            // 
            // PB_DownloadProgress
            // 
            this.PB_DownloadProgress.Location = new System.Drawing.Point(20, 36);
            this.PB_DownloadProgress.Name = "PB_DownloadProgress";
            this.PB_DownloadProgress.Size = new System.Drawing.Size(409, 23);
            this.PB_DownloadProgress.TabIndex = 1;
            // 
            // But_Cancel
            // 
            this.But_Cancel.Location = new System.Drawing.Point(354, 79);
            this.But_Cancel.Name = "But_Cancel";
            this.But_Cancel.Size = new System.Drawing.Size(75, 23);
            this.But_Cancel.TabIndex = 2;
            this.But_Cancel.Text = "Cancel";
            this.But_Cancel.UseVisualStyleBackColor = true;
            // 
            // DownloadPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 117);
            this.Controls.Add(this.But_Cancel);
            this.Controls.Add(this.PB_DownloadProgress);
            this.Controls.Add(this.Lab_Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nemeio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lab_Header;
        private System.Windows.Forms.ProgressBar PB_DownloadProgress;
        private System.Windows.Forms.Button But_Cancel;
    }
}

