using System.Windows.Controls;
using System.Windows.Data;

namespace Autobus.View
{
    /// <summary>
    /// Interaction logic for TümSeferlerView.xaml
    /// </summary>
    public partial class TümSeferlerView : UserControl
    {
        public static CollectionViewSource cvs;

        public TümSeferlerView()
        {
            InitializeComponent();
            cvs = TryFindResource("Müşteriler") as CollectionViewSource;
        }
    }
}