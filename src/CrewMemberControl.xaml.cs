using ReclaimerCrewTracker.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class CrewMemberControl : UserControl
    {
        private const string TITLE = "CrewMemberControl";
        private bool _entered_popup = false;

        public event EventHandler ComboBoxTouched = null;

        public CrewMemberControl()
        {
            InitializeComponent();
        }

        private void ClockInOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.InOutTimes.Add(DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.OnComboBoxTouched();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void arrowTriggerVolume_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.ShouldShowArrow = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void arrowTriggerVolume_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.ShouldShowArrow = false || arrowTriggerVolume.IsMouseOver;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.ShouldShowArrow = true;       // ellipse covers arrow trigger volume

                //popupExtras.VerticalOffset = popupExtras.ActualHeight / 2;      //NOTE: the very first time, the height will be zero, but this is on mouse move of the trigger volume, so it will get called again
                popupExtras.VerticalOffset = popupExtras.Child.RenderSize.Height / 2;

                _entered_popup = false;

                popupExtras.IsOpen = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void parentBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.ShouldShowArrow = false;

                if (!_entered_popup && !popupExtras.IsMouseOver)
                {
                    popupExtras.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void popupExtras_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                _entered_popup = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void popupExtras_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.ShouldShowArrow = false;

                popupExtras.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetTimeAdjust_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.RuntimeAdjustmentMinutes = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimeAdjust_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                if (e.OriginalSource is Button button)
                {
                    string button_text = (button.Content as string) ?? "";

                    //Match match = Regex.Match(button_text, @"^(?<sign>[-+])(?<val>\d+)$");        // may need regex if the text gets more elaborate
                    //if (!match.Success)
                    //    throw new ApplicationException($"Couldn't parse button text: {button_text}");

                    if (!int.TryParse(button_text, out int delta))
                        throw new ApplicationException($"Couldn't parse button text: {button_text}");

                    viewmodel.RuntimeAdjustmentMinutes += delta;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.OnRequestClone();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewmodel = DataContext as CrewMember;
                if (viewmodel == null)
                    return;

                viewmodel.OnRequestDelete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
