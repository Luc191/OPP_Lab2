using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

// слова для отправки для получения перевода

var sw = new Stopwatch();
sw.Start();
Client client = new Client();
client.Connect();
client.Send();
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);
class Client
{
    string[] words = new string[] { "привет", "как дела?", "что делаешь?" };
    Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public void Connect()
    {
        try
        {
            tcpClient.Connect("127.0.0.1", 8888);
        }
        catch (SocketException)
        {
            Console.WriteLine("Не удалось подключиться");
        }
    }
    public void Send()
    {
        // буфер для входящих данных
        var response = new List<byte>();
        foreach (var word in words)
        {
            // считыванием строку в массив байт
            // при отправке добавляем маркер завершения сообщения - \n
            byte[] data = Encoding.UTF8.GetBytes(word + '\n');
            // отправляем данные
            tcpClient.Send(data);

            // буфер для считывания одного байта
            var bytesRead = new byte[1];
            // считываем данные до конечного символа
            while (true)
            {
                var count = tcpClient.Receive(bytesRead);
                // смотрим, если считанный байт представляет конечный символ, выходим
                if (count == 0 || bytesRead[0] == '\n') break;
                // иначе добавляем в буфер
                response.Add(bytesRead[0]);
            }
            var translation = Encoding.UTF8.GetString(response.ToArray());
            Console.WriteLine($"Вы отправили: {word}. Вам ответили: {translation}");
            response.Clear();
        }

        // отправляем маркер завершения подключения - END
        tcpClient.Send(Encoding.UTF8.GetBytes("END\n"));
        Console.WriteLine("Все сообщения отправлены");
    }
}


