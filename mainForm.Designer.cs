namespace LayoutCustomization
{
    partial class mainForm
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
            this.b_placeholder = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // b_placeholder
            // 
            this.b_placeholder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.b_placeholder.AutoSize = true;
            this.b_placeholder.Font = new System.Drawing.Font("Bahnschrift Light", 5F);
            this.b_placeholder.Location = new System.Drawing.Point(12, 559);
            this.b_placeholder.Name = "b_placeholder";
            this.b_placeholder.Size = new System.Drawing.Size(42, 8);
            this.b_placeholder.TabIndex = 0;
            this.b_placeholder.Text = "placeholder";
            this.b_placeholder.Visible = false;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(780, 576);
            this.Controls.Add(this.b_placeholder);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Bahnschrift Light", 10F);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Layout";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label b_placeholder;
    }
}

