using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCalendarGardeningGenerator.Host
{
    public class GardenCalendarGeneratorVm : BindableBase
    {
        public DaysAdjustmentConfigurationSection DaysAdjustment { get; set; } =
            System.Configuration.ConfigurationManager.GetSection(DaysAdjustmentConfigurationSection.SECTION_NAME) as
                DaysAdjustmentConfigurationSection;

        private string _status;

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        private DateTime _lastFrostDate;

        public DateTime LastFrostDate
        {
            get => _lastFrostDate;
            set
            {
                _lastFrostDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _firstFrostDate;

        public DateTime FirstFrostDate
        {
            get => _firstFrostDate;
            set
            {
                _firstFrostDate = value;
                OnPropertyChanged();
            }
        }

        private Tuple<string, string> _selectedCalendar;

        public Tuple<string, string> SelectedCalendar
        {
            get => _selectedCalendar;
            set
            {
                _selectedCalendar = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlantingDate> GeneratedSpringPlantingDates { get; } =
            new ObservableCollection<PlantingDate>();

        public ObservableCollection<PlantingDate> GeneratedFallPlantingDates { get; } =
            new ObservableCollection<PlantingDate>();

        public List<Tuple<string, string>> Calendars { get; } = new List<Tuple<string, string>>();

        public GardenCalendarGeneratorVm()
        {
            PropertyChanged += GardenCalendarGeneratorVm_PropertyChanged;
            var calendars = GoogleCalendarService.Instance.CalendarService.CalendarList.List();
            var list = calendars.Execute();

            foreach (var calendar in list.Items)
            {
                Calendars.Add(new Tuple<string, string>(calendar.Summary, calendar.Id));
            }
        }

        private async void GardenCalendarGeneratorVm_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LastFrostDate):
                    await Task.Run(() => RegenerateSpringDates());
                    break;

                case nameof(FirstFrostDate):
                    await Task.Run(() => RegenerateFallDates());
                    break;
            }
        }

        private void RegenerateFallDates()
        {
            Status = "Generating fall dates.";

            Application.Current.Dispatcher.Invoke(() => GeneratedFallPlantingDates.Clear());
            foreach (var fallDate in DaysAdjustment.FallDates)
            {
                Application.Current.Dispatcher.Invoke(() => GeneratedFallPlantingDates.Add(new PlantingDate(
                    fallDate.Name,
                    FirstFrostDate.AddDays(fallDate.StartOutdoorsOrTransplantOffset),
                    fallDate.StartIndoorsOffset.HasValue
                        ? FirstFrostDate.AddDays(fallDate.StartIndoorsOffset.Value)
                        : (DateTime?) null)));
            }

            Status = "Finished fall dates.";
        }

        private void RegenerateSpringDates()
        {
            Status = "Generating spring dates.";
            Application.Current.Dispatcher.Invoke(() => GeneratedSpringPlantingDates.Clear());
            foreach (var fallDate in DaysAdjustment.SpringDates)
            {
                Application.Current.Dispatcher.Invoke(() => GeneratedSpringPlantingDates.Add(new PlantingDate(
                    fallDate.Name,
                    LastFrostDate.AddDays(fallDate.StartOutdoorsOrTransplantOffset),
                    fallDate.StartIndoorsOffset.HasValue
                        ? LastFrostDate.AddDays(fallDate.StartIndoorsOffset.Value)
                        : (DateTime?) null)));
            }

            Status = "Finished spring dates.";
        }

        public void AddOrUpdateDates()
        {
            IsEnabled = false;
            HandleDates();
            IsEnabled = true;
        }

        private void HandleDates()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Status = "Cleaning up old dates.";
            ClearRelatedYearDatesIfAny();

            var stillExistingEvents = GetOverlappingRecords();

            if (stillExistingEvents.Items.Any())
            {
                MessageBox.Show(
                    $"Failed to delete all events in relevant range: {stillExistingEvents.Items.Count} remaining.");
                Status = "Failed to delete all records.";
                return;
            }

            Status = "Handling spring dates.";
            foreach (var generatedSpringPlantingDate in GeneratedSpringPlantingDates)
            {
                AddNewDate(generatedSpringPlantingDate);
            }


            Status = "Handling fall dates.";
            foreach (var generatedFallPlantingDate in GeneratedFallPlantingDates)
            {
                AddNewDate(generatedFallPlantingDate);
            }

            stopwatch.Stop();
            Status = $"Finished. {stopwatch.Elapsed}";
        }

        private void AddNewDate(PlantingDate generatedSpringPlantingDate)
        {
            Event plantingDateEvent;
            EventsResource.InsertRequest plantingDateEventRequest;
            if (generatedSpringPlantingDate.StartIndoorsDate.HasValue)
            {
                plantingDateEvent = new Event
                {
                    Start = new EventDateTime {DateTime = generatedSpringPlantingDate.StartIndoorsDate},
                    End = new EventDateTime {DateTime = generatedSpringPlantingDate.StartIndoorsDate.Value.AddDays(7)},
                    Summary = $"Start {generatedSpringPlantingDate.Name} Indoors"
                };
                plantingDateEventRequest =
                    GoogleCalendarService.Instance.CalendarService.Events.Insert(plantingDateEvent,
                        SelectedCalendar.Item2);
                try
                {
                    plantingDateEventRequest.Execute();
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                    plantingDateEventRequest.Execute();
                }
                Thread.Sleep(1000);
            }

            plantingDateEvent = new Event
            {
                Start = new EventDateTime {DateTime = generatedSpringPlantingDate.StartOutdoorsOrTransplantDate},
                End = new EventDateTime
                    {DateTime = generatedSpringPlantingDate.StartOutdoorsOrTransplantDate.AddDays(7)},
                Summary = generatedSpringPlantingDate.StartIndoorsDate.HasValue
                    ? $"Transplant {generatedSpringPlantingDate.Name} Outdoors"
                    : $"Plant {generatedSpringPlantingDate.Name} Outdoors",
            };

            plantingDateEventRequest =
                GoogleCalendarService.Instance.CalendarService.Events.Insert(plantingDateEvent, SelectedCalendar.Item2);
            try
            {
                plantingDateEventRequest.Execute();
            }
            catch (Exception ex)
            {
                Thread.Sleep(5000);
                plantingDateEventRequest.Execute();
            }
            Thread.Sleep(1000);
        }

        private void ClearRelatedYearDatesIfAny()
        {
            var events = GetOverlappingRecords();
            if (events.Items.Any() &&
                MessageBox.Show(
                    $"About to delete {events.Items.Count} events, such as {events.Items.FirstOrDefault().Summary} on {events.Items.FirstOrDefault().Start.Date}.\r\nDo you wish to continue?",
                    "Warning", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            foreach (var eventsItem in events.Items)
            {
                var deleteRequest =
                    GoogleCalendarService.Instance.CalendarService.Events.Delete(SelectedCalendar.Item2,
                        eventsItem.Id);
                try
                {
                    deleteRequest.Execute();
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                    deleteRequest.Execute();
                }
                Thread.Sleep(1000);
            }
        }

        private Events GetOverlappingRecords()
        {
            var firstDay = GeneratedSpringPlantingDates.Min(x => x.StartIndoorsDate ?? x.StartOutdoorsOrTransplantDate);
            var lastDay = GeneratedFallPlantingDates.Max(x => x.StartOutdoorsOrTransplantDate);

            var request = GoogleCalendarService.Instance.CalendarService.Events.List(SelectedCalendar.Item2);
            request.TimeMin = firstDay;
            request.TimeMax = lastDay;
            request.ShowDeleted = false;
            request.SingleEvents = true;

            var events = request.Execute(); 
            return events;
        }
    }
}