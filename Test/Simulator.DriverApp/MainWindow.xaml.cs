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

namespace Simulator.DriverApp
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
        private void RequestVehicleLocation(string vehicleId)
        {
            VehicleLocation vehicleLocation = new VehicleLocation() { 
                VehicleId = vehicleId,
                Lat ="123.45",
                Long = "678.98"
            };
            connection.InvokeAsync("SendVehicleLocation", vehicleLocation);
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
        #endregion

        #region Private Methods
        private async Task SetUpSignaRCLient()
        {
            connection = new HubConnectionBuilder()
            .WithUrl(appSettings.DriverHubUrl)
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
            connection.On("RequestVehicleLocation", async (string vehicleId) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var jsonSerializedModel = JsonSerializer.Serialize(vehicleId);
                    lblLogs.Text += Environment.NewLine + $"RequestVehicleLocation : {jsonSerializedModel}";

                    this.RequestVehicleLocation(vehicleId);
                });
            });
        }
        #endregion
    }
}