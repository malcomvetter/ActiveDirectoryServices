using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace System.DirectoryServices.AccountManagement
{
    public class Crypto
    {
        #region CiphertextFormat enum

        public enum CiphertextFormat
        {
            Base64,
            Hex,
            Url,
        }

        #endregion

        #region EncryptionAlgorithm enum

        public enum EncryptionAlgorithm
        {
            Aes128Cbc,
            Aes128Ecb,
            Aes128Cfb,
            Aes128Cts,
            Aes128Ofb,
            Aes256Cbc,
            Aes256Ecb,
            Aes256Cfb,
            Aes256Cts,
            Aes256Ofb,
        }

        #endregion

        #region KeySize enum

        public enum KeySize
        {
            _128bits,
            _256bits
        }

        #endregion

        public static string GetSha256(string text)
        {
            var ue = new UTF8Encoding();
            var message = ue.GetBytes(text);
            var hex = string.Empty;
            var hashString = new SHA256Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate(hex, (current, x) => current + String.Format("{0:x2}", x));
        }

        public static string GetSha1(string text)
        {
            var ue = new UTF8Encoding();
            var message = ue.GetBytes(text);
            var hex = string.Empty;
            var hashString = new SHA1Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate(hex, (current, x) => current + String.Format("{0:x2}", x));
        }

        public static string GetSha384(string text)
        {
            var ue = new UTF8Encoding();
            var message = ue.GetBytes(text);
            var hex = string.Empty;
            var hashString = new SHA384Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate(hex, (current, x) => current + String.Format("{0:x2}", x));
        }

        public static string GetSha512(string text)
        {
            var ue = new UTF8Encoding();
            var message = ue.GetBytes(text);
            var hex = string.Empty;
            var hashString = new SHA512Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate(hex, (current, x) => current + String.Format("{0:x2}", x));
        }

        public static string GetRIPEMD160(string text)
        {
            var ue = new UTF8Encoding();
            var message = ue.GetBytes(text);
            var hex = string.Empty;
            var hashString = new RIPEMD160Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate(hex, (current, x) => current + String.Format("{0:x2}", x));
        }

        public static string EncryptData(string data, string password)
        {
            return EncryptData(data, password, CiphertextFormat.Base64);
        }

        /// <summary>
        ///     Use AES to encrypt data string. The output string is the encrypted bytes as a base64 string.
        ///     The same password must be used to decrypt the string.
        /// </summary>
        /// <param name="data">Clear string to encrypt.</param>
        /// <param name="password">Password used to encrypt the string.</param>
        /// <param name="format">Output string format.</param>
        /// <returns>Encrypted result as Base64 string.</returns>
        public static string EncryptData(string data, string password, CiphertextFormat format)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            var encBytes = EncryptData(Encoding.UTF8.GetBytes(data), password, PaddingMode.ISO10126);
            if (format == CiphertextFormat.Hex)
            {
                return BitConverter.ToString(encBytes).Replace("-", "");
            }
            return format == CiphertextFormat.Base64 ? Convert.ToBase64String(encBytes) : encBytes.ToString();
        }

        public static byte[] HexToByteArray(String hex)
        {
            var numberChars = hex.Length;
            if (numberChars%2 != 0)
            {
                throw new FormatException("Hex string must have an even number of characters");
            }
            var bytes = new byte[numberChars/2];
            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string DecryptData(string data, string password)
        {
            return DecryptData(data, password, CiphertextFormat.Base64);
        }

        /// <summary>
        ///     Decrypt the data string to the original string.  The data must be the base64 string
        ///     returned from the EncryptData method.
        /// </summary>
        /// <param name="data">Encrypted data generated from EncryptData method.</param>
        /// <param name="password">Password used to decrypt the string.</param>
        /// <returns>Decrypted string.</returns>
        public static string DecryptData(string data, string password, CiphertextFormat format)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            var encBytes = new byte[0];
            if (format == CiphertextFormat.Base64)
            {
                encBytes = Convert.FromBase64String(data);
            }
            if (format == CiphertextFormat.Hex)
            {
                encBytes = HexToByteArray(data);
            }
            var decBytes = DecryptData(encBytes, password, PaddingMode.ISO10126);
            return Encoding.UTF8.GetString(decBytes);
        }

        public static byte[] EncryptData(byte[] data, string password, PaddingMode paddingMode)
        {
            return EncryptData(data, password, "Salt", paddingMode, CipherMode.CBC);
        }

        public static byte[] EncryptData(byte[] data, string password, string salt, PaddingMode paddingMode,
            CipherMode cipherMode)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException("data");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (salt == null)
            {
                salt = string.Empty;
            }
            var pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes(salt));
            var rm = new RijndaelManaged {Mode = cipherMode, Padding = paddingMode};
            var encryptor = rm.CreateEncryptor(pdb.GetBytes(16), pdb.GetBytes(16));
            using (var msEncrypt = new MemoryStream())
            {
                using (var encStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    encStream.Write(data, 0, data.Length);
                    encStream.FlushFinalBlock();
                    return msEncrypt.ToArray();
                }
            }
        }

        public static byte[] DecryptData(byte[] data, string password, PaddingMode paddingMode)
        {
            return DecryptData(data, password, "Salt", paddingMode, CipherMode.CBC);
        }

        public static byte[] DecryptData(byte[] data, string password, string salt, PaddingMode paddingMode,
            CipherMode cipherMode)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException("data");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (salt == null)
            {
                salt = string.Empty;
            }
            var pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes(salt));
            var rm = new RijndaelManaged {Mode = cipherMode, Padding = paddingMode};
            var decryptor = rm.CreateDecryptor(pdb.GetBytes(16), pdb.GetBytes(16));
            using (var msDecrypt = new MemoryStream(data))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    // Decrypted bytes will always be less then encrypted bytes, so len of encrypted data will be big enouph for buffer.
                    var fromEncrypt = new byte[data.Length];
                    // Read as many bytes as possible.
                    var read = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                    if (read < fromEncrypt.Length)
                    {
                        // Return a byte array of proper size.
                        var clearBytes = new byte[read];
                        Buffer.BlockCopy(fromEncrypt, 0, clearBytes, 0, read);
                        return clearBytes;
                    }
                    return fromEncrypt;
                }
            }
        }

        public static string Encrypt(EncryptionAlgorithm algorithm, CiphertextFormat format, string data, Key key,
            byte[] salt, byte[] iv, PaddingMode paddingMode)
        {
            var _data = Encoding.UTF8.GetBytes(data);
            switch (format)
            {
                case CiphertextFormat.Base64:
                {
                    return Convert.ToBase64String(Encrypt(algorithm, _data, key, salt, iv, paddingMode));
                }
                case CiphertextFormat.Hex:
                {
                    return BitConverter.ToString(Encrypt(algorithm, _data, key, salt, iv, paddingMode)).Replace("-", "");
                }
                case CiphertextFormat.Url:
                {
                    return HttpUtility.UrlEncode(Encrypt(algorithm, _data, key, salt, iv, paddingMode));
                }
            }
            return string.Empty;
        }

        public static byte[] Encrypt(EncryptionAlgorithm algorithm, byte[] data, Key key, byte[] salt, byte[] iv,
            PaddingMode paddingMode)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException("data");
            }
            if (key == null || key.Bytes.Length == 0)
            {
                throw new ArgumentNullException("password");
            }
            var encryptor = new RijndaelManaged().CreateEncryptor();
            var _enc = new byte[0];

            switch (algorithm)
            {
                case EncryptionAlgorithm.Aes128Cbc:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.CBC, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Ecb:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.ECB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Cfb:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.CFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Cts:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.CTS, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Ofb:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.OFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Cbc:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.CBC, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Ecb:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.ECB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Cfb:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.CFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Cts:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.CTS, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Ofb:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.OFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    encryptor = cipher.CreateEncryptor();
                    break;
                }
            }
            using (var msEncrypt = new MemoryStream())
            {
                using (var encStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    encStream.Write(data, 0, data.Length);
                    encStream.FlushFinalBlock();
                    _enc = msEncrypt.ToArray();
                }
            }
            return _enc;
        }

        public static string Decrypt(EncryptionAlgorithm algorithm, CiphertextFormat format, string data, Key key,
            byte[] salt, byte[] iv, PaddingMode paddingMode)
        {
            var _d = new byte[0];
            switch (format)
            {
                case CiphertextFormat.Base64:
                {
                    _d = Convert.FromBase64String(data);
                    break;
                }
                case CiphertextFormat.Hex:
                {
                    _d = HexToByteArray(data);
                    break;
                }
                case CiphertextFormat.Url:
                {
                    var encoding = new UTF8Encoding();
                    _d = encoding.GetBytes(HttpUtility.UrlDecode(data));
                    break;
                }
            }
            return Decrypt(algorithm, _d, key, salt, iv, paddingMode);
        }

        public static string Decrypt(EncryptionAlgorithm algorithm, byte[] data, Key key, byte[] salt, byte[] iv,
            PaddingMode paddingMode)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException("data");
            }
            if (key == null || key.Bytes.Count() == 0)
            {
                throw new ArgumentNullException("key");
            }
            var decryptor = new RijndaelManaged().CreateDecryptor();
            switch (algorithm)
            {
                case EncryptionAlgorithm.Aes128Cbc:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.CBC, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Ecb:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.ECB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Cfb:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.CFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Cts:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.CTS, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes128Ofb:
                {
                    var cipher = new RijndaelManaged {KeySize = 128, Mode = CipherMode.OFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Cbc:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.CBC, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Ecb:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.ECB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Cfb:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.CFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Cts:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.CTS, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
                case EncryptionAlgorithm.Aes256Ofb:
                {
                    var cipher = new RijndaelManaged {KeySize = 256, Mode = CipherMode.OFB, Padding = paddingMode};
                    if (iv == null)
                    {
                        iv = new byte[16];
                    }
                    cipher.IV = iv;
                    cipher.Key = key.Bytes;
                    decryptor = cipher.CreateDecryptor();
                    break;
                }
            }
            using (var msDecrypt = new MemoryStream(data))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    // Decrypted bytes will always be less then encrypted bytes, so length of encrypted data will be big enough for buffer.
                    var fromEncrypt = new byte[data.Length];
                    // Read as many bytes as possible.
                    var read = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                    if (read < fromEncrypt.Length)
                    {
                        // Return a byte array of proper size.
                        var clearBytes = new byte[read];
                        Buffer.BlockCopy(fromEncrypt, 0, clearBytes, 0, read);
                        return Encoding.UTF8.GetString(clearBytes);
                    }
                    return Encoding.UTF8.GetString(fromEncrypt);
                }
            }
        }

        #region Nested type: Key

        [XmlRoot("Key")]
        public class Key : IComparable
        {
            protected byte[] _bytes;

            public Key()
            {
            }

            public Key(string password, KeySize s)
            {
                GenerateFromPassword(password, s);
            }

            public Key(string s, CiphertextFormat format)
            {
                switch (format)
                {
                    case CiphertextFormat.Hex:
                    {
                        GenerateFromHex(s);
                        break;
                    }
                    case CiphertextFormat.Base64:
                    {
                        GenerateFromBase64(s);
                        break;
                    }
                }
            }

            public Key(byte[] bytes)
            {
                GenerateFromBytes(bytes);
            }

            [XmlIgnore]
            public byte[] Bytes
            {
                get { return _bytes; }
                set
                {
                    var bits = value.Length*8;
                    if (bits == 128/8 || bits == 256/8)
                    {
                        _bytes = value;
                    }
                    else
                    {
                        throw new FormatException("Invalid Key Size");
                    }
                }
            }

            public int Size
            {
                get { return _bytes.Count()*8; }
            }

            public string Hex
            {
                get
                {
                    if (_bytes != null && _bytes.Length > 0)
                    {
                        return BitConverter.ToString(_bytes).Replace("-", "");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                set
                {
                    var _b = HexToByteArray(value);
                    var _s = _b.Count();
                    if (_s != 128/8 && _s != 256/8)
                    {
                        throw new FormatException("Hex string is not a valid key size");
                    }
                    _bytes = _b;
                }
            }

            public string Base64
            {
                get
                {
                    if (_bytes != null && _bytes.Length > 0)
                    {
                        return Convert.ToBase64String(_bytes);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                set
                {
                    var _b = Convert.FromBase64String(value);
                    var _s = _b.Count();
                    if (_s != 128/8 && _s != 256/8)
                    {
                        throw new FormatException("Base64 string is not a valid key size");
                    }
                    _bytes = _b;
                }
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                var result = 1;
                if (obj != null)
                {
                    var key = obj as Key;
                    if (key != null)
                    {
                        var k = key;
                        if (k._bytes != null && k._bytes.Count() > 0)
                        {
                            result = Hex.CompareTo(k.Hex);
                        }
                    }
                    var bytes = obj as byte[];
                    if (bytes != null)
                    {
                        var k = new Key(bytes);
                        if (k._bytes != null && k._bytes.Count() > 0)
                        {
                            result = Hex.CompareTo(k.Hex);
                        }
                    }
                }
                return result;
            }

            #endregion

            public void GenerateFromPassword(string password, KeySize s)
            {
                var _s = 0;
                switch (s)
                {
                    case KeySize._128bits:
                    {
                        _s = 128/8;
                        break;
                    }
                    case KeySize._256bits:
                    {
                        _s = 256/8;
                        break;
                    }
                }
                var _salt = new byte[8];
                for (var i = 0; i < 8; i++)
                {
                    _salt[i] = 0;
                }
                _bytes = new Rfc2898DeriveBytes(password, _salt).GetBytes(_s);
            }

            public void GenerateFromHex(string hex)
            {
                var _b = HexToByteArray(hex);
                var _s = _b.Count();
                if (_s != 128/8 && _s != 256/8)
                {
                    throw new FormatException("Hex string is not a valid key size");
                }
                _bytes = _b;
            }

            public void GenerateFromBase64(string base64)
            {
                var _b = Convert.FromBase64String(base64);
                var _s = _b.Count();
                if (_s != 128/8 && _s != 256/8)
                {
                    throw new FormatException("Base64 string is not a valid key size");
                }
                _bytes = _b;
            }

            public void GenerateFromBytes(byte[] bytes)
            {
                var _s = bytes.Count();
                if (_s != 128/8 && _s != 256/8)
                {
                    throw new FormatException("Byte[] is not a valid key size");
                }
                _bytes = bytes;
            }

            public override bool Equals(Object obj)
            {
                if (obj != null)
                {
                    var key = obj as Key;
                    if (key != null)
                    {
                        var k = key;
                        if (k._bytes != null)
                        {
                            return (k.Hex == Hex);
                        }
                    }
                    var bytes = obj as byte[];
                    if (bytes != null)
                    {
                        var k = new Key(bytes);
                        if (k._bytes != null)
                        {
                            return (k.Hex == Hex);
                        }
                    }
                }
                return false;
            }

            public override string ToString()
            {
                return _bytes.Count() > 0 ? Hex : string.Empty;
            }
        }

        #endregion
    }
}