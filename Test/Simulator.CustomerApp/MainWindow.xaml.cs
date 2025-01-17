using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Response;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
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
using KiloTaxi.Model.DTO.Request;

namespace Simulator.CustomerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string driverName;
        private string driverPhone;
        private string vehicleNo;

        // The properties that will be bound to the UI
        public string DriverName
        {
            get => driverName;
            set
            {
                driverName = value;
                OnPropertyChanged(nameof(DriverName)); // Notify the UI that the property changed
            }
        }

        public string DriverPhone
        {
            get => driverPhone;
            set
            {
                driverPhone = value;
                OnPropertyChanged(nameof(DriverPhone)); // Notify the UI that the property changed
            }
        }

        public string VehicleNo
        {
            get => vehicleNo;
            set
            {
                vehicleNo = value;
                OnPropertyChanged(nameof(VehicleNo)); // Notify the UI that the property changed
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        HubConnection connection;
        Settings appSettings = new Settings();
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Signar Clients Events
      
        private void ReceiveTestMethod(string data)
        {
            var jsonSerializedModel = JsonSerializer.Serialize(data);
            Console.WriteLine(jsonSerializedModel);
            lblLogs.Text += Environment.NewLine + $"ReceiveTestMethod : {jsonSerializedModel}";
        }

        private void ReceiveDriverInfo(string order, string driver)
        {
          
            Console.WriteLine("Hello");
            lblLogs.Text += Environment.NewLine + $"ReceiveDriverInfo : {order} {driver}";
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
            .WithUrl(appSettings.CustomerHubUrl)
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
            
            connection.On("ReceiveTestMethod", async (string data) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var jsonSerializedModel = JsonSerializer.Serialize(data);
                    lblLogs.Text += Environment.NewLine + $"ReceiveTestMethod : {jsonSerializedModel}";

                    this.ReceiveTestMethod(data);
                });
            });

            connection.On("ReceiveDriverInfo", async (OrderDTO orderDTO, DriverInfoDTO driverDTO) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var jsonSerializedModelOrderDTO = JsonSerializer.Serialize(orderDTO);
                    var jsonSerializedModelDriverDTO = JsonSerializer.Serialize(driverDTO);
                    lblLogs.Text += Environment.NewLine + $"ReceiveDriverInfo : Hello";
                    this.ReceiveDriverInfo(jsonSerializedModelOrderDTO, jsonSerializedModelDriverDTO);

                });
            });
            connection.On("ReceiveDriverArrivedLocation", async (OrderDTO orderDTO, DriverInfoDTO driverDTO) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var jsonSerializedModelOrderDTO = JsonSerializer.Serialize(orderDTO);
                    var jsonSerializedModelDriverDTO = JsonSerializer.Serialize(driverDTO);
                    lblLogs.Text += Environment.NewLine + $"Driver {driverDTO.Name} arrived {orderDTO.PickUpLocation}";

                });
            });
            connection.On("ReceiveTripBegin", async (OrderDTO orderDTO) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var jsonSerializedModelOrderDTO = JsonSerializer.Serialize(orderDTO);
                    lblLogs.Text += Environment.NewLine + $"Trip Beginned";

                });
            });
            connection.On("ReceiveDriverLocation", async (VehicleLocation vehicleLocation) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var jsonSerializedModelOrderDTO = JsonSerializer.Serialize(vehicleLocation);
                    lblLogs.Text += Environment.NewLine + $"receive driver location {vehicleLocation.Lat}:{vehicleLocation.Long}";

                });
            });
            connection.On("ReceiveTripComplete", async (OrderFormDTO orderDTO,PromotionUsageDTO promotionUsageDto,List<OrderExtraDemandDTO> orderExtraDemandDtos) =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var orderExtraDemandDTO in orderExtraDemandDtos)
                    {
                        var jsonSerializedModelOrderDTO = JsonSerializer.Serialize(orderDTO);
                        var jsonSerializedModelPromotionUsageDTO = JsonSerializer.Serialize(promotionUsageDto);
                    
                        lblLogs.Text += Environment.NewLine + $"Order Finish :{orderExtraDemandDTO.ExtraDemandDto.Title}     {orderExtraDemandDTO.ExtraDemandDto.Amount} " +
                                        $"{orderExtraDemandDTO.Unit} " +
                                        $"{orderDTO.EstimatedAmount}";
                          
                    }
                    

                });
            });
            
        }

       
        #endregion
    }
}