using ReclaimerCrewTracker.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReclaimerCrewTracker.viewmodels
{
    public class Crew : DependencyObject
    {
        public event EventHandler ComboBoxTouched = null;
        public event EventHandler TimeAdjustmentTouched = null;

        public MainWindow Parent { get; set; }

        public ObservableCollection<CrewMember> Members { get; private set; } = new ObservableCollection<CrewMember>();

        public void AddCrew()
        {
            var member = new CrewMember()
            {
                Parent = this,
            };

            member.ComboBoxTouched += Member_ComboBoxTouched;
            member.TimeAdjustmentTouched += Member_TimeAdjustmentTouched;

            Members.Add(member);
        }

        public void UpdateTotalTimeDisplay(TimeFromTo[] times, DateTime now)
        {
            foreach (var member in Members)
            {
                member.UpdateTotalTimeDisplay(times, now);
            }
        }

        public void ShowTimes()
        {
            foreach (var member in Members)
                member.ShowTimes = true;
        }

        private void Member_ComboBoxTouched(object? sender, EventArgs e)
        {
            ComboBoxTouched?.Invoke(sender, e);
        }
        private void Member_TimeAdjustmentTouched(object? sender, EventArgs e)
        {
            TimeAdjustmentTouched?.Invoke(sender, e);
        }
    }
}
