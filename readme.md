# ServerUDP
ServerUDP é um server criado em .NET Core com C#, é um projeto simples e de fácil entendimento, os dados são enviado e recebido por string entre o servidor e o client, para organizar os dados recomendo que serialize as class ou object para JSON ou XML, a conexão possui criptografia RSA de ambas conexão e é perfeito para criação de servidores de jogos como por exemplo a Unity 3D.

 ![Preview](screenshots/ServerUDP.jpg)
 ![Preview](screenshots/ClientUDP.jpg)

### Como testar
Baixe os dois projetos ([ServerUDP](https://github.com/treviasxk/ServerUDP) e [ClientUDP](https://github.com/treviasxk/ClientUDP)) e apenas compile no Visual Studio Code ou Visual Studio IDE que já funcionará como demonstração, entre a comunicação do Server para o Client.
### Utilizar na Unity
Baixe a DLL em [Releases](https://github.com/treviasxk/ServerUDP/releases), coloque a DLL dentro da pasta Assets de seu projeto Unity, assim `Assets/bin/debug/ServerUDP.dll`, Depois é só importar a biblioteca `using NetworkUDP;` nos seus scripts.
### Utilizar como referencia
Baixe o projeto que você quer referenciar ([ServerUDP](https://github.com/treviasxk/ServerUDP) ou [ClientUDP](https://github.com/treviasxk/ClientUDP)). E dentro do seu projeto utilize o comando `dotnet add reference <local>/ServerUDP`.

## Documentação

| Ações | Descrição |
|-----------|---------------|
| StartServer(String ip, Int port, Bool encrypt(opcional)) | Inicia o servidor com um IP e Porta especifico, você pode definir se a conexão é criptografado com RSA.|
| SendText(String text, IPEndPoint ip) | Envie a string para um Client especifico.|
| SendTextAll(String text) | Envie a string para todos os Clients conectado no servidor.|
| MainThread() | Utilizado para definir a thread principal que irá executar as ações do RunOnMainThread(). Coloque essa ação MainThread() dentro da função void Update() na Unity.|
| RunOnMainThread(Action ações) | Executa ações dentro da thread principal do software, é utilizado para manipular objetos 3D na Unity.|

| Client(Class) | Descrição|
|------|-----|
| IPEndPoint IP | IP do client conectado.|
| float Ping | Ping do client o tempo é definido por milissegundos.|
| long Time | Tempo em segundos em que o client atualizou a conexão. (tempo baseado com o relogio da maquina)|
| string PublicKeyXML | Chave publica RSA para descriptografar.|

| Variáveis | Descrição|
|------|-----|
| List<Client> Clients | Lista de todos os Clients que estão conectado no servidor.|

| Eventos | Descrição|
|------|-----|
| OnConnectedClient(IPEndPoint ip) | O evento é chamado quando um Client se conecta no servidor e também é retornado o endereço IP do Client.|
| OnDisconnectedClient(IPEndPoint ip) | O evento é chamado quando um Client se desconecta do servidor e também é retornado o endereço IP do Client.|
| OnReceivedNewDataClient(String text, Client client) | O evento é chamado quando uma string é recebido de um Client e também será retornado uma string e o endereço IP do Client no parâmetro da função.|