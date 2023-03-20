using Game.Math_WPF.WPF;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReclaimerCrewTracker
{
    public partial class CalculationsControl : UserControl
    {
        private const string TITLE = "CalculationsControl";

        private readonly DropShadowEffect _errorEffect;

        public CalculationsControl()
        {
            InitializeComponent();

            _errorEffect = new DropShadowEffect()
            {
                Color = UtilityWPF.ColorFromHex("C02020"),
                Direction = 0,
                ShadowDepth = 0,
                BlurRadius = 8,
                Opacity = .8,
            };
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                decimal? income = ParseTextBox(txtIncome.Text);

                txtIncome.Effect = income == null ?
                    _errorEffect :
                    null;

                decimal? expenses = ParseTextBox(txtExpenses.Text);

                txtExpenses.Effect = expenses == null ?
                    _errorEffect :
                    null;

                var viewmodel = DataContext as Calculations;
                if (viewmodel == null)
                    return;

                viewmodel.Income = income ?? 0m;
                viewmodel.Expenses = expenses ?? 0m;
                viewmodel.Sum = viewmodel.Income - viewmodel.Expenses;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Each line is a single number entry.  They are allowed to describe the number (description could contain numbers), so the
        /// number to look for will be surrounded by whitespace
        /// </summary>
        private static decimal? ParseTextBox(string text)
        {
            string[] lines = text.
                Replace("\r\n", "\n").
                Split('\n');

            decimal retVal = 0;

            foreach(string line in lines.Where(o => !string.IsNullOrWhiteSpace(o)))
            {
                MatchCollection matches = Regex.Matches(line, @"(^|\s)(?<num>(-|)\d+(\.\d+|))($|\s)");

                if (matches.Count != 1)
                    return null;

                retVal += decimal.Parse(matches[0].Groups["num"].Value);
            }

            return retVal;
        }
    }
}
