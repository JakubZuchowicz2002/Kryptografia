using System;
using System.IO;
using System.Windows.Forms;
using Kryptografia;
using System.Security.Cryptography;
using System.Text;

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
            else if (selected == "AES")
            {
                if (string.IsNullOrEmpty(text))
                {
                    MessageBox.Show("Brak tekstu do zaszyfrowania.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string pass = keyTextBox.Text ?? "";
                if (string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Podaj klucz (hasło) do AES!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                byte[] key = DeriveKey(pass);
                string modeStr = aesModeComboBox.SelectedItem?.ToString() ?? "CBC";
                if (!Enum.TryParse<SzyfrAES.TrybAES>(modeStr, out var tryb))
                    tryb = SzyfrAES.TrybAES.CBC;

                int ivLen = (tryb == SzyfrAES.TrybAES.GCM) ? 12 : 16;
                byte[] iv = ParseOrGenerateIV(ivTextBox.Text, ivLen);

                byte[]? tag;
                try
                {
                    string cipherBase64 = SzyfrAES.Szyfruj(text, key, iv, tryb, out tag);
                    if (tag != null)
                        wynik = $"{cipherBase64}:{Convert.ToBase64String(tag)}:{Convert.ToBase64String(iv)}";
                    else
                        wynik = $"{cipherBase64}:{Convert.ToBase64String(iv)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd szyfrowania AES: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (selected == "ChaCha20")
            {
                if (string.IsNullOrEmpty(text))
                {
                    MessageBox.Show("Brak tekstu do zaszyfrowania.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string pass = keyTextBox.Text ?? "";
                if (string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Podaj klucz (32 znaki ASCII) do ChaCha20!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nonceStr = ivTextBox.Text ?? "";
                if (string.IsNullOrWhiteSpace(nonceStr))
                {
                    MessageBox.Show("Podaj nonce (12 znaków ASCII) do ChaCha20!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    wynik = SzyfrChaCha.SzyfrujKompatybilny(text, pass, nonceStr, 0);
                    
                    outputTextBox.Text = wynik;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd szyfrowania ChaCha20: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Wybierz poprawny szyfr!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
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
                        zaszyfrowany = SzyfrCezara.Szyfruj(text, przesuniecie);
                    }
                    else if (selected == "Szyfr Vigenère'a")
                    {
                        string klucz = keyTextBox.Text;
                        if (string.IsNullOrWhiteSpace(klucz))
                        {
                            MessageBox.Show("Podaj klucz!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        zaszyfrowany = SzyfrVigenere.Szyfruj(text, klucz);
                    }
                    else if (selected == "Running Key Cipher")
                    {
                        string klucz = keyTextBox.Text;
                        if (string.IsNullOrWhiteSpace(klucz) || klucz.Length < text.Length)
                        {
                            MessageBox.Show("Klucz musi być co najmniej tak długi jak tekst!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        zaszyfrowany = SzyfrRunningKey.Szyfruj(text, klucz);
                    }
                    else if (selected == "AES")
                    {
                        string pass = keyTextBox.Text ?? "";
                        if (string.IsNullOrWhiteSpace(pass))
                        {
                            MessageBox.Show("Podaj klucz (hasło) do AES!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        byte[] key = DeriveKey(pass);
                        string modeStr = aesModeComboBox.SelectedItem?.ToString() ?? "CBC";
                        if (!Enum.TryParse<SzyfrAES.TrybAES>(modeStr, out var tryb))
                            tryb = SzyfrAES.TrybAES.CBC;

                        int ivLen = (tryb == SzyfrAES.TrybAES.GCM) ? 12 : 16;
                        byte[] iv = ParseOrGenerateIV(ivTextBox.Text, ivLen);

                        byte[]? tag;
                        try
                        {
                            string cipherBase64 = SzyfrAES.Szyfruj(text, key, iv, tryb, out tag);
                            if (tag != null)
                                zaszyfrowany = $"{cipherBase64}:{Convert.ToBase64String(tag)}:{Convert.ToBase64String(iv)}";
                            else
                                zaszyfrowany = $"{cipherBase64}:{Convert.ToBase64String(iv)}";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd szyfrowania AES: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else if (selected == "ChaCha20")
                    {
                        string pass = keyTextBox.Text ?? "";
                        if (string.IsNullOrWhiteSpace(pass))
                        {
                            MessageBox.Show("Podaj klucz (hasło) do ChaCha20!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string nonceStr = ivTextBox.Text ?? "";
                        if (string.IsNullOrWhiteSpace(nonceStr))
                        {
                            MessageBox.Show("Podaj nonce (12 znaków ASCII) do ChaCha20!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        try
                        {
                            zaszyfrowany = SzyfrChaCha.SzyfrujKompatybilny(text, pass, nonceStr, 0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd szyfrowania ChaCha20: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
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

        private void GenerateKeyButton_Click(object sender, EventArgs e)
        {
            string? selected = algorithmComboBox.SelectedItem?.ToString();
            
            if (selected == "AES")
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                char[] pwd = new char[16];
                byte[] randomBytes = new byte[16];
                RandomNumberGenerator.Fill(randomBytes);

                for (int i = 0; i < 16; i++)
                {
                    pwd[i] = chars[randomBytes[i] % chars.Length];
                }

                keyTextBox.Text = new string(pwd);
            }
            else if (selected == "ChaCha20")
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                char[] key = new char[32];
                byte[] randomBytes = new byte[32];
                RandomNumberGenerator.Fill(randomBytes);

                for (int i = 0; i < 32; i++)
                {
                    key[i] = chars[randomBytes[i] % chars.Length];
                }

                keyTextBox.Text = new string(key);
            }
        }

        private void GenerateNonceButton_Click(object sender, EventArgs e)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] nonce = new char[12];
            byte[] randomBytes = new byte[12];
            RandomNumberGenerator.Fill(randomBytes);

            for (int i = 0; i < 12; i++)
            {
                nonce[i] = chars[randomBytes[i] % chars.Length];
            }

            ivTextBox.Text = new string(nonce);
        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {
            string pakiet = inputTextBox.Text;
            if (string.IsNullOrWhiteSpace(pakiet))
            {
                MessageBox.Show("Wprowadź zaszyfrowany pakiet (tekst) do odszyfrowania.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string? selected = algorithmComboBox.SelectedItem?.ToString();

            if (selected == "AES")
            {
                string pass = keyTextBox.Text ?? "";
                if (string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Podaj klucz (hasło) do AES!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                byte[] key = DeriveKey(pass);
                string modeStr = aesModeComboBox.SelectedItem?.ToString() ?? "CBC";
                if (!Enum.TryParse<SzyfrAES.TrybAES>(modeStr, out var tryb))
                    tryb = SzyfrAES.TrybAES.CBC;

                try
                {
                    string plain = SzyfrAES.Odszyfruj(pakiet, key, tryb);
                    outputTextBox.Text = plain;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd odszyfrowania: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (selected == "ChaCha20")
            {
                string pass = keyTextBox.Text ?? "";
                if (string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Podaj klucz (32 znaki ASCII) do ChaCha20!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nonceStr = ivTextBox.Text ?? "";
                if (string.IsNullOrWhiteSpace(nonceStr))
                {
                    MessageBox.Show("Podaj nonce (12 znaków ASCII) do ChaCha20!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string plain = SzyfrChaCha.OdszyfrujKompatybilny(pakiet, pass, nonceStr, 0);
                    outputTextBox.Text = plain;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd odszyfrowania ChaCha20: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Odszyfrowywanie obsługiwane tylko dla AES i ChaCha20.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DecryptFileButton_Click(object sender, EventArgs e)
        {
            string? selected = algorithmComboBox.SelectedItem?.ToString();
            if (selected != "AES" && selected != "ChaCha20")
            {
                MessageBox.Show("Odszyfrowywanie plików obsługiwane jest tylko dla AES i ChaCha20.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string inputPath = openFileDialog.FileName;
                    string pakiet = File.ReadAllText(inputPath);

                    string pass = keyTextBox.Text ?? "";
                    if (string.IsNullOrWhiteSpace(pass))
                    {
                        MessageBox.Show("Podaj klucz (hasło)!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        string plain = "";

                        if (selected == "AES")
                        {
                            byte[] key = DeriveKey(pass);
                            string modeStr = aesModeComboBox.SelectedItem?.ToString() ?? "CBC";
                            if (!Enum.TryParse<SzyfrAES.TrybAES>(modeStr, out var tryb))
                                tryb = SzyfrAES.TrybAES.CBC;

                            plain = SzyfrAES.Odszyfruj(pakiet, key, tryb);
                        }
                        else if (selected == "ChaCha20")
                        {
                            string nonceStr = ivTextBox.Text ?? "";
                            if (string.IsNullOrWhiteSpace(nonceStr))
                            {
                                MessageBox.Show("Podaj nonce (12 znaków ASCII) do ChaCha20!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            try
                            {
                                plain = SzyfrChaCha.OdszyfrujKompatybilny(pakiet, pass, nonceStr, 0);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Błąd odszyfrowania ChaCha20: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                            saveFileDialog.FileName = "odszyfrowany.txt";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                File.WriteAllText(saveFileDialog.FileName, plain);
                                MessageBox.Show("Plik został odszyfrowany!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd odszyfrowania pliku: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void algorithmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selected = algorithmComboBox.SelectedItem?.ToString();
            bool showShift = selected == "Szyfr Cezara";
            bool showKeyClassic = selected == "Szyfr Vigenère'a" || selected == "Running Key Cipher";
            bool showAES = selected == "AES";
            bool showChaCha = selected == "ChaCha20";

            shiftNumericUpDown.Visible = shiftLabel.Visible = showShift;
            keyTextBox.Visible = keyLabel.Visible = showAES || showKeyClassic || showChaCha;
            ivTextBox.Visible = ivLabel.Visible = showAES || showChaCha;
            aesModeComboBox.Visible = aesModeLabel.Visible = showAES;
            
            GenerateKeyButton.Visible = showAES || showChaCha;
            GenerateNonceButton.Visible = showChaCha; 

            if (showAES)
            {
                keyLabel.Text = "Klucz (hasło):";
                keyTextBox.PlaceholderText = "Hasło -> SHA256 -> klucz";
                ivLabel.Text = "IV:";
                ivTextBox.PlaceholderText = "Base64 lub hex (opcjonalne)";
                GenerateKeyButton.Text = "Generuj hasło";
            }
            else if (showChaCha)
            {
                keyLabel.Text = "Klucz (32 znaki ASCII):";
                keyTextBox.PlaceholderText = "Dokładnie 32 znaki ASCII";
                ivLabel.Text = "Nonce (12 znaków ASCII):";
                ivTextBox.PlaceholderText = "Dokładnie 12 znaków ASCII";
                GenerateKeyButton.Text = "Generuj klucz";
            }
            else
            {
                keyLabel.Text = "Klucz:";
                keyTextBox.PlaceholderText = "";
            }
        }

        private static byte[] DeriveKey(string password)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(password));
        }

        private static byte[] ParseOrGenerateIV(string input, int length)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                byte[] iv = new byte[length];
                RandomNumberGenerator.Fill(iv);
                return iv;
            }

            try
            {
                byte[] b = Convert.FromBase64String(input);
                if (b.Length >= 1)
                {
                    if (b.Length == length) return b;
                    if (b.Length > length)
                    {
                        byte[] r = new byte[length];
                        Array.Copy(b, 0, r, 0, length);
                        return r;
                    }
                }
            }
            catch { }

            try
            {
                string s = input.Trim();
                if (s.Length % 2 == 0)
                {
                    int bytesLen = s.Length / 2;
                    byte[] result = new byte[bytesLen];
                    for (int i = 0; i < bytesLen; i++)
                        result[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);

                    if (result.Length == length) return result;
                    if (result.Length > length)
                    {
                        byte[] r = new byte[length];
                        Array.Copy(result, 0, r, 0, length);
                        return r;
                    }
                }
            }
            catch { }

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            byte[] finalIv = new byte[length];
            Array.Copy(hash, finalIv, length);
            return finalIv;
        }
    }
}