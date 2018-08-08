using AlumnoLibrary;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receive
{
    public class Program
    {

        static void Main(string[] args)
        {
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "hello",
            //                         durable: false,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    var consumer = new EventingBasicConsumer(channel);
            //    consumer.Received += (model, ea) =>
            //    {
            //        var body = ea.Body;
            //        var message = Encoding.UTF8.GetString(body);
            //        Console.WriteLine(" [x] Received {0}", message);
            //    };
            //    channel.BasicConsume(queue: "hello",
            //                         autoAck: true,
            //                         consumer: consumer);

            //    Console.WriteLine(" Press [enter] to exit.");
            //    Console.ReadLine();
            //    AlumnoLibrary.Alumno al = new AlumnoLibrary.Alumno();
            CommonService commonService = new CommonService();
            IConnection connection = commonService.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            ReceiveSerialisationMessages(model);
        }

        private static void ReceiveSerialisationMessages(IModel model)
        {
            model.BasicQos(0, 1, false);
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
            model.BasicConsume(CommonService.SerialisationQueueName, false, consumer);
            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                String jsonified = Encoding.UTF8.GetString(deliveryArguments.Body);
                Alumno alumno = JsonConvert.DeserializeObject<Alumno>(jsonified);
                Console.WriteLine("Pure json: {0}", jsonified);
                Console.WriteLine("Alumno name and age: {0} {1}, {2}", alumno.Nombre, alumno.Apellidos, alumno.Edad);
                model.BasicAck(deliveryArguments.DeliveryTag, false);

                //var consumer2 = new EventingBasicConsumer(channel);
                //consumer.Received += (model, ea) =>
                //{
                //    var body = ea.Body;
                //    var message = Encoding.UTF8.GetString(body);
                //    Console.WriteLine(" [x] Received {0}", message);
                //};
                //channel.BasicConsume(queue: "hello",
                //                     autoAck: true,
                //                     consumer: consumer);
            }
        }
    }
}

