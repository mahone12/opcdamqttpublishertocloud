using System;
using System.Collections.Generic;

using Opc;

using System.Text;

using System.Threading;
using Opc.Da;

using Newtonsoft.Json;

namespace MQTT.Sample
{
    public class OPCClient
    {
        public static ThreadState Thread_status = ThreadState.Unstarted;
        private Opc.URL url;
        private Opc.Da.Server server;
        private OpcCom.Factory fact = new OpcCom.Factory();
        private Opc.Da.Subscription groupRead;
        private Opc.Da.Subscription groupWrite;
        private Opc.Da.SubscriptionState groupState;
        private Opc.Da.SubscriptionState groupStateWrite;
        private Opc.Da.Item[] items = new Opc.Da.Item[8];
        private Opc.Da.Item[] Write_items = new Opc.Da.Item[2];
        public OPCClient()
        {

        }

        public void ConnectToServer()
        {
            try
            {
                url = new Opc.URL("opcda://MUHUMTHA/Kepware.KEPServerEX.V6");
              
                server = new Opc.Da.Server(fact, url);
                System.Net.NetworkCredential networkCredential = new System.Net.NetworkCredential();
                Opc.ConnectData connecteddata = new Opc.ConnectData(networkCredential);
                server.Connect(url, connecteddata);
                groupState = new Opc.Da.SubscriptionState();
                groupState.Name = "Group";
                groupState.UpdateRate = 1;// this isthe time between every reads from OPC server
                groupState.Active = true;//this must be true if you the group has to read value
                groupRead = (Opc.Da.Subscription)server.CreateSubscription(groupState);
                groupRead.DataChanged += new Opc.Da.DataChangedEventHandler(UpdateTagData);//callback when the data are readed
                //Opc.Da.Item[] items_read = groupRead.AddItems(createSomeTags());
                Opc.Da.Item[] items_read = groupRead.AddItems(createSomeTags());

              

            }
            catch (Exception E)
            {

                Console.WriteLine(E.Message);
            }
        }

   
        public Opc.Da.Item[] createSomeTags()
        {
            Opc.Da.Item[] tag_items = new Opc.Da.Item[2];
            tag_items[0] = new Opc.Da.Item();
            //tag_items[0].ItemName = "SDV_1600";
            tag_items[0].ItemName = "MUHU_PLC.PLC1.Pressure1";
            tag_items[1] = new Opc.Da.Item();
            tag_items[1].ItemName = "MUHU_PLC.PLC1.Pressure2";
            return tag_items;
        }

     

        public void UpdateTagData(object subscriptionHandle, object requestHandle, Opc.Da.ItemValueResult[] values)
        {
            //Console.WriteLine("hit");
            for (int i = 0; i < values.Length; i++)
            {

           

                Global.opclist[Global.opclist.FindIndex(delegate (MQTTMessages mqttMessages)
                {
                    return mqttMessages.TagName == values[i].ItemName.Substring(14, 9);
                })]
              .TagValue = System.Convert.ToInt32(values[i].Value);

                //Console.WriteLine("hit");

               // var message = Newtonsoft.Json.JsonConvert.SerializeObject(new MQTTMessages(values[i].ItemName.Substring(14, 9), (float)values[i].Value));
                var pressure = Newtonsoft.Json.JsonConvert.SerializeObject(Global.opclist);
                var pressure1 = Newtonsoft.Json.JsonConvert.SerializeObject(Global.opclist[0]);

                if (Program.mclient.IsConnected)
                {
                    Thread.Sleep(5000);

                    Program.mclient.Publish(Program.topic_pressure, Encoding.UTF8.GetBytes(pressure));
                    Program.mclient.Publish(Program.topic_Pressure1, Encoding.UTF8.GetBytes(pressure1));
                    Console.WriteLine(pressure);
                    //Console.ReadLine();
                }

            }
        }

    }
}