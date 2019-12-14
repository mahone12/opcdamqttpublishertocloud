using System;

namespace MQTT.Sample
{
    public class MQTTMessages
    {
        public string TagName { get; set; }
        public double TagValue { get; set; }
        public System.DateTime timesStamps { get; set; }
        public string timestamp { get; set; }
        public MQTTMessages(string Name, double Value, System.DateTime timesStamps)
        {
            this.TagName = Name;
            this.TagValue = Value;
            this.timestamp = timesStamps.ToString();
        }
       
    }
}