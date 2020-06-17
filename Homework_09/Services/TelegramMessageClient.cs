using Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using Telegram.Bot;

namespace Services
{
    public class TelegramMessageClient
    {
        #region Закрытые поля

        Window window;
        HttpClient hc;
        TelegramBotClient bot;
        ObservableCollection<MessageLog> botMessageLog;

        #endregion

        #region Открытые поля

        /// <summary>
        /// Лист сообщений
        /// </summary>
        public ObservableCollection<MessageLog> BotMessageLog { get => botMessageLog; set => botMessageLog = value; }

        #endregion

        /// <summary>
        /// Конструктор базовый
        /// </summary>
        /// <param name="mainWindow"> Окно формы </param>
        /// <param name="token"> Токен телеграмм бота </param>
        public TelegramMessageClient(Window mainWindow, string token)
        {
            BotMessageLog = new ObservableCollection<MessageLog>();
            bot = new TelegramBotClient(token);
            window = mainWindow;

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        /// <summary>
        /// Конструктор с прокси сервером
        /// </summary>
        /// <param name="mainWindow"> Окно формы </param>
        /// <param name="token"> Токен телеграмм бота </param>
        /// <param name="addressProxy"> Адрес прокси сервера (формат ввода $"http://XXX.XXX.XXX.XXX:XXX") </param>
        /// <param name="login"> Логин </param>
        /// <param name="password"> Пароль </param>
        public TelegramMessageClient(Window mainWindow, string token, string addressProxy, string login = "", string password = "")
        {
            SetProxy(addressProxy, login, password);

            if(hc == null)
            {
                MessageBox.Show("Ошибка прокси сервера!!!");
                return;
            }            

            BotMessageLog = new ObservableCollection<MessageLog>();
            bot = new TelegramBotClient(token, hc);
            window = mainWindow;

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        #region Открытые методы

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="text"> Текст сообщения </param>
        /// <param name="id"> Идентификатор </param>
        public void SendMessage(string text, string id)
        {
            bot.SendTextMessageAsync(Convert.ToInt64(id), text);
        }

        #endregion

        #region Закрытые методы

        /// <summary>
        /// Обработчик события получения сообщения
        /// </summary>        
        private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);                
            }

            if (e.Message.Text == null) return;

            var messageText = e.Message.Text;

            window.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(new MessageLog(DateTime.Now.ToLongTimeString(), messageText, e.Message.Chat.FirstName, e.Message.Chat.Id));
            });            
        }

        /// <summary>
        /// Скачивание документа
        /// </summary>
        /// <param name="fileId"> Идентификатор файла </param>
        /// <param name="path"> Путь к файлу </param>
        private async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);

            using (FileStream fs = new FileStream("_" + path, FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }

        /// <summary>
        /// Задаёт прокси сервер
        /// </summary>
        /// <param name="address"> Адрес прокси сервера </param>
        /// <param name="login"> Логин </param>
        /// <param name="password"> Пароль </param>
        private void SetProxy(string address, string login = "", string password = "")
        {
            var proxy = new WebProxy()
            {
                Address = new Uri(address),
                UseDefaultCredentials = false                
            };

            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            {
                proxy.Credentials = new NetworkCredential(userName: login, password: password);
            }
            
            var httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            };

            hc = new HttpClient(httpClientHandler);
        }

        #endregion
    }
}
