using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();//rabbitmq bağlanmak için oluşturulur

            factory.HostName = "localhost";//rabbitmq yayın adresi verilir

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();//rabbitmq ya bağlanmak için kanal oluşturulur bu kanal üzerinden haberleşilir

            //queue ismi-false olur ise tüm herşey memoryde tutulur restart atılırsa kuryuktan silinir true olursa biryere kaydedilir-
            //3.parametre false olur ise farklı kanaldan bağlanılabilir hale gelir-autodelete tüm subscriberlar
            //işini bitirirse kuyruk silinir bu istenmeyecek birşey
            channel.QueueDeclare("hello-rabbit", true, false, false);
            //bize kaç kaç değer geleceğini belirttiğimiz yerdir
            channel.BasicQos(0, 1, false);

            /*****************Okuma işlemi*****************/
            //consumer oluşturulur bu bizden bir channel ister 

            var consumer = new EventingBasicConsumer(channel);
            //2.parametre mesajın işlenme durumuna göre kuyruktan silme işlemi yapar yani :true ise siler false ise silmez işlem bitince sizin silmenizi bekler
            channel.BasicConsume("hello-rabbit", false, consumer);

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1000);//her işlemde thread 1sn uyutulur
                Console.WriteLine("Gelen Mesaj: " + message);

                channel.BasicAck(e.DeliveryTag, false);//rabbitmq ya gönderme işlemi yapılı yani işlemin başarılı olma durumuna göre silme veya bekletme işlemi yapılır
            };



            Console.ReadLine();
        }

    }
}
