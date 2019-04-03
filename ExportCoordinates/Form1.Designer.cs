namespace ExportCoordinates
{
    partial class Form1
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
            this.checkBox1_Listbox = new System.Windows.Forms.CheckBox();
            this.label2_Found = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1_Cancel = new System.Windows.Forms.Button();
            this.button1_Export = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // checkBox1_Listbox
            // 
            this.checkBox1_Listbox.AutoSize = true;
            this.checkBox1_Listbox.Location = new System.Drawing.Point(14, 2);
            this.checkBox1_Listbox.Name = "checkBox1_Listbox";
            this.checkBox1_Listbox.Size = new System.Drawing.Size(80, 17);
            this.checkBox1_Listbox.TabIndex = 13;
            this.checkBox1_Listbox.Text = "All viewed! ";
            this.checkBox1_Listbox.UseVisualStyleBackColor = true;
            // 
            // label2_Found
            // 
            this.label2_Found.AutoSize = true;
            this.label2_Found.Location = new System.Drawing.Point(11, 455);
            this.label2_Found.Name = "label2_Found";
            this.label2_Found.Size = new System.Drawing.Size(66, 13);
            this.label2_Found.TabIndex = 12;
            this.label2_Found.Text = "Layer found!";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(214, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "Filter";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(254, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(139, 20);
            this.textBox1.TabIndex = 10;
            // 
            // button1_Cancel
            // 
            this.button1_Cancel.Location = new System.Drawing.Point(211, 455);
            this.button1_Cancel.Name = "button1_Cancel";
            this.button1_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button1_Cancel.TabIndex = 9;
            this.button1_Cancel.Text = "Cancel";
            this.button1_Cancel.UseVisualStyleBackColor = true;
            this.button1_Cancel.Click += new System.EventHandler(this.button1_Cancel_Click);
            // 
            // button1_Export
            // 
            this.button1_Export.Location = new System.Drawing.Point(118, 456);
            this.button1_Export.Name = "button1_Export";
            this.button1_Export.Size = new System.Drawing.Size(75, 23);
            this.button1_Export.TabIndex = 8;
            this.button1_Export.Text = "Export";
            this.button1_Export.UseVisualStyleBackColor = true;
            this.button1_Export.Click += new System.EventHandler(this.button1_Export_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 26);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(383, 424);
            this.checkedListBox1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 488);
            this.Controls.Add(this.checkBox1_Listbox);
            this.Controls.Add(this.label2_Found);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1_Cancel);
            this.Controls.Add(this.button1_Export);
            this.Controls.Add(this.checkedListBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Export Structure Coordinates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1_Listbox;
        private System.Windows.Forms.Label label2_Found;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1_Cancel;
        private System.Windows.Forms.Button button1_Export;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}