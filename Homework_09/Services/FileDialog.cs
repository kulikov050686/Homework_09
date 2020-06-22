using System;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using Models;

namespace Services
{
    /// <summary>
    /// Диалоговые окона для открытия и сохранения файла
    /// </summary>
    public static class FileDialog
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        private static string PathFile;

        /// <summary>
        /// Открывает диалоговое окно для сохранения в файл
        /// </summary>
        public static void SaveFileDialog(ObservableCollection<MessageLog> listSave)
        {
            SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            saveFileDialog.Title = "Сохранить файл";
            saveFileDialog.Filter = "files (*.json)|*.json|files (*.xml)|*.xml";
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (saveFileDialog.ShowDialog() == true)
            {
                PathFile = saveFileDialog.FileName;

                if (Path.GetExtension(PathFile) == ".json")
                {
                    FileIOService.SaveAsJSON(PathFile, listSave);
                }

                if (Path.GetExtension(PathFile) == ".xml")
                {
                    FileIOService.SaveAsXML(PathFile, listSave);
                }
            }
        }

        /// <summary>
        /// Открывает диалоговое окно для чтения из файла
        /// </summary>        
        public static ObservableCollection<MessageLog> OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Title = "Открыть файл";
            openFileDialog.Filter = "files (*.json)|*.json|files (*.xml)|*.xml";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (openFileDialog.ShowDialog() == true)
            {
                PathFile = openFileDialog.FileName;

                if (Path.GetExtension(PathFile) == ".json")
                {
                    return FileIOService.OpenAsJSON(PathFile);
                }

                if (Path.GetExtension(PathFile) == ".xml")
                {
                    return FileIOService.OpenAsXML(PathFile);
                }
            }

            return null;
        }

        /// <summary>
        /// Открывает диалоговое окно для чтения из файла токена
        /// </summary>        
        public static string OpenFileDialogToken()
        {
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Title = "Открыть файл";
            openFileDialog.Filter = "files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (openFileDialog.ShowDialog() == true)
            {
                PathFile = openFileDialog.FileName;

                if (Path.GetExtension(PathFile) == ".txt")
                {
                    return FileIOService.OpenAsTXT(PathFile);
                }                
            }

            return null;
        }

        /// <summary>
        /// Открывает диалоговое окно для скачиваемого файла
        /// </summary>
        /// <param name="fileName"> Название файла </param>
        public static string DownloadFileDialog(string fileName)
        {
            SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            saveFileDialog.Title = "Сохранить файл";
            saveFileDialog.Filter = "All files(*.*)|*.*";
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog.FileName = fileName;
           
            if (saveFileDialog.ShowDialog() == true)
            {
                return Path.Combine(Directory.GetCurrentDirectory(), fileName);
            }

            return null;
        }

        /// <summary>
        /// Открывает диалоговое окно для загружаемого файла
        /// </summary>        
        public static string SendFileDialog()
        {
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Title = "Открыть файл";
            openFileDialog.Filter = "All files(*.*)|*.*";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }
}
