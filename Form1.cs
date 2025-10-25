using System;
using System.IO;
using System.Windows.Forms;
using Kryptografia;

namespace kryptografia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

            private void EncryptButton_Click(object sender, EventArgs e)
        {
            string text = inputTextBox.Text;
            string wynik = "";
            string? selected = algorithmComboBox.SelectedItem?.ToString();

            if (selected == "Szyfr Cezara")
            {
                int przesuniecie = (int)shiftNumericUpDown.Value;
                wynik = SzyfrCezara.Szyfruj(text, przesuniecie);
            }
            else if (selected == "Szyfr Vigenère'a")
            {
                string klucz = keyTextBox.Text;
                if (string.IsNullOrWhiteSpace(klucz))
                {
                    MessageBox.Show("Podaj klucz!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                wynik = SzyfrVigenere.Szyfruj(text, klucz);
            }
            else if (selected == "Running Key Cipher")
            {
                string klucz = keyTextBox.Text;
                if (string.IsNullOrWhiteSpace(klucz) || klucz.Length < text.Length)
                {
                    MessageBox.Show("Klucz musi być co najmniej tak długi jak tekst!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                wynik = SzyfrRunningKey.Szyfruj(text, klucz);
            }
            outputTextBox.Text = wynik;
        }

        private void EncryptFileButton_Click(object sender, EventArgs e)
        {
            string? selected = algorithmComboBox.SelectedItem?.ToString();

            if (selected == "Wybierz szyfr" || string.IsNullOrEmpty(selected))
            {
                MessageBox.Show("Wybierz najpierw szyfr!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string inputPath = openFileDialog.FileName;
                    string text = File.ReadAllText(inputPath);
                    string zaszyfrowany = "";

                    if (selected == "Szyfr Cezara")
                    {
                        int przesuniecie = (int)shiftNumericUpDown.Value;
                        zaszyfrowany = Kryptografia.SzyfrCezara.Szyfruj(text, przesuniecie);
                    }
                    else if (selected == "Szyfr Vigenère'a")
                    {
                        string klucz = keyTextBox.Text;
                        if (string.IsNullOrWhiteSpace(klucz))
                        {
                            MessageBox.Show("Podaj klucz!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        zaszyfrowany = Kryptografia.SzyfrVigenere.Szyfruj(text, klucz);
                    }
                    else if (selected == "Running Key Cipher")
                    {
                        string klucz = keyTextBox.Text;
                        if (string.IsNullOrWhiteSpace(klucz) || klucz.Length < text.Length)
                        {
                            MessageBox.Show("Klucz musi być co najmniej tak długi jak tekst!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        zaszyfrowany = Kryptografia.SzyfrRunningKey.Szyfruj(text, klucz);
                    }

                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                        saveFileDialog.FileName = "zaszyfrowany.txt";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(saveFileDialog.FileName, zaszyfrowany);
                            MessageBox.Show("Plik został zaszyfrowany!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

       private void algorithmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        string? selected = algorithmComboBox.SelectedItem?.ToString();
        bool showShift = selected == "Szyfr Cezara";
        bool showKey = selected == "Szyfr Vigenère'a" || selected == "Running Key Cipher";
        shiftNumericUpDown.Visible = shiftLabel.Visible = showShift;
        keyTextBox.Visible = keyLabel.Visible = showKey;
        }
    }
}