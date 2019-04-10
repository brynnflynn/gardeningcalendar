using System;
using System.Collections.Generic;
using System.Configuration;

namespace GoogleCalendarGardeningGenerator.Host
{
    public class DaysAdjustmentConfigurationSection : ConfigurationSection
    {
        public const string SECTION_NAME = "DaysAdjustmentConfig";

        [ConfigurationProperty("SpringDates")]
        public PlantingDatesCollection SpringDates => base["SpringDates"] as PlantingDatesCollection;

        [ConfigurationProperty("FallDates")]
        public PlantingDatesCollection FallDates => base["FallDates"] as PlantingDatesCollection;
    }

    [ConfigurationCollection(typeof(PlantingDateOffset), AddItemName = "Crop",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class PlantingDatesCollection : ConfigurationElementCollection, IEnumerable<PlantingDateOffset>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PlantingDateOffset();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PlantingDateOffset) element).Name;
        }

        public new IEnumerator<PlantingDateOffset> GetEnumerator()
        {
            var count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return BaseGet(i) as PlantingDateOffset;
            }
        }
    }

    public class PlantingDateOffset : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name {
            get => base["Name"] as string;
            set => base["Name"] = value;
        }

        [ConfigurationProperty("StartIndoorsOffset")]
        public int? StartIndoorsOffset
        {
            get => base["StartIndoorsOffset"] as int?;
            set => base["StartIndoorsOffset"] = value;
        }

        [ConfigurationProperty("StartOutdoorsOrTransplantOffset", IsRequired = true)]
        public int StartOutdoorsOrTransplantOffset
        {
            get => (int)base["StartOutdoorsOrTransplantOffset"];
            set => base["StartOutdoorsOrTransplantOffset"] = value;
        }
    }
}