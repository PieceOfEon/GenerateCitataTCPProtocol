using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;

namespace GenerateCitata
{
    /// <summary>
    /// Interaction logic for WindowServer.xaml
    /// </summary>
    public partial class WindowServer : Window
    {

        private TcpListener serverListener;
        public WindowServer()
        {
            InitializeComponent();
        }

        private void AddMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                // добавляем новое сообщение в текстовое поле
                messagesTextBox2.AppendText(message + "\n");

                // прокручиваем текстовое поле до последнего сообщения
                messagesTextBox2.ScrollToEnd();
            });
        }
        public async Task Start(int port)
        {
            try
            {
                // создаем новый экземпляр TcpListener и запускаем прослушивание порта
                serverListener = new TcpListener(IPAddress.Any, port);
                serverListener.Start();

                Console.WriteLine($"Server started on port {port}");

                while (true)
                {
                    // принимаем входящее подключение
                    TcpClient client = await serverListener.AcceptTcpClientAsync();

                    // обрабатываем клиента в отдельном потоке
                    Task.Run(() => HandleClient(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private async void AcceptClients()
        {
            try
            {
                while (true)
                {
                    // принимаем входящее подключение
                    TcpClient clientSocket = await serverListener.AcceptTcpClientAsync();

                    // создаем новый поток для обработки клиента
                    Task.Run(() => HandleClient(clientSocket));
                }
            }
            catch (Exception ex)
            {
                // выводим сообщение об ошибке
                AddMessage($"Ошибка: {ex.Message}");
            }
        }

        private async void HandleClient(TcpClient client)
        {
            try
            {
                string clientName = client.Client.RemoteEndPoint.ToString();

                AddMessage($"Client {clientName} connected at {DateTime.Now}");

                using (NetworkStream stream = client.GetStream())
                {
                    
                    byte[] buffer = new byte[1024];

                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead == 0)
                        {
                            break;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        AddMessage($"Client {clientName}:  {message}");

                        if (message == "Bye")
                        {
                            // if the client sends "Bye", close the connection and break out of the loop
                            AddMessage("Client disconnected: " + DateTime.Now+ "\t" + $"Client {clientName}");
                            break;
                        }
                        // generate a response
                        string response = GenerateResponse();
                        AddMessage($"Server: {response}");
                        // send the response back to the client
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage($"Error handling client: {ex.Message}");

            }

            client.Close();
        }

        private string GenerateResponse()
        {
            string[] responses = {
            "\"Жизнь - это не ожидание того, что буря пройдет, а о том, что учишься танцевать под дождем.\" - Вивиан Грин",
            "\"Смысл жизни заключается в том, чтобы стать лучшей версией самого себя.\" - Оскар Уайльд",
            "\"Смысл жизни - это не что-то, что нужно искать, а что-то, что нужно создавать.\" - Борис Пастернак",
            "\"Жизнь - это путешествие, а не назначение. Наслаждайтесь каждым мгновением, поскольку оно не повторится.\" - Элизабет Кублер-Росс",
            "\"Смысл жизни - это нечто, что необходимо обнаружить, а что-то, что нужно придумать.\" - Фрэнкль Перл",
            "\"Жизнь не сводится к тому, чтобы просто выживать. Смысл жизни заключается в том, чтобы жить и наслаждаться каждым днем.\" - Ниджа Гудал",
            "\"Смысл жизни - это создать что-то, что останется после нас. Что-то, что поможет другим и сделает этот мир лучше.\" - Ральф Уолдо Эмерсон",
            "\"Смысл жизни не заключается в том, чтобы прожить долго, а в том, чтобы прожить смыслом.\" - Сенека",
            "\"Жизнь - это путешествие, которое мы должны пройти вместе. Смысл жизни - это помочь другим и наслаждаться каждым мгновением.\" - Далай Лама",
            "\"Смысл жизни - это жить так, чтобы когда вы умираете, вы оставляете мир немного лучше, чем он был до вас.\" - Роберт У. Сервис."

            };

            Random rand = new Random();
            int index = rand.Next(responses.Length);

            return responses[index];
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // получаем порт сервера из текстового поля
                int port = int.Parse(portTextBox.Text);

                // создаем новый экземпляр TcpListener и запускаем прослушивание порта
                serverListener = new TcpListener(IPAddress.Any, port);
                serverListener.Start();

                // отключаем кнопку "Start"
                ConnectButton.IsEnabled = false;

                // добавляем сообщение об успешном запуске сервера в окно сообщений
                AddMessage("Server started on port " + port);

                // запускаем цикл ожидания подключений от клиентов
                AcceptClients();
            }
            catch (Exception ex)
            {
                // В случае ошибки вывод

            }

        }
    }
}
