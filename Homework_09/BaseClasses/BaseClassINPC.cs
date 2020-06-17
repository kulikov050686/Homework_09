using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseClasse
{
    /// <summary>
    /// Базовый класс с реализацией интерфейса INotifyPropertyChanged
    /// </summary>
    public class BaseClassINPC : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие для извещения об изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для вызова события извещения об изменении свойства
        /// </summary>
        /// <param name="property"> Изменившееся свойство </param>
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Метод для вызова события извещения об изменении списка свойств
        /// </summary>
        /// <param name="propList"> Последовательность имён свойств </param>
        public void OnPropertyChanged(IEnumerable<string> propList)
        {
            foreach (string prp in propList.Where(name => !string.IsNullOrWhiteSpace(name)))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prp));
            }                
        }

        /// <summary>
        /// Метод для вызова события извещения об изменении списка свойств
        /// </summary>
        /// <param name="propList"> Последовательность свойств </param>
        public void OnPropertyChanged(IEnumerable<PropertyInfo> propList)
        {
            foreach (PropertyInfo prp in propList)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prp.Name));
            }                
        }

        /// <summary>
        /// Метод для вызова события извещения об изменении всех свойств
        /// </summary>
        public void OnAllPropertyChanged() => OnPropertyChanged(GetType().GetProperties());        
    }
}
