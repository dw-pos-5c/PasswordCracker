using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using PasswordCrackerBackend.Controllers;

namespace PasswordCrackerBackend.Services
{
    public class PasswordService
    {
        private const string pw0 = "A746222F09D85605C52D4E636788D6FFDC274698B98B8C5F3244C06958683A69";
        private const string pw1 = "3086E346353248775A2C5D74E36A9C9B9BD226A1EE401F830AC499633DC00031";
        private const string pw2 = "26775436073E00D207E192857EE3730CFCA19DE16F01F0780096EF217C2919EF";
        private const string pw3 = "43C19A093B34B581DDCC7207F6BD094F6940DB69F035C444425ED84D2CAC037D";

        private int noThreads = Environment.ProcessorCount;

        private string result = "* no match *";
        private bool finished = false;

        public async Task<string> CrackPassword(string hashCode, string possible, int length)
        {
            var possibleBytes = Encoding.UTF8.GetBytes(possible);
            var hashCodeBytes = HexStringToByte(hashCode);

            List<Thread> threads = new List<Thread>();

            int charsPerThread = possible.Length / noThreads;

            var splitArrays = possibleBytes.Split(charsPerThread, possible.Length % noThreads).ToArray();

            foreach (var msbArray in splitArrays)
            {
                var threadStart = new ThreadStart(() => Crack(hashCodeBytes, possibleBytes, length - 1, msbArray));
                var thread = new Thread(threadStart);
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }

        private async Task Crack(byte[] hashCode, byte[] possible, int length, byte[] msb)
        {
            var indexes = new int[length];

            using (var sha256Hash = SHA256.Create())
            {
                var pw = new byte[length + 1];

                for (var i = 0; i < length; i++)
                {
                    pw[i] = possible[0];
                }

                while (!finished)
                {
                    foreach (byte b in msb)
                    {
                        pw[^1] = b;
                        byte[] pwHash = sha256Hash.ComputeHash(pw);
                        if (pwHash.SequenceEqual(hashCode))
                        {
                            result = Encoding.UTF8.GetString(pw);
                            finished = true;
                            return;
                        }
                    }

                    indexes[0]++;

                    for (var i = 0; i < indexes.Length; i++)
                    {
                        if (indexes[i] == possible.Length)
                        {
                            if (i == length - 1)
                            {
                                return;
                            }
                            indexes[i + 1]++;
                            indexes[i] = 0;
                            pw[i] = possible[indexes[i]];
                        }
                        else
                        {
                            pw[i] = possible[indexes[i]];
                            break;
                        }
                    }
                }
            }
        }

        // Credits to @Michael Wiesinger
        public static byte[] HexStringToByte(string hex)
        {
            var data = new byte[hex.Length / 2];
            for (var i = 0; i < data.Length; i++)
            {
                string byteValue = hex.Substring(i * 2, 2);
                data[i] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
