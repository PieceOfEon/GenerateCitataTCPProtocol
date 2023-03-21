using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace GenerateCitata
{
    /// <summary>
    /// Interaction logic for WindowClient.xaml
    /// </summary>
    public partial class WindowClient : Window
    {
        private TcpClient clientSocket;
        public WindowClient()
        {
            InitializeComponent();
        }

        private void AddMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                // добавляем новое сообщение в текстовое поле
                messagesTextBox.AppendText(message + "\n");

                // прокручиваем текстовое поле до последнего сообщения
                messagesTextBox.ScrollToEnd();
            });
        }

        private async void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    // получаем поток для чтения данных от сервера
                    NetworkStream stream = clientSocket.GetStream();

                    // буфер для приема входящих данных
                    byte[] buffer = new byte[1024];

                    // ожидаем получение данных от сервера
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    // декодируем полученные данные в строку
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message == "")
                    {
                        break;
                    }

                    // выводим сообщение в текстовое поле
                    AddMessage($"Server: {message}");

                    //// проверяем, не отправлено ли сообщение о завершении
                    //if (message == "Bye")
                    //{
                    //    // закрываем соединение
                    //    clientSocket.Close();
                    //    break;
                    //}
                }
            }
            catch (Exception ex)
            {
                // выводим сообщение об ошибке
                AddMessage($"Ошибка: {ex.Message}");
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем IP-адрес и порт сервера из текстовых полей
                string ipAddress = ipAddressTextBox.Text;
                int port = int.Parse(portTextBox.Text);

                // Создаем новый экземпляр TcpClient и подключаемся к серверу
                clientSocket = new TcpClient(ipAddress, port);

                // Отключаем кнопку "Connect"
                ConnectButton.IsEnabled = false;

                // Добавляем сообщение об успешном подключении в окно сообщений
                AddMessage("Connected to server at " + ipAddress + ":" + port);

                // Запускаем цикл приема сообщений от сервера
                ReceiveMessages();
            }
            catch (Exception ex)
            {
                // В случае ошибки выводим сообщение в окно сообщений
                AddMessage($"Error: {ex.Message}");
            }
        }
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (messagesSendTextBox.Text != "")
            {
                try
                {
                    // получаем поток для записи данных на сервер
                    NetworkStream stream = clientSocket.GetStream();

                    // преобразуем текст сообщения в массив байт
                    byte[] messageBytes = Encoding.UTF8.GetBytes(messagesSendTextBox.Text);

                    // отправляем сообщение на сервер
                    stream.Write(messageBytes, 0, messageBytes.Length);

                    // выводим отправленное сообщение в текстовое поле
                    AddMessage($"Я: {messagesSendTextBox.Text}");

                    // очищаем текстовое поле для ввода сообщения
                    messagesSendTextBox.Clear();
                }
                catch (Exception ex)
                {
                    // выводим сообщение об ошибке
                    AddMessage($"Ошибка: {ex.Message}");
                }
            }

        }
    }
}
