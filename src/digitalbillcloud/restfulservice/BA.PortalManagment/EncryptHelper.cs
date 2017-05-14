using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA.PortalManagment
{
    public class EncryptHelper
    {
        // 验值       
        static string saltValue = "ufida2012";
        // 密码值      
        static string pwdValue = "yongyou2012";

        public static string Encrypt(string input)
        {
            try
            {
                byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(input);
                byte[] salt = System.Text.UTF8Encoding.UTF8.GetBytes(saltValue);
                // AesManaged - 高级加密标准(AES) 对称算法的管理类             
                System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
                // Rfc2898DeriveBytes - 通过使用基于 HMACSHA1 的伪随机数生成器，实现基于密码的密钥派生功能 (PBKDF2 - 一种基于密码的密钥派生函数)             // 通过 密码 和 salt 派生密钥             
                System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(pwdValue, salt);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                // 用当前的 Key 属性和初始化向量 IV 创建对称加密器对象            
                System.Security.Cryptography.ICryptoTransform encryptTransform = aes.CreateEncryptor();
                // 加密后的输出流            
                System.IO.MemoryStream encryptStream = new System.IO.MemoryStream();
                // 将加密后的目标流（encryptStream）与加密转换（encryptTransform）相连接            
                System.Security.Cryptography.CryptoStream encryptor = new System.Security.Cryptography.CryptoStream(encryptStream, encryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);
                // 将一个字节序列写入当前 CryptoStream （完成加密的过程）            
                encryptor.Write(data, 0, data.Length);
                encryptor.Close();
                // 将加密后所得到的流转换成字节数组，再用Base64编码将其转换为字符串            
                string encryptedString = Convert.ToBase64String(encryptStream.ToArray());

                return encryptedString;
            }
            catch
            {

            }
            return input;
        }


        public static string Decrypt(string input)
        {
            try
            {
                if (input.Length <= 15)
                    return input;

                byte[] encryptBytes = Convert.FromBase64String(input);
                byte[] salt = Encoding.UTF8.GetBytes(saltValue);
                System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
                System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(pwdValue, salt);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                // 用当前的 Key 属性和初始化向量 IV 创建对称解密器对象            
                System.Security.Cryptography.ICryptoTransform decryptTransform = aes.CreateDecryptor();
                // 解密后的输出流             
                System.IO.MemoryStream decryptStream = new System.IO.MemoryStream();
                // 将解密后的目标流（decryptStream）与解密转换（decryptTransform）相连接             
                System.Security.Cryptography.CryptoStream decryptor = new System.Security.Cryptography.CryptoStream(decryptStream, decryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);
                // 将一个字节序列写入当前 CryptoStream （完成解密的过程）            
                decryptor.Write(encryptBytes, 0, encryptBytes.Length);
                decryptor.Close();
                // 将解密后所得到的流转换为字符串           
                byte[] decryptBytes = decryptStream.ToArray();
                string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                return decryptedString;
            }
            catch
            {

            }
            return input;
        }

        public static string Encrypt(string input, string pwdValue)
        {
            try
            {
                byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(input);
                byte[] salt = System.Text.UTF8Encoding.UTF8.GetBytes(saltValue);
                // AesManaged - 高级加密标准(AES) 对称算法的管理类             
                System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
                // Rfc2898DeriveBytes - 通过使用基于 HMACSHA1 的伪随机数生成器，实现基于密码的密钥派生功能 (PBKDF2 - 一种基于密码的密钥派生函数)             // 通过 密码 和 salt 派生密钥             
                System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(pwdValue, salt);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                // 用当前的 Key 属性和初始化向量 IV 创建对称加密器对象            
                System.Security.Cryptography.ICryptoTransform encryptTransform = aes.CreateEncryptor();
                // 加密后的输出流            
                System.IO.MemoryStream encryptStream = new System.IO.MemoryStream();
                // 将加密后的目标流（encryptStream）与加密转换（encryptTransform）相连接            
                System.Security.Cryptography.CryptoStream encryptor = new System.Security.Cryptography.CryptoStream(encryptStream, encryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);
                // 将一个字节序列写入当前 CryptoStream （完成加密的过程）            
                encryptor.Write(data, 0, data.Length);
                encryptor.Close();
                // 将加密后所得到的流转换成字节数组，再用Base64编码将其转换为字符串            
                string encryptedString = Convert.ToBase64String(encryptStream.ToArray());

                return encryptedString;
            }
            catch
            {

            }
            return input;
        }


        public static string Decrypt(string input, string pwdValue)
        {
            try
            {
                if (input.Length <= 15)
                    return input;

                byte[] encryptBytes = Convert.FromBase64String(input);
                byte[] salt = Encoding.UTF8.GetBytes(saltValue);
                System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
                System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(pwdValue, salt);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                // 用当前的 Key 属性和初始化向量 IV 创建对称解密器对象            
                System.Security.Cryptography.ICryptoTransform decryptTransform = aes.CreateDecryptor();
                // 解密后的输出流             
                System.IO.MemoryStream decryptStream = new System.IO.MemoryStream();
                // 将解密后的目标流（decryptStream）与解密转换（decryptTransform）相连接             
                System.Security.Cryptography.CryptoStream decryptor = new System.Security.Cryptography.CryptoStream(decryptStream, decryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);
                // 将一个字节序列写入当前 CryptoStream （完成解密的过程）            
                decryptor.Write(encryptBytes, 0, encryptBytes.Length);
                decryptor.Close();
                // 将解密后所得到的流转换为字符串           
                byte[] decryptBytes = decryptStream.ToArray();
                string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                return decryptedString;
            }
            catch
            {

            }
            return input;
        }
    }
}
