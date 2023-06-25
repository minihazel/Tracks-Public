namespace LayoutCustomization
{
    partial class splashscreen
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
            this.title1 = new System.Windows.Forms.Label();
            this.loading_icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.loading_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // title1
            // 
            this.title1.Font = new System.Drawing.Font("Bahnschrift Light", 14F);
            this.title1.Location = new System.Drawing.Point(12, 71);
            this.title1.Name = "title1";
            this.title1.Size = new System.Drawing.Size(631, 117);
            this.title1.TabIndex = 0;
            this.title1.Text = "Tracks is loading";
            this.title1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loading_icon
            // 
            this.loading_icon.BackgroundImage = global::LayoutCustomization.Properties.Resources.splashscreen_loading_icon;
            this.loading_icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.loading_icon.Location = new System.Drawing.Point(311, 191);
            this.loading_icon.Name = "loading_icon";
            this.loading_icon.Size = new System.Drawing.Size(32, 32);
            this.loading_icon.TabIndex = 1;
            this.loading_icon.TabStop = false;
            // 
            // splashscreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(655, 344);
            this.ControlBox = false;
            this.Controls.Add(this.loading_icon);
            this.Controls.Add(this.title1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Bahnschrift Light", 10F);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "splashscreen";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.loading_icon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label title1;
        private System.Windows.Forms.PictureBox loading_icon;
    }
}