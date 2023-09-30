using Microsoft.AspNet.SignalR.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RAbbitMQ.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {

            //RabbitMq ile bağlantı kurabilmek için ConnectionFactory sınıfını çağırıyoruz.
            var factory = new ConnectionFactory();

            //RAbbitMq ya hanig uri üzerinden bağlanacaksan uri ını da veriyoruz.
            factory.Uri = new Uri("amqps://vxdvxesj:VT6vpW9ieHhgHroI29tDgiM2yqHaXezh@toad.rmq.cloudamqp.com/vxdvxesj");


            // Bağlantı Uri inide verdikten sonra bağlantı açma işlemini gerçekleştirebiliriz. Using scope ları içinde yapıyoruz. Amacı scope bittğinde bağlatıda kesilecek. Güvenlik!!!
            using (var connnection = factory.CreateConnection())
            {
                // Bağlantı işlemi bir kanal üzerinden yapılıyor using bloğu içinde bir kanal açıyoruz. Bu kanal üzerinden RabbitMq ile haberleşebiliyoruz.

                var channel = connnection.CreateModel();

                //Kanal oluştu. RabbitMQ ya bir mesaj gönderebilmek için kuyruğa ihtiycaım var. Kuyruk olmazsa mesajlar boşa gider. O yüzden kuyruk oluşuruyoruz.
                // QueueDeclare=> Kuyru oluşturmamızı sağlayan metodumuz.
                // 1 - Kuruğa isim veriyoruz. 2- (durable property)Olurda restart olursa bilgisayar vs. Mesajlar ın kaybolmasını istemiyorsak true yaparız. Mesajlar fiziksel olarak saklanır. False yaparsak mesajlar ram de tutulur. Restart vs olursa mesajlar gider. 3 - (Exclusive property : True: Sadece bu kanal üzerinden bağlanabilirsin. False : Kuyruğa farklı kanllardan ulaşabilmek te istiyorsan false yaparız. 4 - (AutoDelete property : True: Subscribe bir şekilde bağlantısı kopar ise kuyruk otomatik silinir. False: Subscribe bağlantısı bir nedenden kesilse bile kuyruk silinmez. ) )
                channel.QueueDeclare("hello-queue", true, false, false);




                /// Subcriber a her defasında kaç adet mesj gönderileceğini belirliyoruz. Örneğimizde 0 dan başlayarak her defasında 1 adet gönder diyoruz.Overloadda 3. propertinin FALSE : yaparsak her defasında bir subcriber a kaç adet gönderileceğini belirtiriz. TRUE : yaparsak Toplam kaç subcriber var ise o kadar subcriber a her defasında kaç adet toplamda kaç adet mesajın gideceğini belirtmiş oluruz.
                channel.BasicQos(0,1,false);

                // Subscribe/ cunsomer tarafınız yazıyoruz. Yani alıcı tarafı yazıyoruz. Dinleme işinin hangi kanaldan yapılacağını belirticez.
                var consumer = new EventingBasicConsumer(channel);

                // Bu seferde cunsomer hangi kuyruğu dinlicek onu yazıcaz.  1 - queue property: (channel dan gelen hangi kuyruk dinlenecek) Kuyruğumuzun ismini istedi. 2- autoAck : True: Mesaj gönderildikten sonra mesaj doğrudan işlense yanlışta işlense kuyruktan siler. False: True nin tersi. 3- Consumer Dinlediğimiz kanalı temsil eden değişkeni yazdım.

                //channel.BasicConsume("hello-queue",true,consumer);




                //Overload da 2. property yi false yaparsak eğer RabbitMq da mesaj doğru işlenmişse mesaj silinebilir diye dön demiş oluyoruz. Bu durumuda     channel.BasicAck(e.DeliveryTag, false);  Kod satırı ile sağlıyoruz.
                channel.BasicConsume("hello-queue",false,consumer);






                consumer.Received += (object sender, BasicDeliverEventArgs e)=> 
                {
                    var message = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine("Gelen Mesaj =======>" + message);


                    // Bu kod satırı ile RabbitMQ ya gelen mesajın Tag ını işledim diye bildir demiş oluyoruz. Silinmesi için Overloadda ki 2 property ile True : dersek işlenmeyen mesajlarıda bildir demiş oluruz False: dersek işlenen mesajları bildir demiş oluruz.
                    channel.BasicAck(e.DeliveryTag, false);
                };



            }
            Console.ReadKey();
        }

       
    }
}

