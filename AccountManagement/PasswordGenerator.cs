using System.Security.Cryptography;
using System.Text;

namespace System.DirectoryServices.AccountManagement
{
    public class PasswordGenerator
    {
        private const int DefaultMinimum = 6;
        private const int DefaultMaximum = 10;
        private const int UBoundDigit = 61;

        private readonly char[] pwdCharArray =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?"
                .ToCharArray();

        private readonly RNGCryptoServiceProvider rng;
        private int maxSize;
        private int minSize;

        public PasswordGenerator()
        {
            Minimum = DefaultMinimum;
            Maximum = DefaultMaximum;
            ConsecutiveCharacters = false;
            RepeatCharacters = true;
            ExcludeSymbols = false;
            Exclusions = null;
            rng = new RNGCryptoServiceProvider();
        }

        public PasswordGenerator(int min, int max, bool consecutiveAllowed, bool repeatsAllowed, bool symbolsAllowed,
            string excludedChars)
        {
            Minimum = min;
            Maximum = max;
            ConsecutiveCharacters = consecutiveAllowed;
            RepeatCharacters = repeatsAllowed;
            ExcludeSymbols = !symbolsAllowed;
            Exclusions = excludedChars;
            rng = new RNGCryptoServiceProvider();
        }

        public string Exclusions { get; set; }

        public int Minimum
        {
            get { return minSize; }
            set
            {
                minSize = value;
                if (DefaultMinimum > minSize)
                {
                    minSize = DefaultMinimum;
                }
            }
        }

        public int Maximum
        {
            get { return maxSize; }
            set
            {
                maxSize = value;
                if (minSize > maxSize)
                {
                    maxSize = minSize;
                }
            }
        }

        public bool ExcludeSymbols { get; set; }

        public bool RepeatCharacters { get; set; }

        public bool ConsecutiveCharacters { get; set; }

        protected int GetCryptographicRandomNumber(int lBound, int uBound)
        {
            // Assumes lBound >= 0 && lBound < uBound
            // returns an int >= lBound and < uBound
            uint urndnum;
            var rndnum = new Byte[4];
            if (lBound == uBound - 1)
            {
                // test for degenerate case where only lBound can be returned   
                return lBound;
            }

            var xcludeRndBase = (uint.MaxValue - (uint.MaxValue%(uint) (uBound - lBound)));

            do
            {
                rng.GetBytes(rndnum);
                urndnum = BitConverter.ToUInt32(rndnum, 0);
            } while (urndnum >= xcludeRndBase);

            return (int) (urndnum%(uBound - lBound)) + lBound;
        }

        protected char GetRandomCharacter()
        {
            var upperBound = pwdCharArray.GetUpperBound(0);

            if (ExcludeSymbols)
            {
                upperBound = UBoundDigit;
            }

            var randomCharPosition = GetCryptographicRandomNumber(pwdCharArray.GetLowerBound(0), upperBound);

            var randomChar = pwdCharArray[randomCharPosition];

            return randomChar;
        }

        public string Generate()
        {
            // Pick random length between minimum and maximum   
            var pwdLength = Minimum == Maximum ? Maximum : GetCryptographicRandomNumber(Minimum, Maximum);

            var pwdBuffer = new StringBuilder {Capacity = Maximum};

            // Generate random characters
            char nextCharacter;

            // Initial dummy character flag
            var lastCharacter = nextCharacter = '\n';

            for (var i = 0; i < pwdLength; i++)
            {
                nextCharacter = GetRandomCharacter();

                if (!ConsecutiveCharacters)
                {
                    while (lastCharacter == nextCharacter)
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }

                if (!RepeatCharacters)
                {
                    var temp = pwdBuffer.ToString();
                    var duplicateIndex = temp.IndexOf(nextCharacter);
                    while (-1 != duplicateIndex)
                    {
                        nextCharacter = GetRandomCharacter();
                        duplicateIndex = temp.IndexOf(nextCharacter);
                    }
                }

                if (!string.IsNullOrEmpty(Exclusions))
                {
                    while (-1 != Exclusions.IndexOf(nextCharacter))
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }

                pwdBuffer.Append(nextCharacter);
                lastCharacter = nextCharacter;
            }

            return pwdBuffer.ToString();
        }
    }
}