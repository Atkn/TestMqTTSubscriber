using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace TestMqttPublisher
{
    class TestClass
    {
        public int Temperature { get; set; }

        public int Counter { get; set; }
    }
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;
        static void Main(string[] args)
        {
            
            Console.WriteLine("Start Publisher!");
            try
            {
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();
                _options = new MqttClientOptionsBuilder()
                    .WithClientId("publisherId")
                    .WithTcpServer("localhost", 1884)
                    .WithCredentials("bud", "%spencer%")
                    .WithCleanSession()
                    .Build();

                //handlers
                _client.UseConnectedHandler(e =>
                {
                    Console.WriteLine("Connected successfully with MQTT Brokers.");
                });
                _client.UseDisconnectedHandler(e =>
                {
                    Console.WriteLine("Disconnected from MQTT Brokers.");
                });
                _client.UseApplicationMessageReceivedHandler(e =>
                {
                    try
                    {
                        string topic = e.ApplicationMessage.Topic;
                        if (string.IsNullOrWhiteSpace(topic) == false)
                        {
                            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                            Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, ex);
                    }
                });

                _client.ConnectAsync(_options).Wait();
                Console.WriteLine("press key to publish message");
                Console.ReadLine();
                SimulatePublish();

            }
            catch (Exception)
            {

                throw;
            }
        }
        static void SimulatePublish()
        {
            
            var counter = 0;
            var random = new Random();
            var number = random.Next(20, 30);
            var number2 = random.Next(50, 70);
            while (true)
            {
                TestClass model = new TestClass
                {
                    Counter = number,
                    Temperature = number2
                };
                string cls = JsonConvert.SerializeObject(model);
                counter++;
                var testMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("state")
                    .WithPayload(" / " + cls)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();
                if (_client.IsConnected)
                {
                    Console.WriteLine($"publishing at {DateTime.Now.ToString("dd/MM/yyyy hh:mm")}");
                    _client.PublishAsync(testMessage);
                }
                Thread.Sleep(2000);

            }
        }
    }
}
