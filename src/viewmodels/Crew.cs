using Game.Core;
using ReclaimerCrewTracker.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;

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
            var member = new CrewMember();
            FillOutCrewMember(member);

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
        private void Member_RequestClone(object? sender, EventArgs e)
        {
            if (sender is CrewMember member)
            {
                // Getting exception: Deferring loader 'TemplateContentLoader' does not support the Save operation.
                //CrewMember clone = null;
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    XamlServices.Save(stream, member);
                //    stream.Position = 0;
                //    clone = (CrewMember)XamlServices.Load(stream);
                //}


                // Getting exception: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 64.
                // Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles.
                // Path: $.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.Members.Parent.
                //var options = new JsonSerializerOptions()
                //{
                //    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                //};

                //string json = JsonSerializer.Serialize(member, options);
                //CrewMember clone = JsonSerializer.Deserialize<CrewMember>(json);


                // Could probably do something with reflection, but just resorting to hardcoding
                CrewMember clone = new CrewMember()
                {
                    Name = member.Name,

                    StatusText = member.StatusText,
                    StatusBack = member.StatusBack,
                    StatusBorder = member.StatusBorder,
                    StatusFore = member.StatusFore,

                    TotalTimeSeconds = member.TotalTimeSeconds,
                    TotalTimeDisplay = member.TotalTimeDisplay,
                    ShowTimes = member.ShowTimes,

                    RuntimeAdjustmentMinutes = member.RuntimeAdjustmentMinutes,
                    RuntimeAdjustmentMinutesDisplay = member.RuntimeAdjustmentMinutesDisplay,

                    Amount = member.Amount,
                    AmountDisplay = member.AmountDisplay,
                };

                clone.InOutTimes.AddRange(member.InOutTimes);

                FillOutCrewMember(clone);

                Members.Add(clone);
            }
            else
            {
                throw new ApplicationException($"Sender is not CrewMember: {sender?.GetType().ToString() ?? "null"}");
            }
        }
        private void Member_RequestDelete(object? sender, EventArgs e)
        {
            if (sender is CrewMember member)
            {
                Members.Remove(member);
            }
            else
            {
                throw new ApplicationException($"Sender is not CrewMember: {sender?.GetType().ToString() ?? "null"}");
            }
        }

        private void FillOutCrewMember(CrewMember member)
        {
            member.Parent = this;

            member.ComboBoxTouched += Member_ComboBoxTouched;
            member.TimeAdjustmentTouched += Member_TimeAdjustmentTouched;
            member.RequestClone += Member_RequestClone;
            member.RequestDelete += Member_RequestDelete;
        }
    }
}
