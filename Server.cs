// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

namespace NetworkUDP {
   public partial class Eventos: EventArgs{
      /// <summary>
      /// O evento é chamado quando um Datagrama é recebido de um Client e também será retornado um Datagram como object e o endereço do Client no parâmetro da função.
      /// </summary>
      public delegate void OnReceivedNewDataClient(string _text, Client _client);
      /// <summary>
      /// O evento é chamado quando um Client se conecta no servidor e também é retornado o endereço do Client IPEndPoint.
      /// </summary>
      public delegate void OnConnectedClient(Client _client);
      /// <summary>
      /// O evento é chamado quando um Client se desconecta do servidor e também é retornado o endereço do Client IPEndPoint.
      /// </summary>
      public delegate void OnDisconnectedClient(Client _client);
   }

   public class Client{
      public IPEndPoint IP;
      public float Ping;
      public long Time;
      public string PublicKeyXML = "";
   }
   public class ServerUDP {
      static UdpClient Server;
      static bool EncryptConnection;
      static string PrivateKeyXML, PublicKeyXML;
      /// <summary>
      /// O evento é chamado quando um Datagrama é recebido de um Client e também será retornado um Datagram como object e um IPEndPoint nos parâmetros da função.
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
      static List<Client> Clients = new List<Client>();
      /// <summary>
      /// Inicie o servidor com um IP e Porta especifico e insire a class do Datagram.
      /// </summary>
      public static bool StartServer(string IP, int PORT, bool _encrypt = false){
         try{
            EncryptConnection = _encrypt;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            PrivateKeyXML = RSA.ToXmlString(true);
            PublicKeyXML = RSA.ToXmlString(false);
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
      /// Envie o Datagram para um ou vários Clients. 
      /// </summary>
      public static bool SendData(string _text, Client _client){
         try{
            byte[] buffer = TextToByte(_text, EncryptConnection, _client.PublicKeyXML);
            Server.Send(buffer, buffer.Length, _client.IP);
            return true;
         }catch{
            return false;
         }
      }
      public static bool SendDataAll(string _text){
         try{
            byte[] buffer = TextToByte(_text);
            for(int i = 0; i < Clients.Count; i++){
               Server.Send(buffer, buffer.Length, Clients.ElementAt(i).IP);
            }
            return true;
         }catch{
            return false;
         }
      }
      
      static void ServerReceiveUDPCallback(IAsyncResult _result){
         IPEndPoint _ip = new IPEndPoint(IPAddress.Any, 0);
         byte[] data = Server.EndReceive(_result, ref _ip);
         Client _client = new Client();
         if(Clients.Any(Client => Client.IP.ToString() == _ip.ToString())){
            int index = Clients.FindIndex(Client => Client.IP.ToString() == _ip.ToString());
            _client = Clients.ElementAt(index);
               if(data.Length == 0){
                  _client.Ping = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _client.Time - 1000;
                  _client.Time = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
               }
            }else{
            _client = new Client() {IP = _ip, Time = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)};
            Clients.Add(_client);
            OnConnectedClient?.Invoke(_client);
            SendOnline(_ip);
         }

         if(data.Length > 0){
            string _text = ByteToText(data);
            if(_text.StartsWith("<RSAKeyValue>") && _text.EndsWith("</RSAKeyValue>")){
               _client.PublicKeyXML = _text;
            }else{
               if(_client.PublicKeyXML == "")
                  OnReceivedNewDataClient?.Invoke(_text, _client);
               else
                  OnReceivedNewDataClient?.Invoke(ByteToText(data, true), _client);
            }
         }else{
            if(_client.PublicKeyXML == "" && EncryptConnection){
               SendEncryption(_client.IP);
            }
         }
         
         try{
            Server.BeginReceive(new AsyncCallback(ServerReceiveUDPCallback), null);
         }catch{
            Server.BeginReceive(new AsyncCallback(ServerReceiveUDPCallback), null);
         }
      }
      static bool SendEncryption(IPEndPoint _ip){
         try{
            byte[] buffer = TextToByte(PublicKeyXML);
            Server.Send(buffer, buffer.Length, _ip);
            return true;
         }catch{
            return false;
         }
      }
      static void SendOnline(IPEndPoint _ip){
         try{
            byte[] buffer = new byte[] {};
            Server.Send(buffer, buffer.Length, _ip);
         }catch{}
      }
      static void CheckOnline(){
         while(true){
            for(int i = 0; i < Clients.Count; i++){
               if(Clients.ElementAt(i).Time + 3000 < (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)){
                  OnDisconnectedClient?.Invoke(Clients.ElementAt(i));
                  Clients.RemoveAt(i);
               }
            }
            Thread.Sleep(1000);
         }
      }
      static string ByteToText(byte[] _byte, bool _decrypt = false){
         if(_decrypt){
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(PrivateKeyXML);
            return Encoding.UTF8.GetString(RSA.Decrypt(_byte, true));
         }else{
            return Encoding.UTF8.GetString(_byte);
         }
      }
      static byte[] TextToByte(string _text, bool _encrypt = false, string _keyClient = ""){
         if(_encrypt){
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_keyClient);
            return rsa.Encrypt(Encoding.UTF8.GetBytes(_text), true);
         }else{
            return Encoding.UTF8.GetBytes(_text);
         }
      }
   }
}