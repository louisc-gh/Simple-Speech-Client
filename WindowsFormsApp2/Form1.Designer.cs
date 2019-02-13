namespace WindowsFormsApp2
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
            this.startRecognitionButton = new System.Windows.Forms.Button();
            this.transcriptTextControl = new System.Windows.Forms.RichTextBox();
            this.stopRecognitionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // startRecognitionButton
            // 
            this.startRecognitionButton.AutoSize = true;
            this.startRecognitionButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.startRecognitionButton.Location = new System.Drawing.Point(12, 23);
            this.startRecognitionButton.Name = "startRecognitionButton";
            this.startRecognitionButton.Size = new System.Drawing.Size(103, 25);
            this.startRecognitionButton.TabIndex = 0;
            this.startRecognitionButton.Text = "Start Recognition";
            this.startRecognitionButton.UseVisualStyleBackColor = true;
            this.startRecognitionButton.Click += new System.EventHandler(this.OnStartRecognitionClicked);
            // 
            // transcriptTextControl
            // 
            this.transcriptTextControl.Location = new System.Drawing.Point(12, 73);
            this.transcriptTextControl.Name = "transcriptTextControl";
            this.transcriptTextControl.Size = new System.Drawing.Size(765, 328);
            this.transcriptTextControl.TabIndex = 1;
            this.transcriptTextControl.Text = "";
            // 
            // stopRecognitionButton
            // 
            this.stopRecognitionButton.AutoSize = true;
            this.stopRecognitionButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.stopRecognitionButton.Location = new System.Drawing.Point(137, 23);
            this.stopRecognitionButton.Name = "stopRecognitionButton";
            this.stopRecognitionButton.Size = new System.Drawing.Size(103, 25);
            this.stopRecognitionButton.TabIndex = 2;
            this.stopRecognitionButton.Text = "Stop Recognition";
            this.stopRecognitionButton.UseVisualStyleBackColor = true;
            this.stopRecognitionButton.Click += new System.EventHandler(this.stopRecognitionButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.stopRecognitionButton);
            this.Controls.Add(this.transcriptTextControl);
            this.Controls.Add(this.startRecognitionButton);
            this.Name = "Form1";
            this.Text = "Simple Recognizer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Button startRecognitionButton;
        private System.Windows.Forms.RichTextBox transcriptTextControl;
        private System.Windows.Forms.Button stopRecognitionButton;
    }
}

