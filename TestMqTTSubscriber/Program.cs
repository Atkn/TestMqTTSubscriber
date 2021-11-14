using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Text;

namespace TestMqTTSubscriber
{
    class TestClass
    {
        public string temperature { get; set; }

        public string counter { get; set; }
    }
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Subscriber!");
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();

                _options = new MqttClientOptionsBuilder()
                    .WithClientId("SubscriberId")
                    .WithTcpServer("localhost", 1884)
                    .WithCredentials("bud", "%spencer%")
                    .WithCleanSession()
                    .Build();
                _client.UseConnectedHandler(e =>
                {
                    Console.WriteLine("Connected Successfully with mqtt brokers");
                    _client.SubscribeAsync(new TopicFilterBuilder().WithTopic("state").Build()).Wait();
                });

                _client.UseDisconnectedHandler(e =>
                {
                    Console.WriteLine("Disconnected");

                });

                _client.UseApplicationMessageReceivedHandler(e =>
                {
                    var values = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    var replaceString = values.Split('/');
                    var model = JsonConvert.DeserializeObject<TestClass>(replaceString[1]);
                    //Console.WriteLine(model.Counter);
                    //Console.WriteLine(model.Temperature);
                    Console.WriteLine("values:{0}", values);
                    Console.WriteLine("Application message");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {values}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();
                });
                _client.ConnectAsync(_options).Wait();
                Console.WriteLine("Press key to exit");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            

        }
    }
}
