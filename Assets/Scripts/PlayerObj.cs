using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class PlayerObj
    {
        public int ID { get; set; }
        public int Money { get; set; }

        public List<CardObject> properties;
        public List<PropertyStatus> propertiesStat;
        public List<RailroadObj> railroads;

    }
}
