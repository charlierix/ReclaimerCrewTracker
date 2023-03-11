using Game.Core;
using Game.Math_WPF.Mathematics;
using Game.Math_WPF.WPF;
using ReclaimerCrewTracker.models;
using ReclaimerCrewTracker.viewmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.Windows.Threading;
using System.Xml.Linq;

namespace ReclaimerCrewTracker
{
    public partial class MainWindow : Window
    {
        private readonly string _namesFilename;

        private Crew _salvage = null;
        private Crew _protection = null;

        private DispatcherTimer _timer;

        private readonly DropShadowEffect _errorEffect;

        private bool _initialized = false;

        public MainWindow()
        {
            InitializeComponent();

            Background = SystemColors.ControlBrush;

            DataContext = this;

            StartStopTimes = new ObservableCollection<DateTime>();
            StartStopTimes.CollectionChanged += StartStopTimes_CollectionChanged;

            //StartStopTimes.Add(DateTime.UtcNow);      // maybe start in a running state?  I have a feeling this will be missed, but if it starts in a running state, the owner will get an inflated percent until they add crew
            StartStopTimes_CollectionChanged(this, null);

            NamesList = new ObservableCollection<string>();

            _errorEffect = new DropShadowEffect()
            {
                Color = UtilityWPF.ColorFromHex("C02020"),
                Direction = 0,
                ShadowDepth = 0,
                BlurRadius = 8,
                Opacity = .8,
            };

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            string folder_exe = Environment.CurrentDirectory;
            _namesFilename = System.IO.Path.Combine(folder_exe, "names.json");
            LoadNames();

            _initialized = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _salvage = new Crew() { Parent = this };
                _salvage.ComboBoxTouched += ComboBoxTouched;
                salvage.DataContext = _salvage;

                _protection = new Crew() { Parent = this };
                _protection.ComboBoxTouched += ComboBoxTouched;
                protection.DataContext = _protection;

                txtProtectPercent_TextChanged(this, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PauseResumeOperation_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                StartStopTimes.Add(DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                RefreshVisuals();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSellAmt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!_initialized)
                    return;

                if (decimal.TryParse(txtSellAmt.Text, out decimal amount))
                {
                    NetAmount = amount * 0.995m;
                    txtSellAmt.Effect = null;
                }
                else
                {
                    NetAmount = 0m;
                    txtSellAmt.Effect = _errorEffect;
                }

                NetAmountDisplay = NetAmount.ToString("N0");

                RefreshVisuals();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void txtProtectPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!_initialized)
                    return;

                if (double.TryParse(txtProtectPercent.Text, out double percent))
                {
                    ProtectPercent = UtilityMath.Clamp(percent / 100d, 0, 1);
                    txtProtectPercent.Effect = null;
                }
                else
                {
                    ProtectPercent = 1;
                    txtProtectPercent.Effect = _errorEffect;
                }

                // actually, there's no need for a label.  oh well, the property is populated just in case
                ProtectPercentDisplay = Math.Round(ProtectPercent * 100, 0).ToString() + "%";

                RefreshVisuals();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBoxTouched(object? sender, EventArgs e)
        {
            try
            {
                if (!_initialized)
                    return;

                string[] names = _salvage.Members.
                    Concat(_protection.Members).
                    Select(o => (o.Name ?? "").Trim()).
                    Concat(_names).
                    Where(o => !string.IsNullOrWhiteSpace(o)).
                    Distinct().
                    OrderBy(o => o).
                    ToArray();

                SaveNames(names);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ------------- StartStop / Running|Stopped -------------

        public ObservableCollection<DateTime> StartStopTimes { get; private set; }

        private void StartStopTimes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            bool is_running = StartStopTimes.Count % 2 == 1;

            RunningStatusText = is_running ?
                Constants.RUNNING_TEXT :
                Constants.STOPPED_TEXT;

            RunningStatusBack = is_running ?
                UtilityWPF.BrushFromHex(Constants.RUNNING_BACK) :
                UtilityWPF.BrushFromHex(Constants.STOPPED_BACK);

            RunningStatusBorder = is_running ?
                UtilityWPF.BrushFromHex(Constants.RUNNING_BORDER) :
                UtilityWPF.BrushFromHex(Constants.STOPPED_BORDER);

            RunningStatusFore = is_running ?
                UtilityWPF.BrushFromHex(Constants.RUNNING_FORE) :
                UtilityWPF.BrushFromHex(Constants.STOPPED_FORE);
        }

        public string RunningStatusText
        {
            get { return (string)GetValue(RunningStatusTextProperty); }
            set { SetValue(RunningStatusTextProperty, value); }
        }
        public static readonly DependencyProperty RunningStatusTextProperty = DependencyProperty.Register("RunningStatusText", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public Brush RunningStatusBack
        {
            get { return (Brush)GetValue(RunningStatusBackProperty); }
            set { SetValue(RunningStatusBackProperty, value); }
        }
        public static readonly DependencyProperty RunningStatusBackProperty = DependencyProperty.Register("RunningStatusBack", typeof(Brush), typeof(MainWindow), new PropertyMetadata(Brushes.Transparent));

        public Brush RunningStatusBorder
        {
            get { return (Brush)GetValue(RunningStatusBorderProperty); }
            set { SetValue(RunningStatusBorderProperty, value); }
        }
        public static readonly DependencyProperty RunningStatusBorderProperty = DependencyProperty.Register("RunningStatusBorder", typeof(Brush), typeof(MainWindow), new PropertyMetadata(Brushes.Transparent));

        public Brush RunningStatusFore
        {
            get { return (Brush)GetValue(RunningStatusForeProperty); }
            set { SetValue(RunningStatusForeProperty, value); }
        }
        public static readonly DependencyProperty RunningStatusForeProperty = DependencyProperty.Register("RunningStatusFore", typeof(Brush), typeof(MainWindow), new PropertyMetadata(Brushes.Transparent));

        // ------------- Total Runtime -------------

        public string TotalTimeDisplay
        {
            get { return (string)GetValue(TotalTimeDisplayProperty); }
            set { SetValue(TotalTimeDisplayProperty, value); }
        }
        public static readonly DependencyProperty TotalTimeDisplayProperty = DependencyProperty.Register("TotalTimeDisplay", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        private void UpdateTotalTimeDisplay(TimeFromTo[] times)
        {
            TimeSpan total = new TimeSpan();
            foreach (var span in times)
                total = total.Add(span.To - span.From);

            TotalTimeDisplay = Utility.TimeSpanToString(total);
        }

        private void ShowTimes_Click(object sender, RoutedEventArgs e)
        {
            _salvage.ShowTimes();
            _protection.ShowTimes();
        }

        // ------------- Protect Percent -------------

        public double ProtectPercent { get; set; }

        public string ProtectPercentDisplay
        {
            get { return (string)GetValue(ProtectPercentDisplayProperty); }
            set { SetValue(ProtectPercentDisplayProperty, value); }
        }
        public static readonly DependencyProperty ProtectPercentDisplayProperty = DependencyProperty.Register("ProtectPercentDisplay", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        // ------------- Net Amount -------------

        public decimal NetAmount { get; set; }

        public string NetAmountDisplay
        {
            get { return (string)GetValue(NetAmountDisplayProperty); }
            set { SetValue(NetAmountDisplayProperty, value); }
        }
        public static readonly DependencyProperty NetAmountDisplayProperty = DependencyProperty.Register("NetAmountDisplay", typeof(string), typeof(MainWindow), new PropertyMetadata("0"));

        // ------------- Names List -------------

        private string[] _names = new string[0];

        public ObservableCollection<string> NamesList { get; private set; }

        // -------------------------- Private Methods --------------------------

        private void RefreshVisuals()
        {
            DateTime now = DateTime.UtcNow;

            var times = Utility.GetStartStopTimes(StartStopTimes, now);
            UpdateTotalTimeDisplay(times);

            _salvage.UpdateTotalTimeDisplay(times, now);
            _protection.UpdateTotalTimeDisplay(times, now);

            double total_salvage = _salvage.Members.Sum(o => o.TotalTimeSeconds);
            double total_protection = _protection.Members.Sum(o => o.TotalTimeSeconds);

            if ((total_salvage + total_protection).IsNearZero())
            {
                foreach (var member in _salvage.Members.Concat(_protection.Members))
                    member.Amount = 0;

                return;
            }

            decimal amount_salvage = NetAmount * Convert.ToDecimal(total_salvage / (total_salvage + (total_protection * ProtectPercent)));
            decimal amount_protection = NetAmount - amount_salvage;

            foreach (var member in _salvage.Members)
            {
                member.Amount = total_salvage.IsNearZero() ?        // can't divide by zero
                    0 :
                    amount_salvage * Convert.ToDecimal(member.TotalTimeSeconds / total_salvage);
            }

            foreach (var member in _protection.Members)
            {
                member.Amount = total_protection.IsNearZero() ?        // can't divide by zero
                    0 :
                    amount_protection * Convert.ToDecimal(member.TotalTimeSeconds / total_protection);
            }
        }

        private void LoadNames()
        {
            if (!File.Exists(_namesFilename))
                return;

            string jsonString = System.IO.File.ReadAllText(_namesFilename);

            _names = JsonSerializer.Deserialize<string[]>(jsonString) ?? new string[0];

            RefreshComboLists();
        }
        private void SaveNames(string[] names)
        {
            if (names == null || names.Length == 0)
                return;

            if (!(_names == null || _names.Except(names).Count() > 0 || names.Except(_names).Count() > 0))
                return;

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            string serialized = JsonSerializer.Serialize(names, options);

            File.WriteAllText(_namesFilename, serialized);
            _names = names;

            RefreshComboLists();
        }
        private void RefreshComboLists()
        {
            NamesList.Clear();
            foreach (string name in _names)
                NamesList.Add(name);
        }
    }
}
