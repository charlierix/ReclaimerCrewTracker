﻿using ReclaimerCrewTracker.viewmodels;
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
    public partial class CrewMemberControl : UserControl
    {
        private const string TITLE = "CrewMemberControl";

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
    }
}