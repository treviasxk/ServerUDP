// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using NetworkUDP;
using System.Net;
using System.Text.Json;

 class dtg {
    public string name { get; set; }
    public string msg { get; set; }
}

class Program {
   static void Main(string[] args){
      if(ServerUDP.StartServer(IPAddress.Any.ToString(), 26950)){
         Console.WriteLine("[SERVER] Servidor iniciado e hospedado na porta: {0}", 26950);
         ServerUDP.OnReceivedNewDataClient += new Eventos.OnReceivedNewDataClient(OnReceivedNewDataClient);
         ServerUDP.OnConnectedClient += new Eventos.OnConnectedClient(OnConnectedClient);
         ServerUDP.OnDisconnectedClient += new Eventos.OnDisconnectedClient(OnDisconnectedClient);
      }
   }
   //========================= Evento =========================
   static void OnReceivedNewDataClient(string _text, Client _client){
      var data = JsonSerializer.Deserialize<dtg>(_text);
      Console.WriteLine("{0}: {1}", data.name, data.msg);
      data.name = "SERVER";
      data.msg = "Olá :)";
      Console.WriteLine("{0}: {1}", data.name, data.msg);
      ServerUDP.SendData(JsonSerializer.Serialize(data), _client);
   }
   static void OnConnectedClient(Client _client){
      Console.WriteLine("[SERVER] {0} conectou no servidor.", _client.IP);
   }
   static void OnDisconnectedClient(Client _client){
      Console.WriteLine("[SERVER] {0} desconectou do servidor.", _client.IP);
   }
}