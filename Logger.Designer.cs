﻿namespace TaskManager
{
    partial class Logger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Logger));
            richTextBox1 = new RichTextBox();
            button1 = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(20, 46);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(752, 383);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            richTextBox1.ReadOnly = true;
            // 
            // button1
            // 
            button1.Location = new Point(20, 9);
            button1.Name = "button1";
            button1.Size = new Size(173, 28);
            button1.TabIndex = 1;
            button1.Text = "Обновление логов";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(355, 9);
            label1.Name = "label1";
            label1.Size = new Size(417, 30);
            label1.TabIndex = 2;
            label1.Text = "Данные логи не обновляются автоматически, поэтому следует нажимать \r\nна кнопку обновления логов для получения новой информации";
            // 
            // Logger
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(richTextBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Logger";
            Text = "Logger";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button button1;
        private Label label1;
    }
}