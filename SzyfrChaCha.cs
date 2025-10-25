using System;
using System.Text;

namespace Kryptografia
{
    public static class SzyfrChaCha
    {
        private const int KeySize = 32;
        private const int NonceSize = 12;

        public static string Szyfruj(string tekst, byte[] klucz, byte[] nonce, uint initialCounter = 0)
        {
            if (klucz.Length != KeySize) throw new ArgumentException("Klucz musi mieć 32 bajty.");
            if (nonce.Length != NonceSize) throw new ArgumentException("Nonce musi mieć 12 bajtów.");

            byte[] dane = Encoding.UTF8.GetBytes(tekst);
            byte[] cipher = new byte[dane.Length];

            ChaCha20Engine engine = new ChaCha20Engine(klucz, nonce, initialCounter);
            engine.Process(dane, cipher);

            return Convert.ToBase64String(cipher);
        }

        public static string Odszyfruj(string cipherBase64, byte[] klucz, byte[] nonce, uint initialCounter = 0)
        {
            if (klucz.Length != KeySize) throw new ArgumentException("Klucz musi mieć 32 bajty.");
            if (nonce.Length != NonceSize) throw new ArgumentException("Nonce musi mieć 12 bajtów.");

            byte[] cipher = Convert.FromBase64String(cipherBase64);
            byte[] dane = new byte[cipher.Length];

            ChaCha20Engine engine = new ChaCha20Engine(klucz, nonce, initialCounter);
            engine.Process(cipher, dane);

            return Encoding.UTF8.GetString(dane);
        }

        public static string SzyfrujKompatybilny(string tekst, string kluczAscii, string nonceAscii, uint initialCounter = 1)
        {
            if (string.IsNullOrEmpty(kluczAscii) || kluczAscii.Length < 32)
                throw new ArgumentException("Klucz ASCII musi mieć co najmniej 32 znaki.");
            if (string.IsNullOrEmpty(nonceAscii) || nonceAscii.Length < 12)
                throw new ArgumentException("Nonce ASCII musi mieć co najmniej 12 znaków.");

            byte[] klucz = Encoding.ASCII.GetBytes(kluczAscii.Substring(0, 32));
            byte[] nonce = Encoding.ASCII.GetBytes(nonceAscii.Substring(0, 12));

            return Szyfruj(tekst, klucz, nonce, initialCounter);
        }

        public static string OdszyfrujKompatybilny(string cipherBase64, string kluczAscii, string nonceAscii, uint initialCounter = 0)
        {
            if (string.IsNullOrEmpty(kluczAscii) || kluczAscii.Length < 32)
                throw new ArgumentException("Klucz ASCII musi mieć co najmniej 32 znaki.");
            if (string.IsNullOrEmpty(nonceAscii) || nonceAscii.Length < 12)
                throw new ArgumentException("Nonce ASCII musi mieć co najmniej 12 znaków.");

            byte[] klucz = Encoding.ASCII.GetBytes(kluczAscii.Substring(0, 32));
            byte[] nonce = Encoding.ASCII.GetBytes(nonceAscii.Substring(0, 12));

            return Odszyfruj(cipherBase64, klucz, nonce, initialCounter);
        }

        private class ChaCha20Engine
        {
            private uint[] state = new uint[16];
            private byte[] keyStream = new byte[64];
            private int keyStreamIndex = 64;
            private uint counter;

            public ChaCha20Engine(byte[] key, byte[] nonce, uint initialCounter)
            {
                counter = initialCounter;
                InitializeState(key, nonce);
            }

            private void InitializeState(byte[] key, byte[] nonce)
            {
                if (key.Length != 32) throw new ArgumentException("Key must be 32 bytes");
                if (nonce.Length != 12) throw new ArgumentException("Nonce must be 12 bytes");

                state[0] = 0x61707865;
                state[1] = 0x3320646e;
                state[2] = 0x79622d32;
                state[3] = 0x6b206574;

                for (int i = 0; i < 8; i++)
                    state[4 + i] = ToUInt32LittleEndian(key, i * 4);

                state[12] = counter;
                state[13] = ToUInt32LittleEndian(nonce, 0);
                state[14] = ToUInt32LittleEndian(nonce, 4);
                state[15] = ToUInt32LittleEndian(nonce, 8);
            }

            private static uint ToUInt32LittleEndian(byte[] bytes, int startIndex)
            {
                return (uint)(bytes[startIndex] | (bytes[startIndex + 1] << 8) | 
                             (bytes[startIndex + 2] << 16) | (bytes[startIndex + 3] << 24));
            }

            public void Process(byte[] input, byte[] output)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (keyStreamIndex == 64)
                    {
                        GenerateKeyStream();
                        keyStreamIndex = 0;
                    }

                    output[i] = (byte)(input[i] ^ keyStream[keyStreamIndex]);
                    keyStreamIndex++;
                }
            }

            private void GenerateKeyStream()
            {
                uint[] working = new uint[16];
                Array.Copy(state, working, 16);

                for (int i = 0; i < 10; i++)
                {
                    QuarterRound(ref working[0], ref working[4], ref working[8], ref working[12]);
                    QuarterRound(ref working[1], ref working[5], ref working[9], ref working[13]);
                    QuarterRound(ref working[2], ref working[6], ref working[10], ref working[14]);
                    QuarterRound(ref working[3], ref working[7], ref working[11], ref working[15]);

                    QuarterRound(ref working[0], ref working[5], ref working[10], ref working[15]);
                    QuarterRound(ref working[1], ref working[6], ref working[11], ref working[12]);
                    QuarterRound(ref working[2], ref working[7], ref working[8], ref working[13]);
                    QuarterRound(ref working[3], ref working[4], ref working[9], ref working[14]);
                }

                for (int i = 0; i < 16; i++)
                    working[i] += state[i];

                for (int i = 0; i < 16; i++)
                {
                    int offset = i * 4;
                    keyStream[offset] = (byte)(working[i] & 0xFF);
                    keyStream[offset + 1] = (byte)((working[i] >> 8) & 0xFF);
                    keyStream[offset + 2] = (byte)((working[i] >> 16) & 0xFF);
                    keyStream[offset + 3] = (byte)((working[i] >> 24) & 0xFF);
                }

                state[12]++;
                if (state[12] == 0)
                {
                    state[13]++;
                }
            }

            private static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
            {
                a += b; d ^= a; d = RotateLeft(d, 16);
                c += d; b ^= c; b = RotateLeft(b, 12);
                a += b; d ^= a; d = RotateLeft(d, 8);
                c += d; b ^= c; b = RotateLeft(b, 7);
            }

            private static uint RotateLeft(uint x, int n) => (x << n) | (x >> (32 - n));
        }
    }
}