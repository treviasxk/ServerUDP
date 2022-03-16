// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using NetworkUDP;
using System.Net;
using static NetworkUDP.Eventos;

public class dtg {
    public string? name { get; set; }
    public string? msg { get; set; }
}

class Program {
   static void Main(string[] args){
      if(ServerUDP.StartServer(IPAddress.Any.ToString(), 26950, new dtg())){
         ServerUDP.OnReceivedNewDataClient += new OnReceivedNewDataClient(OnReceivedNewDataClient);
         Console.WriteLine("Servidor iniciado e hospedado na porta: {0}", 26950);
      }
      Console.ReadKey();
   }

   //========================= Evento =========================
   static void OnReceivedNewDataClient(object _data, IPEndPoint _client){
      var data = (dtg)_data;
      Console.WriteLine("Datagram recebido de: {0}", _client.Port);
      data.name = "SERVIDOR";
      data.msg = "Olá :)";
      Console.WriteLine("Enviado mensagem de resposta!");
      ServerUDP.SendData(_data, _client);
   }
}