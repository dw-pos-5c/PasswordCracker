using System.Timers;
using Microsoft.AspNetCore.SignalR;
using PasswordCrackerBackend.Services;

namespace PasswordCrackerBackend.Hubs
{
    public class PasswordHub : Hub<IPasswordHub>
    {
        public async Task<string> CrackPassword(string hashCode, string possible, int length)
        {
            var passwordService = new PasswordService();
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (_, _) =>
            {
                Clients.All.ProgressChanged(passwordService.CalcProgress());
            };
            timer.Start();

            string result;
            if (possible.Length == 0 || length <= 0)
            {
                result = passwordService.TryPassword(hashCode).Result;
            }
            else
            {
                result = passwordService.CrackPassword(hashCode, possible, length).Result;
            }

            timer.Dispose();
            return result;
        }
    }
}
