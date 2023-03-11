using Game.Math_WPF.Mathematics;
using Game.Math_WPF.WPF;
using Game.Math_WPF.WPF.Controls3D;
using ReclaimerCrewTracker.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReclaimerCrewTracker.viewmodels
{
    public class CrewMember : DependencyObject
    {
        public event EventHandler ComboBoxTouched = null;
        public void OnComboBoxTouched()
        {
            ComboBoxTouched?.Invoke(this, EventArgs.Empty);
        }

        public CrewMember()
        {
            InOutTimes = new ObservableCollection<DateTime>();
            InOutTimes.CollectionChanged += InOutTimes_CollectionChanged;

            InOutTimes_CollectionChanged(this, null);
        }

        public Crew Parent { get; set; }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(CrewMember), new PropertyMetadata(""));

        // ------------- InOut / Running|Stopped -------------

        public ObservableCollection<DateTime> InOutTimes { get; private set; }

        private void InOutTimes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            bool is_running = InOutTimes.Count % 2 == 1;

            StatusText = is_running ?
                Constants.RUNNING_TEXT :
                Constants.STOPPED_TEXT;

            StatusBack = is_running ?
                UtilityWPF.BrushFromHex(Constants.RUNNING_BACK) :
                UtilityWPF.BrushFromHex(Constants.STOPPED_BACK);

            StatusBorder = is_running ?
                UtilityWPF.BrushFromHex(Constants.RUNNING_BORDER) :
                UtilityWPF.BrushFromHex(Constants.STOPPED_BORDER);

            StatusFore = is_running ?
                UtilityWPF.BrushFromHex(Constants.RUNNING_FORE) :
                UtilityWPF.BrushFromHex(Constants.STOPPED_FORE);
        }

        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }
        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register("StatusText", typeof(string), typeof(CrewMember), new PropertyMetadata(""));

        public Brush StatusBack
        {
            get { return (Brush)GetValue(StatusBackProperty); }
            set { SetValue(StatusBackProperty, value); }
        }
        public static readonly DependencyProperty StatusBackProperty = DependencyProperty.Register("StatusBack", typeof(Brush), typeof(CrewMember), new PropertyMetadata(Brushes.Transparent));

        public Brush StatusBorder
        {
            get { return (Brush)GetValue(StatusBorderProperty); }
            set { SetValue(StatusBorderProperty, value); }
        }
        public static readonly DependencyProperty StatusBorderProperty = DependencyProperty.Register("StatusBorder", typeof(Brush), typeof(CrewMember), new PropertyMetadata(Brushes.Transparent));

        public Brush StatusFore
        {
            get { return (Brush)GetValue(StatusForeProperty); }
            set { SetValue(StatusForeProperty, value); }
        }
        public static readonly DependencyProperty StatusForeProperty = DependencyProperty.Register("StatusFore", typeof(Brush), typeof(CrewMember), new PropertyMetadata(Brushes.Transparent));

        // ------------- Total Runtime -------------

        public double TotalTimeSeconds { get; set; } = 0;

        public string TotalTimeDisplay
        {
            get { return (string)GetValue(TotalTimeDisplayProperty); }
            set { SetValue(TotalTimeDisplayProperty, value); }
        }
        public static readonly DependencyProperty TotalTimeDisplayProperty = DependencyProperty.Register("TotalTimeDisplay", typeof(string), typeof(CrewMember), new PropertyMetadata(""));

        public void UpdateTotalTimeDisplay(TimeFromTo[] times_parent, DateTime now)
        {
            var times_member = Utility.GetStartStopTimes(InOutTimes, now);

            var times_intersect = Utility.Intersect(times_parent, times_member);

            if (ShowTimes)
            {
                ShowTheTimes(times_parent, times_member, times_intersect);
                ShowTimes = false;
            }

            TimeSpan total = new TimeSpan();
            foreach (var span in times_intersect)
                total = total.Add(span.To - span.From);

            TotalTimeSeconds = total.TotalSeconds;
            TotalTimeDisplay = Utility.TimeSpanToString(total);
        }
        private void ShowTheTimes(TimeFromTo[] parent, TimeFromTo[] member, TimeFromTo[] intersect)
        {
            var all = parent.
                Concat(member).
                Concat(intersect).
                ToArray();

            if (all.Length == 0)
                return;

            DateTime min = all.Min(o => o.From);
            DateTime max = all.Max(o => o.To);

            double total_seconds = (max - min).TotalSeconds;

            Debug3DWindow window = new Debug3DWindow()
            {
                Title = Name ?? "",
            };

            ShowTheTimes_Draw(window, min, total_seconds, parent, -0.25, "000");
            ShowTheTimes_Draw(window, min, total_seconds, member, 0.25, "FFF");
            ShowTheTimes_Draw(window, min, total_seconds, intersect, 0, "20E357");

            window.Show();
        }
        private static void ShowTheTimes_Draw(Debug3DWindow window, DateTime min, double total_seconds, TimeFromTo[] times, double y, string color)
        {
            const double SIZE = 12;

            var sizes = Debug3DWindow.GetDrawSizes(SIZE);

            for (int i = 0; i < times.Length; i++)
            {
                double x1 = GetX(min, times[i].From, total_seconds, SIZE);
                double x2 = GetX(min, times[i].To, total_seconds, SIZE);

                window.AddLine(new Point3D(x1, y, 0), new Point3D(x2, y, 0), sizes.line, UtilityWPF.ColorFromHex(color));
            }
        }

        private static double GetX(DateTime min, DateTime val, double total_seconds, double size)
        {
            double elapsed = (val - min).TotalSeconds;
            double percent = elapsed / total_seconds;

            return (size / -2) + (size * percent);
        }

        public bool ShowTimes = false;

        // ------------- Amount -------------

        private decimal _amount = 0;
        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                AmountDisplay = _amount.ToString("N0");
            }
        }

        public string AmountDisplay
        {
            get { return (string)GetValue(AmountDisplayProperty); }
            set { SetValue(AmountDisplayProperty, value); }
        }
        public static readonly DependencyProperty AmountDisplayProperty = DependencyProperty.Register("AmountDisplay", typeof(string), typeof(CrewMember), new PropertyMetadata(""));
    }
}
