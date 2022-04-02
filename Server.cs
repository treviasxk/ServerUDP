// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace NetworkUDP {
   public partial class Eventos: EventArgs{
      /// <summary>
      /// O evento é chamado quando um datagrama é recebido de um Client e também será retornado um datagram como object e o endereço do Client no parâmetro da função.
      /// </summary>
      public delegate void OnReceivedNewDataClient(object _data, IPEndPoint _client);
      /// <summary>
      /// O evento é chamado quando um Client se conecta no servidor e também é retornado o endereço do Client IPEndPoint.
      /// </summary>
      public delegate void OnConnectedClient(IPEndPoint _client);
      /// <summary>
      /// O evento é chamado quando um Client se desconecta do servidor e também é retornado o endereço do Client IPEndPoint.
      /// </summary>
      public delegate void OnDisconnectedClient(IPEndPoint _client);
   }

   public class ServerUDP {
      static object datagram = new object();
      /// <summary>
      /// O evento é chamado quando um datagrama é recebido de um Client e também será retornado um datagram como object e um IPEndPoint nos parâmetros da função.
      /// </summary>
      public static event Eventos.OnReceivedNewDataClient OnReceivedNewDataClient;
      /// <summary>
      /// O evento é chamado quando um Client é conectado no servidor.
      /// </summary>
      public static event Eventos.OnConnectedClient OnConnectedClient;
      /// <summary>
      /// O evento é chamado quando um Client é deconectado do servidor.
      /// </summary>
      public static event Eventos.OnDisconnectedClient OnDisconnectedClient;
      /// <summary>
      /// Lista de todos os Clients que estão conectado no servidor.
      /// </summary>
      public static Dictionary<IPEndPoint, long> ListClients = new Dictionary<IPEndPoint, long>();
      static UdpClient Server = new UdpClient();
      /// <summary>
      /// Inicie o servidor com um IP e Porta especifico e insire a class do datagram.
      /// </summary>
      public static bool StartServer(string IP, int PORT, object _data){
         try{
            datagram = _data;
            IPEndPoint Host = new IPEndPoint(IPAddress.Parse(IP), PORT);
            Server = new UdpClient(Host);
            Server.BeginReceive(new AsyncCallback(ServerReceiveUDPCallback), null);
            Thread t = new Thread(new ThreadStart(CheckOnline));
            t.Start();
            return true;
         }
         catch{
            return false;
         }
      }
      /// <summary>
      /// Envie o datagram para um Client especifico. 
      /// </summary>
      public static bool SendData(object _data, IPEndPoint _client){
         try{
            byte[] buffer = DatagramJsonToByte(_data);
            Server.Send(buffer, buffer.Length, _client);
            return true;
         }catch{
            return false;
         }
      }
      /// <summary>
      /// Envie o datagram para todos os Clients conectado no servidor.
      /// </summary>
      public static bool SendDataAll(object _data){
         try{
            for(int i = 0; i < ListClients.Count; i++){
               byte[] buffer = DatagramJsonToByte(_data);
               Server.Send(buffer, buffer.Length, ListClients.ElementAt(i).Key);
            }
            return true;
         }catch{
            return false;
         }
      }
      static void ServerReceiveUDPCallback(IAsyncResult _result){
         IPEndPoint _client = new IPEndPoint(IPAddress.Any, 0);
         byte[] data = Server.EndReceive(_result, ref _client);
         if(ListClients.ContainsKey(_client)){
            ListClients[_client] = (DateTime.Now.Ticks / TimeSpan.TicksPerSecond);
         }else{
            ListClients.Add(_client, (DateTime.Now.Ticks / TimeSpan.TicksPerSecond));
            SendOnline(_client);
            OnConnectedClient?.Invoke(_client);
         }
         if(data.Length > 0){
            object _data = ByteJsonToDatagram(data);
            OnReceivedNewDataClient?.Invoke(_data, _client);
         }
         try{
            Server.BeginReceive(new AsyncCallback(ServerReceiveUDPCallback), null);
         }catch{
            Server.BeginReceive(new AsyncCallback(ServerReceiveUDPCallback), null);
         }
      }
      static void SendOnline(IPEndPoint _client){
         try{
            byte[] buffer = new byte[] {};
            Server.Send(buffer, buffer.Length,_client);
         }catch{}
      }
      static void CheckOnline(){
         while(true){
            for(int i = 0; i < ListClients.Count; i++){
               if(ListClients.ElementAt(i).Value + 3 < (DateTime.Now.Ticks / TimeSpan.TicksPerSecond)){
                  OnDisconnectedClient?.Invoke(ListClients.ElementAt(i).Key);
                  ListClients.Remove(ListClients.ElementAt(i).Key);
               }
            }
            Thread.Sleep(1000);
         }
      }
      static object ByteJsonToDatagram(byte[] _json){
         try{
            var utf8Reader = new Utf8JsonReader(_json);
            return JsonSerializer.Deserialize(ref utf8Reader, datagram.GetType());
         }catch{
            return null;
         }
      }
      static byte[] DatagramJsonToByte(object _data){
         try{
            return JsonSerializer.SerializeToUtf8Bytes(_data);
         }catch{
            return null;
         }
      }
   }
}