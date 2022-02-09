using System.Security.Cryptography;
using System.Text;

namespace PasswordCrackerBackend.Services
{
    public class PasswordService
    {
        private const string pw0 = "A746222F09D85605C52D4E636788D6FFDC274698B98B8C5F3244C06958683A69";
        private const string pw1 = "3086E346353248775A2C5D74E36A9C9B9BD226A1EE401F830AC499633DC00031";
        private const string pw2 = "26775436073E00D207E192857EE3730CFCA19DE16F01F0780096EF217C2919EF";
        private const string pw3 = "43C19A093B34B581DDCC7207F6BD094F6940DB69F035C444425ED84D2CAC037D";


        public async Task<string> CrackPassword(string hashCode, string possible, int length)
        {
            var possibleChars = possible.ToCharArray();
            int[] indexArray = new int[length];
            int currIndex = 0;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                while (true)
                {
                    var currTry = "";
                    for (int i = 0; i < length; i++)
                    {
                        
                    }

                    byte[] pwHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(currTry));
                    if (pwHash.ToString().ToLower().Equals(hashCode.ToLower()))
                    {
                        Console.Write(currTry);
                        return currTry;
                    }

                    currIndex++;
                }
            }

            return "* no match *";
        }
    }
}
