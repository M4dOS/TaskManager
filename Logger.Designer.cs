namespace TaskManager
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
            button2 = new Button();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.Location = new Point(20, 46);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(752, 383);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // button1
            // 
            button1.Location = new Point(20, 9);
            button1.Name = "button1";
            button1.Size = new Size(125, 28);
            button1.TabIndex = 1;
            button1.Text = "Обновление логов";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(250, 7);
            label1.Name = "label1";
            label1.Size = new Size(522, 30);
            label1.TabIndex = 2;
            label1.Text = "Если данные логи не обновляются автоматически, следует нажимать на кнопку обновления \r\nлогов для получения новой информации либо нажимать просто на всякий случай";
            // 
            // button2
            // 
            button2.Location = new Point(148, 9);
            button2.Name = "button2";
            button2.Size = new Size(96, 28);
            button2.TabIndex = 3;
            button2.Text = "Дерево";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Logger
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(richTextBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
        private Button button2;
    }
}