using System;
using System.ComponentModel;

namespace Extensions
{
    public abstract class InpcBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        [field: NonSerialized]
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}