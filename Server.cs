using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Serialization;

public partial class Eventos: EventArgs{
   public delegate void OnReceivedNewDataClient(datagram _data, IPEndPoint _client);
}

public class ServerUDP {
   public static event Eventos.OnReceivedNewDataClient? OnReceivedNewDataClient;
   static UdpClient Server = new UdpClient();
   public static bool StartServer(string IP, int PORT){
      try{
         IPEndPoint Host = new IPEndPoint(IPAddress.Parse(IP), PORT);
         Server = new UdpClient(Host);
         Server.BeginReceive(new AsyncCallback(ServerReceiveUDPCallback), null);
         return true;
      }
      catch{
         return false;
      }
   }
   static void ServerReceiveUDPCallback(IAsyncResult _result){
      IPEndPoint? _client = new IPEndPoint(IPAddress.Any, 0);
      datagram _data = XmlToDatagram(Encoding.UTF8.GetString(Server.EndReceive(_result, ref _client)));
      if(_data != null && _client != null){
         OnReceivedNewDataClient?.Invoke(_data, _client);
      }
      Server.BeginReceive(new AsyncCallback(ServerReceiveUDPCallback), null);
   }
   public static void SendData(datagram _data, IPEndPoint _client){
      byte[] buffer = Encoding.UTF8.GetBytes(DatagramToXml(_data));
      Server.Send(buffer, buffer.Length, _client);
   }

   static datagram XmlToDatagram(string _xml){
      XmlSerializer x = new XmlSerializer(typeof(datagram));
      var _data = (datagram?)x.Deserialize(new StringReader(_xml));
     if(_data != null){
         return _data;
      }else{
         return new datagram();
      }
   }

   static string DatagramToXml(datagram _data){
      XmlSerializer x = new XmlSerializer(typeof(datagram));
      StringWriter _xml = new StringWriter();
      var textWriter = XmlWriter.Create(_xml);
      x.Serialize(textWriter, _data);
      return _xml.ToString();
   }
}