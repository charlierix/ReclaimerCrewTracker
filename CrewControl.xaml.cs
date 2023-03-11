using ReclaimerCrewTracker.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReclaimerCrewTracker
{
    public partial class CrewControl : UserControl
    {
        private const string TITLE = "CrewControl";

        public CrewControl()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as Crew;
                if (viewmodel == null)
                    return;

                viewmodel.AddCrew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
