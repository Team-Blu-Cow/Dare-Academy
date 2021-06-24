using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Security.Cryptography;

// modified from microsoft documentation on 07-JUN-2021
// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-5.0

namespace blu.FileIO
{
    public class Encryptor
    {
        public Encryptor(byte[] key, byte[] IV)
        {
            m_key = key;
            m_iv = IV;

            if (m_key == null)
                m_key = new byte[0];

            if (m_iv == null)
                m_iv = new byte[0];

            if (m_key.Length != 32)
            {
                Debug.LogError("AES Key invalid length, length = " + m_key.Length + " bytes, length of 32 bytes was expected");
            }

            if (m_iv.Length != 16)
            {
                Debug.LogError("AES Initial-Vector invalid length, length = " + m_iv.Length + " bytes, length of 16 bytes was expected");
            }
        }

        private byte[] m_key;
        private byte[] m_iv;

        public byte[] Encrypt(string plainText)
        {
            return EncryptStringToBytes_Aes(plainText, m_key, m_iv);
        }

        public string Decrypt(byte[] cipherText)
        {
            return DecryptStringFromBytes_Aes(cipherText, m_key, m_iv);
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("FileIO.Encryptor.plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("FileIO.Encryptor.Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("FileIO.Encryptor.IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("FileIO.Encryptor.cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("FileIO.Encryptor.Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("FileIO.Encryptor.IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}