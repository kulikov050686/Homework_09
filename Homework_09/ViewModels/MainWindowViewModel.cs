using BaseClasse;
using Models;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Homework_09
{
    /// <summary>
    /// Модель-Представление главного окна
    /// </summary>
    public class MainWindowViewModel : BaseClassINPC
    {
        #region Закрытые поля

        TelegramMessageClient client;
        Window window;
        ObservableCollection<MessageLog> listMessages;
        string token;
        bool openApp;
        string inputText;

        ICommand openFileToken;
        ICommand addText;
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
        /// Список сообщений
        /// </summary>
        public ObservableCollection<MessageLog> ListMessages
        { 
            get => listMessages;

            set 
            {
                if(client != null)
                {
                    listMessages = client.BotMessageLog;
                }                

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

                        client = new TelegramMessageClient(window, token);
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
                    ListMessages.Add(new MessageLog(DateTime.Now.ToLongTimeString(), InputText, "Pavel", 25));

                    if(client != null)
                    {
                        client.SendMessage(InputText, "12345");
                    }                    

                    InputText = "";
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

            ListMessages = new ObservableCollection<MessageLog>();
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

        #endregion
    }
}
