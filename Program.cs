// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using NetworkUDP;
using System.Net;

class Program {
   static void Main(string[] args){
      if(ServerUDP.StartServer(IPAddress.Any.ToString(), 26950)){
         Console.WriteLine("[SERVER] Servidor iniciado e hospedado na porta: {0}", 26950);
         ServerUDP.OnConnectedClient += OnConnectedClient;
         ServerUDP.OnDisconnectedClient += OnDisconnectedClient;
         ServerUDP.OnReceivedNewDataClient += OnReceivedNewDataClient;
      }
   }
   //========================= Events =========================
   static void OnReceivedNewDataClient(string _text, Client _client){
      Console.WriteLine("[CLIENT] {0}", _text);
      _text = "OLÁ";
      Console.WriteLine("[SERVER] {0}", _text);
      ServerUDP.SendText(_text, _client);
   }
   static void OnConnectedClient(Client _client){
      Console.WriteLine("[CLIENT] {0} conectou no servidor.", _client.IP);
   }
   static void OnDisconnectedClient(Client _client){
      Console.WriteLine("[CLIENT] {0} desconectou do servidor.", _client.IP);
   }
}

/* Script Unity Example - Network.cs
using UnityEngine;
using NetworkUDP;

public class Network : MonoBehaviour{
    void Start(){
      if(ServerUDP.StartServer(IPAddress.Any.ToString(), 26950)){
         Debug.Log("<color=green>SERVER:</color>: Servidor iniciado e hospedado na porta: 26950");
         ServerUDP.OnConnectedClient += OnConnectedClient;
         ServerUDP.OnDisconnectedClient += OnDisconnectedClient;
         ServerUDP.OnReceivedNewDataClient += OnReceivedNewDataClient;
      }
    }

    void Update() {
         ServerUDP.MainThread();
    }

//========================= Events =========================
    void OnReceivedNewDataClient(string _text, Client _client){
      Debug.Log("<color=green>MESSAGE:</color>: " + _text);
      ServerUDP.SendText("OLÁ", _client);
      ServerUDP.RunOnMainThread(() => {
         gameObject.name = _text;
      });
    }

    void OnConnectedClient(Client _client){
        Debug.Log("<color=green>CLIENT:</color>: " + _client.IP + "conectou no servidor.");
    }

    void OnDisconnectedClient(Client _client){
        Debug.Log("<color=green>CLIENT:</color>: " + _client.IP + "desconectou do servidor.");
    }
}
*/