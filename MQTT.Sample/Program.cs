using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using MQTT.Sample;
using uPLibrary.Networking.M2Mqtt;
using System.Threading;

namespace MQTT.Sample
{
    public class Program
    {

        /// <summary>
        /// AWS IoT endpoint - replace with your own
        /// </summary>
        string iotendpoint = "a269tr3b87yw6l-ats.iot.us-west-2.amazonaws.com";
        int BrokerPort = 8883;
        public static string topic_Pressure1 = "iopoint/pressure/Pressure1";
        public static string topic_Pressure2 = "iopoint/pressure/Pressure2";
        public static string topic_pressure = "iopoint/pressure";
        public static MqttClient mclient = null;

        public static void Main(string[] args)
        {
            //initaliseload opc data lists
            
            var publisher = new Program();
            publisher.LoadDataholders();
            publisher.Publish();
            var opclient = new OPCClient();

            ThreadStart opcthreadstart = new ThreadStart(opclient.ConnectToServer);
            Thread opcthread = new Thread(opcthreadstart);
           
            opcthread.Start();


            opclient.ConnectToServer();
         
            Console.ReadLine();
        }
       
        public void LoadDataholders()
        {
            if (Global.opclist.Count==0)
            {
               
               
                   
                    Global.opclist.Add(new MQTTMessages("Pressure1", 0.0,DateTime.Now));
                     Global.opclist.Add(new MQTTMessages("Pressure2", 0.0, DateTime.Now));

            }

        }
        public void Publish()
        {
            //convert to pfx using openssl - see confluence
            //you'll need to add these two files to the project and copy them to the output (not included in source control deliberately!)
            var clientCert = new X509Certificate2(@"E:\certs\3adb459b9apfx.pfx", "123456789");
            var caCert = X509Certificate.CreateFromSignedFile(@"E:\certs\root-CA.crt");
            // create the client
             mclient = new MqttClient(iotendpoint, BrokerPort, true, caCert, clientCert, MqttSslProtocols.TLSv1_2);
            //message to publish - could be anything
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(Global.opclist[1]);
            //client naming has to be unique if there was more than one publisher
            mclient.Connect("Pi3-DHT11-Node");
            
          
        }

    
}
}
