// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using System.Net;
using static Eventos;

public class datagram {
    public string? name { get; set; }
    public string? msg { get; set; }
}

class Program {
   static void Main(string[] args){
      if(ServerUDP.StartServer(IPAddress.Any.ToString(), 26950)){
         ServerUDP.OnReceivedNewDataClient += new OnReceivedNewDataClient(OnReceivedNewDataClient);
         Console.WriteLine("Servidor iniciado e hospedado na porta: {0}", 26950);
      }
      Console.ReadKey();
   }

   //========================= Evento =========================
   static void OnReceivedNewDataClient(datagram _data, IPEndPoint _client){
      Console.WriteLine("Datagram recebido de: {0}", _client.Port);
      _data.name = "SERVIDOR";
      _data.msg = "Olá :)";
      Console.WriteLine("Enviado mensagem de resposta!");
      ServerUDP.SendData(_data, _client);
   }
}