using System;

namespace GoogleCalendarGardeningGenerator.Host
{
    public class PlantingDate
    {
        public string Name { get; set; }
        public DateTime? StartIndoorsDate { get; set; }
        public DateTime StartOutdoorsOrTransplantDate { get; set; }

        public PlantingDate(string name, DateTime startOutdoorsOrTransplantDate, DateTime? startIndoorsDate = null)
        {
            Name = name;
            StartOutdoorsOrTransplantDate = startOutdoorsOrTransplantDate;
            StartIndoorsDate = startIndoorsDate;
        }
    }
}