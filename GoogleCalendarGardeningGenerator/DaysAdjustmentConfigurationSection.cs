using System;
using System.Collections.Generic;
using System.Configuration;

namespace GoogleCalendarGardeningGenerator
{
    public class DaysAdjustmentConfigurationSection : ConfigurationSection
    {
        public const string SECTION_NAME = "DaysAdjustmentConfig";

        [ConfigurationProperty("SpringDates")]
        public PlantingDatesCollection SpringDates => base["SpringDates"] as PlantingDatesCollection;

        [ConfigurationProperty("FallDates")]
        public PlantingDatesCollection FallDates => base["FallDates"] as PlantingDatesCollection;
    }

    public class PlantingDatesCollection : ConfigurationElementCollection, IEnumerable<PlantingDate>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PlantingDate();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PlantingDate) element).Name;
        }

        public new IEnumerator<PlantingDate> GetEnumerator()
        {
            var count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return BaseGet(i) as PlantingDate;
            }
        }
    }

    public class PlantingDate : ConfigurationElement
    {
        public string Name { get; set; }
        public DateTime? StartIndoorsDate { get; set; }
        public DateTime StartOutdoorsOrTransplant { get; set; }
    }
}