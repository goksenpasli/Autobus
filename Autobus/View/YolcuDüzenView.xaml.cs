using Autobus.Model;
using Autobus.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Autobus.View
{
    /// <summary>
    /// Interaction logic for YolcuDüzenView.xaml
    /// </summary>
    public partial class YolcuDüzenView : UserControl
    {
        public YolcuDüzenView()
        {
            InitializeComponent();
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                Müşteri müşteri = e.Data.GetData(typeof(Müşteri)) as Müşteri;
                müşteri.KoltukNo = (int)MovableControl.GetPlacedData(stackPanel);
                MainViewModel.DatabaseSave.Execute(null);
                YolcuDüzenViewModel dc = DataContext as YolcuDüzenViewModel;
                Sefer temp = dc.SeçiliSefer;
                dc.SeçiliSefer = null;
                dc.SeçiliSefer = temp;
            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is StackPanel stackPanel && e.LeftButton == MouseButtonState.Pressed && MovableControl.GetDraggedData(stackPanel) is Müşteri müşteri)
            {
                DragDrop.DoDragDrop(stackPanel, müşteri, DragDropEffects.Copy);
            }
        }
    }
}