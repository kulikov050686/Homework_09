using BaseClasse;
using Models;
using Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Homework_09
{
    /// <summary>
    /// Модель-Представление главного окна
    /// </summary>
    public class MainWindowViewModel : BaseClassINPC
    {
        #region Закрытые поля

        TelegramBotClient bot;
        Window window;
        ObservableCollection<MessageLog> listMessages;
        string token;
        bool openApp;
        string inputText;
        int indexElement;

        ICommand openFileToken;
        ICommand addText;
        ICommand addFile;
        ICommand exit;
        ICommand saveAs;
        ICommand open;

        #endregion

        #region Открытые поля

        /// <summary>
        /// Название приложения
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ввод текстовой информации
        /// </summary>
        public string InputText
        { 
            get => inputText;

            set
            {
                inputText = value;
                OnPropertyChanged("InputText");
            }
        }

        /// <summary>
        /// Номер выбранного элемента списка
        /// </summary>
        public int IndexElement
        { 
            get => indexElement;

            set 
            { 
                indexElement = value;
                OnPropertyChanged("IndexElement");
            } 
        }

        /// <summary>
        /// Список сообщений
        /// </summary>
        public ObservableCollection<MessageLog> ListMessages
        { 
            get => listMessages;

            set 
            {
                listMessages = value;
                OnPropertyChanged("ListMessages");
            } 
        }

        /// <summary>
        /// Команда открытия токена бота
        /// </summary>
        public ICommand OpenFileToken
        {
            get 
            {
                return openFileToken ?? (openFileToken = new RelayCommand((obj) => 
                {
                    var temp = FileDialog.OpenFileDialogToken();

                    if(!string.IsNullOrWhiteSpace(temp))
                    {
                        token = temp;
                        openApp = true;

                        bot = new TelegramBotClient(token);                       

                        bot.OnMessage += MessageListener;

                        bot.StartReceiving();
                    }                    
                }, (obj) => !openApp));
            }
        }

        /// <summary>
        /// Добавить текст
        /// </summary>
        public ICommand AddText
        {
            get 
            {
                return addText ?? (addText = new RelayCommand((obj) => 
                {
                    ListMessages.Add(new MessageLog(DateTime.Now.ToLongTimeString(), InputText, "Pavel", 23));

                    if (bot != null)
                    {
                        SendMessage(ListMessages[IndexElement].Id, InputText);
                    }

                    InputText = "";
                }, (obj) => openApp)); 
            }
        }

        /// <summary>
        /// Добавить файл
        /// </summary>
        public ICommand AddFile
        {
            get 
            {
                return addFile ?? (addFile = new RelayCommand((obj) => 
                {
                    if(bot != null)
                    {
                        string path = FileDialog.SendFileDialog();

                        if(path != null)
                        {
                            Send(ListMessages[IndexElement].Id, path);                            
                        }                        
                    }

                }, (obj) => openApp));
            }            
        }

        /// <summary>
        /// Выход из приложения
        /// </summary>
        public ICommand Exit
        {
            get 
            {
                return exit ?? (exit = new RelayCommand((obj) =>
                {
                    window.Close();
                }, (obj) => openApp)); 
            } 
        }

        /// <summary>
        /// Сохранить историю
        /// </summary>
        public ICommand SaveAs
        {
            get 
            {
                return saveAs ?? (saveAs = new RelayCommand((obj) => 
                {
                    FileDialog.SaveFileDialog(ListMessages);
                }, (obj) => openApp));
            } 
        }

        /// <summary>
        /// Открыть сохранённую историю 
        /// </summary>
        public ICommand Open
        {
            get 
            {
                return open ?? (open = new RelayCommand((obj) => 
                {
                    var temp = FileDialog.OpenFileDialog();

                    if (temp != null)
                    {                        
                        ListMessages = temp;
                    }
                }));
            }            
        }        

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindowViewModel()
        {
            Title = "Бот Telegram";
            openApp = false;
            window = ThisWindow();

            listMessages = new ObservableCollection<MessageLog>();
        }

        #region Закрытые методы

        /// <summary>
        /// Определяет окно
        /// </summary>        
        private Window ThisWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    return window;                    
                }
            }

            return null;
        }

        /// <summary>
        /// Обработчик события получения сообщения
        /// </summary>        
        private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if(e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                MessageBoxResult result = MessageBox.Show("Выберите один из вариантов","Сохранить документ?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes)
                {
                    string path = FileDialog.DownloadFileDialog(e.Message.Document.FileName);

                    if(path != null)
                    {
                        Download(e.Message.Document.FileId, path);
                    }
                }
            }

            if (e.Message.Text == null) return;

            var messageText = e.Message.Text;

            window.Dispatcher.Invoke(() =>
            {
                ListMessages.Add(new MessageLog(DateTime.Now.ToLongTimeString(), messageText, e.Message.Chat.FirstName, e.Message.Chat.Id));
            });
        }

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="text"> Текст сообщения </param>
        /// <param name="id"> Идентификатор </param>
        private void SendMessage(long id, string text)
        {
            bot.SendTextMessageAsync(id, text);
        }

        /// <summary>
        /// Сохранение файла из чата
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        private async void Download(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }

        /// <summary>
        /// Загрузка файла в чат
        /// </summary>
        /// <param name="id"> Идентификатор чата </param>
        /// <param name="path"> Имя файла </param>
        private async void Send(long id, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                var file = new InputOnlineFile(fs, path);
                await bot.SendDocumentAsync(id, file);
            }
        }

        #endregion
    }
}
