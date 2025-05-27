namespace Figures
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DrawingField = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.DrawingField)).BeginInit();
            this.SuspendLayout();
            // 
            // DrawingField
            // 
            this.DrawingField.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DrawingField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DrawingField.Location = new System.Drawing.Point(0, 0);
            this.DrawingField.Margin = new System.Windows.Forms.Padding(4);
            this.DrawingField.Name = "DrawingField";
            this.DrawingField.Size = new System.Drawing.Size(1400, 870);
            this.DrawingField.TabIndex = 0;
            this.DrawingField.TabStop = false;
            this.DrawingField.SizeChanged += new System.EventHandler(this.DrawingField_SizeChanged);
            this.DrawingField.Click += new System.EventHandler(this.DrawingField_Click);
            this.DrawingField.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingField_Paint);
            this.DrawingField.DoubleClick += new System.EventHandler(this.DrawingField_DoubleClick);
            this.DrawingField.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawingField_MouseDown);
            this.DrawingField.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.DrawingField_PreviewKeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 870);
            this.Controls.Add(this.DrawingField);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.DrawingField)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox DrawingField;
    }
}

