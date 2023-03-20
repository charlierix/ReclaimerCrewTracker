using Game.Math_WPF.WPF;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ReclaimerCrewTracker.viewmodels
{
    public class Calculations : DependencyObject
    {
        public event EventHandler CalculationChanged = null;
        private static void OnCalculationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calculations obj = (Calculations)d;
            obj.CalculationChanged?.Invoke(obj, new EventArgs());
        }

        public bool UseForFinal
        {
            get { return (bool)GetValue(UseForFinalProperty); }
            set { SetValue(UseForFinalProperty, value); }
        }
        public static readonly DependencyProperty UseForFinalProperty = DependencyProperty.Register("UseForFinal", typeof(bool), typeof(Calculations), new PropertyMetadata(false, OnCalculationChanged));

        public decimal Sum
        {
            get { return (decimal)GetValue(SumProperty); }
            set { SetValue(SumProperty, value); }
        }
        public static readonly DependencyProperty SumProperty = DependencyProperty.Register("Sum", typeof(decimal), typeof(Calculations), new PropertyMetadata(0m, OnCalculationChanged));

        public decimal Income
        {
            get { return (decimal)GetValue(IncomeProperty); }
            set { SetValue(IncomeProperty, value); }
        }
        public static readonly DependencyProperty IncomeProperty = DependencyProperty.Register("Income", typeof(decimal), typeof(Calculations), new PropertyMetadata(0m, OnCalculationChanged));

        public decimal Expenses
        {
            get { return (decimal)GetValue(ExpensesProperty); }
            set { SetValue(ExpensesProperty, value); }
        }
        public static readonly DependencyProperty ExpensesProperty = DependencyProperty.Register("Expenses", typeof(decimal), typeof(Calculations), new PropertyMetadata(0m, OnCalculationChanged));

        public string ItemsToolTip
        {
            get { return (string)GetValue(ItemsToolTipProperty); }
            set { SetValue(ItemsToolTipProperty, value); }
        }
        public static readonly DependencyProperty ItemsToolTipProperty = DependencyProperty.Register("ItemsToolTip", typeof(string), typeof(Calculations), new PropertyMetadata("One item per line\r\nDescription text is allowed, just add a space between description and number"));
    }
}
