using System;

namespace MQTT.Sample
{
    public class MQTTMessages
    {
        public string TagName { get; set; }
        public double TagValue { get; set; }
        public int timerSec { get; set; }
        public MQTTMessages(string Name, double Value)
        {
            this.TagName = Name;
            this.TagValue = Value;
        }
       
    }
}