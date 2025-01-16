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
using KiloTaxi.Common.Enums;
using KiloTaxi.Model.DTO.Request;

namespace Simulator.DriverApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        Settings appSettings = new Settings();
        private OrderDTO currentOrderDto;

        public MainWindow()
        {
            InitializeComponent();
        }
        private DriverStatus currentStatus = DriverStatus.Offline;

        #region Signar Clients Events
        private void RequestVehicleLocation(string vehicleId)
        {
            VehicleLocation vehicleLocation = new VehicleLocation() { 
                VehicleId = vehicleId,
                Lat ="16.78083",
                Long = "96.14972"
            };
            connection.InvokeAsync("SendVehicleLocation", vehicleLocation);
        }
        private void ReceiveTestMethod(string data)
        {
            var jsonSerializedModel = JsonSerializer.Serialize(data);
            Console.WriteLine(jsonSerializedModel);
            lblLogs.Text += Environment.NewLine + $"ReceiveTestMethod : {jsonSerializedModel}";
        }

        private void ReceiveOrder(OrderDTO orderDTO)
        {
            currentOrderDto = orderDTO;
            var jsonSerializedModel = JsonSerializer.Serialize(orderDTO);
            Console.WriteLine(jsonSerializedModel);
            lblLogs.Text += Environment.NewLine + $"ReceiveOrder : {jsonSerializedModel}";
        }
        // private void SendSos(SosDTO SosDTO)
        // {
        //     Console.WriteLine("Sending SOS");
        //     connection.InvokeAsync("SendSos", SosDTO);
        // }
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
        private void btnSendSos_Click(object sender, RoutedEventArgs e)
        {
            var sosDto = new SosDTO
            {
                Address = txtAddress.Text,
                ReferenceId = int.Parse(txtReferenceId.Text),
                ReasonId = int.Parse(txtReasonId.Text),
                UserType = Enum.Parse<UserType>(txtWalletType.Text),
                Status = Enum.Parse<GeneralStatus>(txtStatus.Text),
            };
            connection.InvokeAsync("SendSos", sosDto);
            lblLogs.Text += Environment.NewLine + "SendSos has been invoked";
        }
        private void btnSendTripLocation_Click(object sender, RoutedEventArgs e)
        {
            var tripLocation = new TripLocation
            {
                OrderId = "1",
                Lat = "12.15",
                Long = "18.12",
               
            };
            connection.InvokeAsync("SendTripLocation", tripLocation);
            lblLogs.Text += Environment.NewLine + "SendTripLocation has been invoked";
        }
        private async void btnSendDriverAvalilityStatus_Click(object sender, RoutedEventArgs e)
        {
            if (currentStatus == DriverStatus.Offline)
            {
                currentStatus = DriverStatus.Online;
                btnDriverMode.Content = "DriverMode Off";
            }
            else
            {
                currentStatus = DriverStatus.Offline;
                btnDriverMode.Content = "DriverMode On";
            }

            // Send the status to the server
            await connection.InvokeAsync("SendDriverAvalilityStatus", currentStatus.ToString());
        }

        private async  void btnAcceptOrder_Click(object sender, RoutedEventArgs e)
        {
            if (currentOrderDto != null)
            {
                // Log the OrderDTO data
                var jsonSerializedModel = JsonSerializer.Serialize(currentOrderDto);
                
             
                // Invoke the SignalR hub method
              await  connection.InvokeAsync("AcceptOrder", currentOrderDto);
                
            }
            else
            {
                // Handle case when no OrderDTO is available
                lblLogs.Text += Environment.NewLine + "No order available to accept.";
            }
        }
        private async  void btnArrivedLocation_Click(object sender, RoutedEventArgs e)
        {
            if (currentOrderDto != null)
            {
                // Log the OrderDTO data
                var jsonSerializedModel = JsonSerializer.Serialize(currentOrderDto);
                
             
                // Invoke the SignalR hub method
                await  connection.InvokeAsync("ArrivedLocation", currentOrderDto);
                
            }
            else
            {
                // Handle case when no OrderDTO is available
                lblLogs.Text += Environment.NewLine + "No order available to accept.";
            }
        }
        private async  void btnTripBegin_Click(object sender, RoutedEventArgs e)
        {
            if (currentOrderDto != null)
            {
                var jsonSerializedModel = JsonSerializer.Serialize(currentOrderDto);
                await  connection.InvokeAsync("SendTripBegin", currentOrderDto);
            }
            else
            {
                lblLogs.Text += Environment.NewLine + "No order for Trip begin.";
            }
        }
        private async  void btnTripFinish_Click(object sender, RoutedEventArgs e)
        {
            if (currentOrderDto != null)
            {
                List<OrderExtraDemandDTO> orderExtraDemandDTOs = new List<OrderExtraDemandDTO>
                {
                    new OrderExtraDemandDTO{OrderId = currentOrderDto.Id,Unit = 2,ExtraDemandId = 1},
                    new OrderExtraDemandDTO{OrderId = currentOrderDto.Id,Unit = 2,ExtraDemandId = 2},
                };
                OrderFormDTO orderFormDto = new OrderFormDTO();
                orderFormDto.Id = currentOrderDto.Id;
                orderFormDto.CustomerId = currentOrderDto.CustomerId;
                orderFormDto.PickUpLocation = currentOrderDto.PickUpLocation;
                orderFormDto.PickUpLat  = currentOrderDto.PickUpLat;
                orderFormDto.PickUpLong  = currentOrderDto.PickUpLong;
                orderFormDto.DriverId = currentOrderDto.DriverId;
                orderFormDto.DestinationLocation = currentOrderDto.DestinationLocation;
                orderFormDto.DestinationLat = currentOrderDto.DestinationLat;
                orderFormDto.DestinationLong = currentOrderDto.DestinationLong;
                orderFormDto.EstimatedAmount = currentOrderDto.EstimatedAmount;
                orderFormDto.Status = currentOrderDto.Status;
                await  connection.InvokeAsync("SendTripFinish", orderFormDto, orderExtraDemandDTOs);
            }
            else
            {
                lblLogs.Text += Environment.NewLine + "No order for finsih.";
            }
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

            connection.On("ReceiveOrder", async (OrderDTO orderDTO) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var jsonSerializedModel = JsonSerializer.Serialize(orderDTO);
                   // lblLogs.Text += Environment.NewLine + $"Receive : {jsonSerializedModel}";

                    this.ReceiveOrder(orderDTO);
                });
            });
            // connection.On("SendSos", async (SosDTO sosDto) =>
            // {
            //     this.Dispatcher.Invoke(() =>
            //     {
            //         var jsonSerializedModel = JsonSerializer.Serialize(sosDto);
            //         lblLogs.Text += Environment.NewLine + $"SendSos : {jsonSerializedModel}";
            //         this.SendSos(sosDto);
            //     });
            // });
        }
        #endregion
    }
}