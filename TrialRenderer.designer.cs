namespace MissionPlanner.Plugins.AEOLUS
{
    partial class TrialRenderer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.focusDotGraphic = new System.Windows.Forms.PictureBox();
            this.label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.focusDotGraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(485, 348);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // focusDotGraphic
            // 
            this.focusDotGraphic.BackColor = System.Drawing.Color.Transparent;
            this.focusDotGraphic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.focusDotGraphic.Enabled = false;
            this.focusDotGraphic.Location = new System.Drawing.Point(0, 0);
            this.focusDotGraphic.Name = "focusDotGraphic";
            this.focusDotGraphic.Size = new System.Drawing.Size(485, 348);
            this.focusDotGraphic.TabIndex = 3;
            this.focusDotGraphic.TabStop = false;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(0, 316);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(0, 32);
            this.label.TabIndex = 4;
            // 
            // TrialRenderer
            // 
            this.Controls.Add(this.label);
            this.Controls.Add(this.focusDotGraphic);
            this.Controls.Add(this.pictureBox1);
            this.Name = "TrialRenderer";
            this.Size = new System.Drawing.Size(485, 348);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.focusDotGraphic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox focusDotGraphic;
        private System.Windows.Forms.Label label;
    }
}
