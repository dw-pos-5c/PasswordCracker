using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace PasswordCrackerBackend.Services
{
    public class PasswordService
    {
        // "A746222F09D85605C52D4E636788D6FFDC274698B98B8C5F3244C06958683A69";
        // "3086E346353248775A2C5D74E36A9C9B9BD226A1EE401F830AC499633DC00031";
        // "26775436073E00D207E192857EE3730CFCA19DE16F01F0780096EF217C2919EF";
        // "43C19A093B34B581DDCC7207F6BD094F6940DB69F035C444425ED84D2CAC037D";

        private int noThreads = Environment.ProcessorCount;

        private string result = "* no match *";
        private bool running = true;

        private byte[] possible;
        private byte[] hashCode;
        private int threadLength;

        private int[] valuesCalcedList;
        private long totalValuesToCalc;

        public async Task<string> CrackPassword(string hashCode, string possible, int length)
        {
            this.possible = Encoding.UTF8.GetBytes(possible);
            this.hashCode = HexStringToByte(hashCode);
            threadLength = length - 1;

            totalValuesToCalc = (long) Math.Pow(possible.Length, length);

            List<Thread> threads = new List<Thread>();

            int charsPerThread = possible.Length / noThreads;

            var lastBytesArray = this.possible.Split(charsPerThread, possible.Length % noThreads).ToArray();

            valuesCalcedList = new int[lastBytesArray.Length];

            int tempIndex = 0;
            foreach (var lastBytes in lastBytesArray)
            {
                int bled = tempIndex;
                Console.WriteLine(Encoding.UTF8.GetString(lastBytes));
                var thread = new Thread(() =>
                {
                    Crack(lastBytes, out valuesCalcedList[bled]);
                });
                threads.Add(thread);
                thread.Start();
                tempIndex++;
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }

        private Task Crack(byte[] lastBytes, out int valuesCalced)
        {
            var indexes = new int[threadLength];
            valuesCalced = 0;

            using (var sha256Hash = SHA256.Create())
            {
                var pw = new byte[threadLength + 1];

                for (var i = 0; i < threadLength; i++)
                {
                    pw[i] = possible[0];
                }

                while (running)
                {
                    foreach (byte b in lastBytes)
                    {
                        pw[^1] = b;
                        byte[] pwHash = sha256Hash.ComputeHash(pw);
                        if (pwHash.SequenceEqual(hashCode))
                        {
                            result = Encoding.UTF8.GetString(pw);
                            running = false;
                            return Task.CompletedTask;
                        }

                        valuesCalced++;
                    }

                    indexes[0]++;

                    for (var i = 0; i < indexes.Length; i++)
                    {
                        if (indexes[i] == possible.Length)
                        {
                            if (i == threadLength - 1)
                            {
                                return Task.CompletedTask;
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
            
            return Task.CompletedTask;
        }

        public async Task<string> TryPassword(string hashCode)
        {
            //*[@id="mw-content-text"]/div[1]/ul[1]/li[1]/a

            var hashCodeBytes = HexStringToByte(hashCode);

            valuesCalcedList = new int[1];

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://de.wikipedia.org/wiki/Liste_von_Fabelwesen");

            var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/div/ul/li/a");
            totalValuesToCalc = nodes.Count;
            var passwordsToTry = nodes.Select(x => x.InnerText);
            using (var sha256Hash = SHA256.Create())
            {
                foreach (var password in passwordsToTry)
                {
                    byte[] pwHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                    if (pwHash.SequenceEqual(hashCodeBytes))
                    {
                        result = password;
                        return result;
                    }

                    valuesCalcedList[0]++;
                }
            }

            return result;
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

        public double CalcProgress()
        {
            long totalValuesCalced = 0;
            foreach (var valuesCalced in valuesCalcedList)
            {
                totalValuesCalced += valuesCalced;
            }

            return (double) totalValuesCalced / totalValuesToCalc;
        }
    }
}
