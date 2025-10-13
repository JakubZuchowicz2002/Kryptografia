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

            if (algorithmComboBox.SelectedItem?.ToString() == "Szyfr Cezara")
            {
                int przesuniecie = (int)shiftNumericUpDown.Value; 
                wynik = SzyfrCezara.Szyfruj(text, przesuniecie);
            }
            outputTextBox.Text = wynik;
        }

        private void EncryptFileButton_Click(object sender, EventArgs e)
        {
            if (algorithmComboBox.SelectedItem?.ToString() != "Szyfr Cezara")
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
                    int przesuniecie = (int)shiftNumericUpDown.Value;
                    string zaszyfrowany = Kryptografia.SzyfrCezara.Szyfruj(text, przesuniecie);

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
            bool showShift = algorithmComboBox.SelectedItem?.ToString() == "Szyfr Cezara";
            shiftNumericUpDown.Visible = showShift;
            shiftLabel.Visible = showShift;
        }
    }
}