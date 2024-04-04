using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;


Server server = new Server();
server.Start();
server.Receive();

class Server
{
    IPEndPoint ep = new IPEndPoint(IPAddress.Any, 8888);
    Socket tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    Dictionary<string, string> words = new Dictionary<string, string>()
    {
       { "привет", "и тебе привет" },
       { "как дела?", "нормально" },
       { "пока", "и тебе пока" },
    };
    public void Start()
    {
        tcpListener.Bind(ep);
        tcpListener.Listen();    // запускаем сервер
        Console.WriteLine("Сервер запущен. Ожидание подключений... ");
    }
    public void Receive()
    {
        try
        {

            while (true)
            {
                // получаем подключение в виде TcpClient
                using var tcpClient = tcpListener.Accept();

                // буфер для накопления входящих данных
                var response = new List<byte>();
                // буфер для считывания одного байта
                var bytesRead = new byte[1];
                while (true)
                {
                    // считываем данные до конечного символа
                    while (true)
                    {
                        var count = tcpClient.Receive(bytesRead);
                        // смотрим, если считанный байт представляет конечный символ, выходим
                        if (count == 0 || bytesRead[0] == '\n') break;
                        // иначе добавляем в буфер
                        response.Add(bytesRead[0]);
                    }
                    var word = Encoding.UTF8.GetString(response.ToArray());
                    // если прислан маркер окончания взаимодействия,
                    // выходим из цикла и завершаем взаимодействие с клиентом
                    if (word == "END") break;

                    Console.WriteLine($"Вам отправили: {word}");
                    // находим слово в словаре и отправляем обратно клиенту
                    if (!words.TryGetValue(word, out var translation)) translation = "Не знаю как ответить :(";
                    // добавляем символ окончания сообщения 
                    translation += '\n';
                    // отправляем перевод слова из словаря
                    tcpClient.Send(Encoding.UTF8.GetBytes(translation));
                    response.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}