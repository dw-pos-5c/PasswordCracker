using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;

namespace PasswordCrackerFrontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5095/Cracker")
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
                .Build();

            ConnectToHub();
        }

        private async void ConnectToHub()
        {
            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                LblError.Content = "Could not connect to Hub.";
            }

            if (connection.State != HubConnectionState.Connected)
            {
                LblError.Content = "Could not connect to Hub.";
            }
        }

        private async void CrackPassword(object sender, RoutedEventArgs e)
        {
            LblResult.Content = "Starting...";
            ProgressBar.Value = 0;

            var hashCode = TbHashcode.Text;
            var alphabet = TbAlphabet.Text;
            var sLength = TbLength.Text;
            var parsedLength = int.TryParse(sLength, out int length);

            if (hashCode.Length == 0 || (sLength.Length != 0 && !parsedLength))
            {
                LblError.Content = "Hashcode has to be provided";
                return;
            }

            connection.On<double>("ProgressChanged", (val) =>
            {
                ProgressBar.Value = val * 100;
                LblResult.Content = $"{Math.Round(val * 100)}%";
            });

            LblError.Content = "";

            TbHashcode.IsEnabled = false;
            TbAlphabet.IsEnabled = false;
            TbLength.IsEnabled = false;
            BtnCrackPassword.IsEnabled = false;

            var result = await connection.InvokeAsync<string>("CrackPassword", hashCode, alphabet, length);
            LblResult.Content = result;

            TbHashcode.IsEnabled = true;
            TbAlphabet.IsEnabled = true;
            TbLength.IsEnabled = true;
            BtnCrackPassword.IsEnabled = true;

            connection.Remove("ProgressChanged");

            ProgressBar.Value = 100;
        }
    }
}
