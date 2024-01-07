using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IoTFeeder.Common.Helpers
{
    public class Encryption
    {
        protected static string strKey = "IoTFeeder";

        public static string Encrypt(string textToBeEncrypted)
        {
            if (textToBeEncrypted == string.Empty || textToBeEncrypted == null)
            {
                return textToBeEncrypted;
            }

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            string password = strKey;
            byte[] plainText = System.Text.Encoding.Unicode.GetBytes(textToBeEncrypted);
            byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(password, salt);

            //Creates a symmetric encryptor object.
            ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();

            //Defines a stream that links data streams to cryptographic transformations
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainText, 0, plainText.Length);

            //Writes the final state and clears the buffer
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string encryptedData = Convert.ToBase64String(cipherBytes);

            return encryptedData;
        }

        public static string Decrypt(string textToBeDecrypted)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            string password = strKey;
            string decryptedData;

            try
            {
                if (!string.IsNullOrWhiteSpace(textToBeDecrypted))
                {
                    byte[] encryptedData = Convert.FromBase64String(textToBeDecrypted.Replace(' ', '+'));
                    byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());

                    //Making of the key for decryption
                    PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);

                    //Creates a symmetric Rijndael decryptor object.
                    ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
                    MemoryStream memoryStream = new MemoryStream(encryptedData);

                    //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                    byte[] plainText = new byte[encryptedData.Length];
                    int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                    memoryStream.Close();
                    cryptoStream.Close();

                    //Converting to string
                    decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                }
                else
                {
                    decryptedData = textToBeDecrypted;
                }
            }
            catch
            {
                decryptedData = textToBeDecrypted;
            }

            return decryptedData;
        }

        public static string EncryptWithUrlEncode(string str)
        {
            return HttpUtility.UrlEncode(Encrypt(str));
        }
        public static string DecryptWithUrlDecode(string str)
        {
            return Decrypt(HttpUtility.UrlDecode(str));
        }
    }
}
