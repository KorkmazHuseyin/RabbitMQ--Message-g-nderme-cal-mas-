using RabbitMQ.Client;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace RAbbitMQ.Publisher
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
                // 1 - Kuruğa isim veriyoruz. 2- (durable property)Olurda restart olursa bilgisayar vs. Mesajlar ın kaybolmasını istemiyorsak true yaparız. Mesajlar fiziksel olarak saklanır. False yaparsak mesajlar ram de tutulur. Restrar vs olursa mesajlar gider. 3 - (Exclusive property : True: Sadece bu kalan üzerinden bağlanabilirsin. False : Kuyruğa farklı kanllardan ulaşabilmek te istiyorsan false yaparız. 4 - (AutoDelete property : True: Subscribe bir şekilde bağlantısı kopar ise kuyruk otomatik silinir. False: Subscribe bağlantısı bir nedenden kesilse bile kuyruk silinmez. ) )
                channel.QueueDeclare("hello-queue", true, false, false);


                ///Foreach döngüsü ile 1 den 50 ye kadar dönen bir döngü içinde mesajımızı 50 kez yazdırmayı deniyoruz.
                foreach (var item in Enumerable.Range(1,50))
                {

                    //RabbitMQ ya göndereceğimiz mesajı yazabiliriz. Not message ler byte [] şeklinde gönderilmektedir. Byte [] olması istediğin herşeyi gönderebileceğin anlamına geliyor. Word, pdf, dosya vs vs .
                    string message = $"Message{item}";

                    // message nin byte dizinini almamız gerekiyor bu nedenle Encoding sınıfı Utf8 dili ile byte ını alıyoruz.
                    var messageBody = Encoding.UTF8.GetBytes(message);

                    // Mesajı kuyruğa gönderme sırası geldi. Message kuyruğa kanal üzerinden gönderilir. 1 - Şuanda gönderci taraftayız o nedenle Publish i seçtik. Not: Bu örnekte Exchange kullanmıyoruz. Mesajı direk kuyruğa gönderiyoruz. 1- Önce bizden Exchange istedi. exchange kullanmadığımız için string.Empty olarak geçtik yani boş. Yani default Exchange 2 - routing Key istedi. yani gönderdiğimiz messagenin ismi. bizim mesajn ismi neyde hello-queue   3- diğer ayarlar yani  basic property kullanmıyoruz o yüzden null geçtik. 4 - sonra mesajımızın body sini istiyor Bizim mesajımızın body si messageBody idi. onu yazıyoruz.
                    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);



                    Console.WriteLine($"Mesaj gönderilmiştir : {message}");


                }



               





               


                Console.ReadKey();
            }
        }
    }
}
