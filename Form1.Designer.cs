namespace kryptografia;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private System.Windows.Forms.TextBox inputTextBox;
    private System.Windows.Forms.TextBox outputTextBox;
    private System.Windows.Forms.ComboBox algorithmComboBox;
    private System.Windows.Forms.Button EncryptButton;
    private System.Windows.Forms.Button EncryptFileButton; // Dodaj pole
    private System.Windows.Forms.Label inputLabel;
    private System.Windows.Forms.Label outputLabel;
    private System.Windows.Forms.NumericUpDown shiftNumericUpDown;
    private System.Windows.Forms.Label shiftLabel; // Dodaj pole na label
    private System.Windows.Forms.Label keyLabel;
    private System.Windows.Forms.TextBox keyTextBox;

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.inputTextBox = new System.Windows.Forms.TextBox();
        this.outputTextBox = new System.Windows.Forms.TextBox();
        this.algorithmComboBox = new System.Windows.Forms.ComboBox();
        this.EncryptButton = new System.Windows.Forms.Button();
        this.EncryptFileButton = new System.Windows.Forms.Button();
        this.inputLabel = new System.Windows.Forms.Label();
        this.outputLabel = new System.Windows.Forms.Label();
        this.shiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.shiftLabel = new System.Windows.Forms.Label(); 
        this.keyLabel = new System.Windows.Forms.Label();
        this.keyTextBox = new System.Windows.Forms.TextBox();
        ((System.ComponentModel.ISupportInitialize)(this.shiftNumericUpDown)).BeginInit();
        this.SuspendLayout();
        //
        // inputTextBox
        //
        this.inputTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        this.inputTextBox.Location = new System.Drawing.Point(12, 29);
        this.inputTextBox.Multiline = true;
        this.inputTextBox.Name = "inputTextBox";
        this.inputTextBox.Size = new System.Drawing.Size(260, 110);
        this.inputTextBox.TabIndex = 0;
        //
        // outputTextBox
        //
        this.outputTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        this.outputTextBox.Location = new System.Drawing.Point(12, 175);
        this.outputTextBox.Multiline = true;
        this.outputTextBox.Name = "outputTextBox";
        this.outputTextBox.Size = new System.Drawing.Size(260, 114);
        this.outputTextBox.TabIndex = 1;
        this.outputTextBox.ReadOnly = true;
        //
        // algorithmComboBox
        //
        this.algorithmComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        this.algorithmComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.algorithmComboBox.FormattingEnabled = true;
        this.algorithmComboBox.Items.AddRange(new object[] {
        "Wybierz szyfr", "Szyfr Cezara", "Szyfr Vigenère'a", "Running Key Cipher"});
        this.algorithmComboBox.Location = new System.Drawing.Point(290, 29);
        this.algorithmComboBox.Name = "algorithmComboBox";
        this.algorithmComboBox.Size = new System.Drawing.Size(121, 23);
        this.algorithmComboBox.TabIndex = 2;
        this.algorithmComboBox.SelectedIndex = 0;
        this.algorithmComboBox.SelectedIndexChanged += new System.EventHandler(this.algorithmComboBox_SelectedIndexChanged);
        //
        // shiftLabel
        //
        this.shiftLabel.AutoSize = true;
        this.shiftLabel.Location = new System.Drawing.Point(290, 58);
        this.shiftLabel.Name = "shiftLabel";
        this.shiftLabel.Size = new System.Drawing.Size(68, 15);
        this.shiftLabel.TabIndex = 7;
        this.shiftLabel.Text = "Przesunięcie:";
        this.shiftLabel.Visible = false;
        //
        // shiftNumericUpDown
        //
        this.shiftNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        this.shiftNumericUpDown.Location = new System.Drawing.Point(290, 76);
        this.shiftNumericUpDown.Minimum = 1;
        this.shiftNumericUpDown.Maximum = 25;
        this.shiftNumericUpDown.Value = 3;
        this.shiftNumericUpDown.Name = "shiftNumericUpDown";
        this.shiftNumericUpDown.Size = new System.Drawing.Size(121, 23);
        this.shiftNumericUpDown.TabIndex = 6;
        this.shiftNumericUpDown.Visible = false;
        //
        // EncryptButton
        //
        this.EncryptButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        this.EncryptButton.Location = new System.Drawing.Point(290, 115);
        this.EncryptButton.Name = "EncryptButton";
        this.EncryptButton.Size = new System.Drawing.Size(121, 30);
        this.EncryptButton.TabIndex = 3;
        this.EncryptButton.Text = "Szyfruj";
        this.EncryptButton.UseVisualStyleBackColor = true;
        this.EncryptButton.Click += new System.EventHandler(this.EncryptButton_Click);
        //
        // EncryptFileButton
        //
        this.EncryptFileButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        this.EncryptFileButton.Location = new System.Drawing.Point(290, 155);
        this.EncryptFileButton.Name = "EncryptFileButton";
        this.EncryptFileButton.Size = new System.Drawing.Size(121, 30);
        this.EncryptFileButton.TabIndex = 8;
        this.EncryptFileButton.Text = "Szyfruj plik";
        this.EncryptFileButton.UseVisualStyleBackColor = true;
        this.EncryptFileButton.Click += new System.EventHandler(this.EncryptFileButton_Click);
        //
        // keyLabel
        //
        this.keyLabel.AutoSize = true;
        this.keyLabel.Location = new System.Drawing.Point(290, 58);
        this.keyLabel.Name = "keyLabel";
        this.keyLabel.Size = new System.Drawing.Size(39, 15);
        this.keyLabel.TabIndex = 9;
        this.keyLabel.Text = "Klucz:";
        this.keyLabel.Visible = false;
        //
        // keyTextBox
        //
        this.keyTextBox.Location = new System.Drawing.Point(290, 76);
        this.keyTextBox.Name = "keyTextBox";
        this.keyTextBox.Size = new System.Drawing.Size(121, 23);
        this.keyTextBox.TabIndex = 10;
        this.keyTextBox.Visible = false;
        //
        // inputLabel
        //
        this.inputLabel.AutoSize = true;
        this.inputLabel.Location = new System.Drawing.Point(12, 9);
        this.inputLabel.Name = "inputLabel";
        this.inputLabel.Size = new System.Drawing.Size(120, 15);
        this.inputLabel.TabIndex = 4;
        this.inputLabel.Text = "Tekst do szyfrowania:";
        //
        // outputLabel
        //
        this.outputLabel.AutoSize = true;
        this.outputLabel.Location = new System.Drawing.Point(12, 157); 
        this.outputLabel.Name = "outputLabel";
        this.outputLabel.Size = new System.Drawing.Size(117, 15);
        this.outputLabel.TabIndex = 5;
        this.outputLabel.Text = "Zaszyfrowany tekst:";
        //
        // Form1
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(430, 320); 
        this.Controls.Add(this.keyLabel);
        this.Controls.Add(this.keyTextBox);
        this.Controls.Add(this.EncryptFileButton);
        this.Controls.Add(this.shiftLabel);
        this.Controls.Add(this.shiftNumericUpDown);
        this.Controls.Add(this.outputLabel);
        this.Controls.Add(this.inputLabel);
        this.Controls.Add(this.EncryptButton);
        this.Controls.Add(this.algorithmComboBox);
        this.Controls.Add(this.outputTextBox);
        this.Controls.Add(this.inputTextBox);
        this.MinimumSize = new System.Drawing.Size(450, 360); 
        this.Name = "Form1";
        this.Text = "Kryptografia";
        ((System.ComponentModel.ISupportInitialize)(this.shiftNumericUpDown)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
        
    }

    #endregion
}
