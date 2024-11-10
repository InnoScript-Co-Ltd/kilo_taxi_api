using KiloTaxi.Model.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simulator.Dashbaord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        Settings appSettings = new Settings();
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Signar Clients Events
        private void ReceiveLocationData(VehicleLocation vehicleLocation)
        {
            var jsonSerializedModel = JsonSerializer.Serialize(vehicleLocation);
            lblLogs.Text += Environment.NewLine + $"ReceiveLocationData : {jsonSerializedModel}";
        }
        #endregion

        #region Form Events
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            SetUpSignaRCLient();
        }
        private void btnClearLogs_Click(object sender, RoutedEventArgs e)
        {
            lblLogs.Text = "";
        }

        private void btnRequestVehicleLocation_Click(object sender, RoutedEventArgs e)
        {
            connection.InvokeAsync("RequestVehicleLocation", txtVehicleId.Text);
            lblLogs.Text += Environment.NewLine + "RequestVehicleLocation has been invoked";
        }

        #endregion

        #region Private Methods
        private async Task SetUpSignaRCLient()
        {
            connection = new HubConnectionBuilder()
            .WithUrl(appSettings.DashboardHubUrl)
            .WithAutomaticReconnect()
            .Build();

            connection.Reconnecting += error =>
            {
                lblLogs.Text += Environment.NewLine + "Reconnecting due to error: " + error.Message;
                // Optionally inform the user
                return Task.CompletedTask;
            };

            connection.Reconnected += connectionId =>
            {
                lblLogs.Text += Environment.NewLine + "Reconnected with new connection ID: " + connectionId;
                // Re-establish user-specific setup if necessary
                return Task.CompletedTask;
            };

            SubscribeSignalrEvents();

            await connection.StartAsync().ContinueWith(task =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (task.IsCompletedSuccessfully && connection.State == HubConnectionState.Connected)
                    {

                        btnConnect.IsEnabled = false;
                        btnConnect.Content = "Connected";
                        lblLogs.Text += Environment.NewLine + "SignalR client successfully connected.";


                    }
                    else if (task.IsFaulted)
                    {
                        lblLogs.Text += Environment.NewLine + "Failed to connect to SignalR: " + task.Exception?.GetBaseException().Message;
                    }
                    else
                    {
                        lblLogs.Text += Environment.NewLine + "SignalR client is not connected.";
                    }
                });
            });

        }
        private void SubscribeSignalrEvents()
        {
            connection.On("ReceiveLocationData", async (VehicleLocation vehicleLocation) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    ReceiveLocationData(vehicleLocation);
                });
            });

        }
        #endregion


    }
}