using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();//rabbitmq bağlanmak için oluşturulur

            factory.HostName = "localhost";//rabbitmq yayın adresi verilir

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();//rabbitmq ya bağlanmak için kanal oluşturulur bu kanal üzerinden haberleşilir
            //queue ismi-false olur ise tüm herşey memoryde tutulur restart atılırsa kuryuktan silinir true olursa biryere kaydedilir-3.parametre false olur ise farklı kanaldan bağlanılabilir hale gelir-autodelete tüm subscriberlar
            //işini bitirirse kuyruk silinir bu istenmeyecek birşey
            channel.QueueDeclare("hello-rabbit", true, false, false);

            //50 adet mesaj gönderme işlemi
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Message {x}";

                //mesajlar byte dizi olarak gönderilir

                var messageBody = Encoding.UTF8.GetBytes(message);

                //mesajımızı rabbitmq ya gönderir
                channel.BasicPublish(string.Empty, "hello-rabbit", null, messageBody);

                Console.WriteLine($"Mesaj Gönderildi : {message}");
            });



            Console.ReadLine();
        }
    }
}
