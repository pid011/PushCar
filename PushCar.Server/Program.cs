using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;

internal class Program
{
    private static void Main()
    {
        // 서버 소켓이 동작하는 스레드
        var serverThread = new Thread(serverFunc);
        serverThread.IsBackground = true;
        serverThread.Start();
        Thread.Sleep(500); // 소켓 서버용 스레드가 실행될 시간을 주기 위해

        Console.WriteLine("*** 자동차 게임을 위한 게임 서버 입니다. ***");
        Console.WriteLine("종료하려면 아무 키나 누르세요...");
        Console.ReadLine();
    }

    private static void serverFunc(object obj)
    {
        var srvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var endPoint = new IPEndPoint(IPAddress.Any, 10200);

        srvSocket.Bind(endPoint);

        var recvBytes = new byte[1024];
        EndPoint clientEP = new IPEndPoint(IPAddress.None, 0);

        while (true)
        {
            var nRecv = srvSocket.ReceiveFrom(recvBytes, ref clientEP);
            var txt = Encoding.UTF8.GetString(recvBytes, 0, nRecv).Split(";;");
            Console.WriteLine("- 게임 유저가 보내온 기록 : " + txt[1]);

            var connection = new MySqlConnection("Server=localhost;Database=ckgame;Uid=root;Pwd=root;");
            try
            {
                connection.Open();
                var sqlAddCommand = new MySqlCommand($"INSERT INTO cargame VALUES ('{txt[0]}', '{txt[1]}')", connection);
                sqlAddCommand.ExecuteNonQuery();

                var sqlGetCommand = new MySqlCommand($"SELECT * FROM cargame ORDER BY result;", connection);
                var sqlReader = sqlGetCommand.ExecuteReader();

                var sb = new StringBuilder();
                while (sqlReader.Read())
                {
                    sb.AppendLine($"{sqlReader["userid"]} : {sqlReader["result"]}");
                }

                byte[] sendBytes = Encoding.UTF8.GetBytes(sb.ToString());
                srvSocket.SendTo(sendBytes, clientEP);

                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}

