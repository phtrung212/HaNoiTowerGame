using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Server
{
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class Server : Form
    {
        public int completePlayer = 0;
        public bool START = true;
        public bool EndGame = false;
        List<String> username=new List<String>();
        List<String> Code=new List<String>();
        int connectedPlayer = 0;// số lượng player đã đăng ký username

        [Serializable]
        public class Tower
        {
            public List<int> A;
            public List<int> B;
            public List<int> C;
            public Tower()
            {
                A = new List<int>();
                B = new List<int>();
                C = new List<int>();
            }
        }
        public class Player
        {
            public Tower tower = new Tower();
            public String username;
            public String Code;
            public int Score;
        }
        public Server()
        {
            InitializeComponent();
            Connect();
            
        }

        private void lvMess_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        IPEndPoint iPAddress;
        Socket server;
        List<Socket> clientList;
        List<Player> listPlayer;
        List<Player> listPlayerComplete=new List<Player>();
        void Connect()
        {
            clientList = new List<Socket>();
            listPlayer = new List<Player>();
            iPAddress = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            username=Enumerable.Repeat("", 3).ToList();
            server.Bind(iPAddress);

            Thread listen = new Thread(()=> {
                try {
                    while (true)
                    {
                        server.Listen(20);
                        if (clientList.Count <= 3)
                        {
                            Socket client = server.Accept();
                            Player player = new Player();
                            clientList.Add(client);
                            if (clientList.Count == 1)
                                txtSt1.Invoke((Action)delegate { txtSt1.Text = "Connected"; });
                            if (clientList.Count == 2)
                                txtSt2.Invoke((Action)delegate { txtSt2.Text = "Connected"; });
                            if (clientList.Count == 3)
                                txtSt3.Invoke((Action)delegate { txtSt3.Text = "Connected"; });

                            player.Code = client.RemoteEndPoint + "";
                            listPlayer.Add(player);
                            Thread receive = new Thread(Receive);
                            receive.IsBackground = true;
                            receive.Start(client);
                        }
                    }
                }
                catch
                {
                    iPAddress = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }

                
                
            });
            listen.IsBackground = true;
            listen.Start();
        }
        void close()
        {
            server.Close();
        }
        void test(string msg)
        {
            txtSt1.Text = msg;
        }
        void Send(Socket client, Tower tower)
        {
           
            String stringTower = towerToString(tower);
            client.Send(Serialize(stringTower));
        }
        void Send(Socket client, String msg)
        {
            client.Send(Serialize(msg));
        }


        
        void Receive(Object obj)
        {
            Socket client=obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024];

                    client.Receive(data);
                    
                    String msg = (String)Deserialize(data);
                    String RegisterResult = "";
                    if (msg == "END")
                    {
                        completePlayer += 1;
                        if (completePlayer == 3)
                        {
                            if (listPlayerComplete.Count != 0)
                            {
                                listPlayerComplete.Sort((x, y) => x.Score.CompareTo(y.Score));
                                client.Send(Serialize("W!Người chiến thắng là: " + listPlayerComplete[0].username + " với " + listPlayerComplete[0].Score + " lần di chuyển!"));
                            }
                            else
                                client.Send(Serialize("W!Không có người chiến thắng"));

                        }
                        msg = "";
                    }
                    if (msg[0] == 'R'&& msg[1] == '!') //Register command
                    {
                        msg=msg.Remove(0, 2);
                        foreach (var item in listPlayer)
                        {
                            if (item.username==msg)
                            {
                                RegisterResult = "R0";
                                break;
                            }
                     
                        }
                        if(RegisterResult != "R0")
                        {
                            for (int i=0; i<listPlayer.Count; i++)
                            {
                                if (listPlayer[i].Code == client.RemoteEndPoint+"")
                                {
                                    listPlayer[i].username=(msg);
                                    connectedPlayer++;
                                    RegisterResult = "R1";
                                    if(i==0)
                                    {
                                        txtUs1.Invoke((Action)delegate { txtUs1.Text = msg; });
                                    }
                                    if (i == 1)
                                    {
                                        txtUs2.Invoke((Action)delegate { txtUs2.Text = msg; });
                                    }
                                    if (i == 2)
                                    {
                                        txtUs3.Invoke((Action)delegate { txtUs3.Text = msg; });
                                    }
                                }
                            }
                        }
                        Send(client, RegisterResult);
                    }
                    if(msg[0]=='C' && msg[1]=='!')//Command message from client
                    {
                        msg = msg.Remove(0, 2);
                        string msgResult="";
                        foreach (var item in listPlayer)
                        {
                            if(item.Code==client.RemoteEndPoint+"")
                            {
                                msgResult=changeTower(msg, item);

                                break;
                            }
                        }
                        if(msgResult=="Error1")
                        {
                            client.Send(Serialize("Error1"));
                        }
                        if (msgResult == "Error2")
                        {
                            client.Send(Serialize("Error2"));
                        }
                        if (msgResult[0] == 'A')
                        {
                            client.Send(Serialize("C!"+msgResult));
                        }
                        if(EndGame)
                        {
                            EndGame = false;
                            completePlayer += 1;
                            foreach (var item in listPlayer)
                            {
                                if (item.Code == client.RemoteEndPoint + "")
                                {
                                    listPlayerComplete.Add(item);

                                    break;
                                }
                            }
                            if (completePlayer!=3)
                            {
                                client.Send(Serialize("End"));
                               
                            }else
                            {
                                if (listPlayerComplete.Count != 0)
                                {
                                    listPlayerComplete.Sort((x, y) => x.Score.CompareTo(y.Score));
                                    client.Send(Serialize("W!Trò chơi kết thúc\nNgười chiến thắng là: " + listPlayerComplete[0].username + " với " + listPlayerComplete[0].Score + " lần di chuyển!"));
                                }
                                else
                                    client.Send(Serialize("W!Không có người chiến thắng"));
                            }
                            

                        }
                    }
                    if (connectedPlayer == 3 && START)
                    {
                        startGame();
                        foreach (Socket item in clientList)
                        {

                            item.Send(Serialize("START"));
                        }
                        START = false;
                    }
                   
                }
            }
            catch {
                clientList.Remove(client);
                client.Close();
            }

        }
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
           
        }
        void startGame()
        {

            
            foreach (var item in listPlayer)
            {
                Tower tower = new Tower();
                tower = RandomTower();
                item.tower = tower;
            }
            foreach (Socket item in clientList)
            {
               
                Send(item,listPlayer[0].tower);
            }
        }
        void startGameMsg()
        {

            foreach (Socket item in clientList)
            {

                item.Send(Serialize( "START"));
            }
        }
        private void Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }
        public String towerToString(Tower tower)
        {
            String s = "A";
            foreach(int item in tower.A)
            {
                s += item;
            }
            s += "B";
            foreach (int item in tower.B)
            {
                s += item;
            }
            s += "C";
            foreach (int item in tower.C)
            {
                s += item;
            }
            return s; 
        }
        public Tower stringToTower(String towerString)
        {

            Tower tower = new Tower();
            int i = 1;
            while (towerString[i] + "" != "B")
            {
                tower.A.Add(Int32.Parse(towerString[i] + ""));
                i++;
            }
            while (tower.A.Count < 6)
                tower.A.Add(0);
            i++;
            while (towerString[i] + "" != "C")
            {
                tower.B.Add(Int32.Parse(towerString[i] + ""));
                i++;
            }
            while (tower.B.Count < 6)
                tower.B.Add(0);
            i++;
            while (i < towerString.Length)
            {
                tower.C.Add(Int32.Parse(towerString[i] + ""));
                i++;
            }
            while (tower.C.Count < 6)
                tower.C.Add(0);
            return tower;

        }
        Tower RandomTower()
        {
            Random random = new Random();
            Tower tower = new Tower();
            int[] disk = { 1, 2, 3, 4, 5, 6 };
            int mountofDisk = random.Next(3, 7);
            foreach (int item in disk)
            {
                if (item <= mountofDisk)
                {
                    int diskSelected = random.Next(1, 4);
                    if (diskSelected == 1)
                    {
                        int temp = item;
                        tower.A.Add(temp);
                    }
                    else if (diskSelected == 2)
                    {
                        int temp = item;
                        tower.B.Add(temp);
                    }
                    else
                    {
                        int temp = item;
                        tower.C.Add(temp);
                    }
                }
            }
            return tower;
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }
        String changeTower(String cmd,Player player)
        {
            String newTower = "";
            int disk = Int32.Parse(cmd[0] + "");
            char column = cmd[2];
            if(column=='A')
            {
                foreach (var item in player.tower.A)
                {
                    if (disk == item)
                        newTower = "Error1";//Đĩa đã nằm trên cột
                }
            }
            if (column == 'B')
            {
                foreach (var item in player.tower.B)
                {
                    if (disk == item)
                        newTower = "Error1";//Đĩa đã nằm trên cột
                }
            }
            if (column == 'C')
            {
                foreach (var item in player.tower.C)
                {
                    if (disk == item)
                        newTower = "Error1";//Đĩa đã nằm trên cột
                }
            }
            String diskincolumn="";
            if (newTower!="Error1")
            {
                //Xác định đĩa nằm trên cột nào
                
                foreach (var item in player.tower.A)
                {
                    if (disk == item)
                    {
                        diskincolumn = "A";
                        if (item != player.tower.A[player.tower.A.Count - 1])
                            newTower = "Error2";
                    }
                       
                }
                foreach (var item in player.tower.B)
                {
                    if (disk == item)
                    {
                        diskincolumn = "B";
                        if (item != player.tower.B[player.tower.B.Count - 1])
                            newTower = "Error2";
                    }
                }
                foreach (var item in player.tower.C)
                {
                    if (disk == item)
                    {
                        diskincolumn = "C";
                        if (item != player.tower.C[player.tower.C.Count - 1])
                            newTower = "Error2";
                    }
                       
                }
                if(newTower!="Error2")
                {
                    if (column == 'A')
                    {
                        if (player.tower.A.Count == 0)
                        {
                            player.tower.A.Add(disk);
                        }
                        else
                        {
                            if (player.tower.A[player.tower.A.Count - 1] > disk)
                            {
                                player.tower.A.Add(disk);
                            }
                            else
                                newTower = "Error2";
                        }

                    }
                    if (column == 'B')
                    {
                        if (player.tower.B.Count == 0)
                        {
                            player.tower.B.Add(disk);
                        }
                        else
                        {
                            if (player.tower.B[player.tower.B.Count - 1] > disk)
                            {
                                player.tower.B.Add(disk);
                            }
                            else
                                newTower = "Error2";
                        }

                    }
                    if (column == 'C')
                    {
                        if (player.tower.C.Count == 0)
                        {
                            player.tower.C.Add(disk);

                        }
                        else
                        {
                            if (player.tower.C[player.tower.C.Count - 1] > disk)
                            {
                                player.tower.C.Add(disk);
                            }
                            else
                                newTower = "Error2";
                        }

                    }
                }
                //di chuyển đĩa qua cột chỉ định
                
                if(newTower!="Error2")
                {
                    if(diskincolumn=="A")
                    {
                        player.tower.A.Remove(disk);
                    }
                    if (diskincolumn == "B")
                    {
                        player.tower.B.Remove(disk);
                    }
                    if (diskincolumn == "C")
                    {
                        player.tower.C.Remove(disk);
                    }
                    newTower = towerToString(player.tower);
					player.Score += 1;
                    for (int i = 0; i < listPlayer.Count; i++)
                    {
                        if (listPlayer[i].Code == player.Code)
                        {
                            if (i == 0)
                                txtSc1.Invoke((Action)delegate { txtSc1.Text = player.Score + ""; });
                            else if (i == 1)
                                txtSc2.Invoke((Action)delegate { txtSc2.Text = player.Score + ""; });
                            else
                                txtSc3.Invoke((Action)delegate { txtSc3.Text = player.Score + ""; });
                        }
                    }
                }
            }
            if((player.tower.A.Count==0 && player.tower.B.Count == 0) ||
                (player.tower.A.Count == 0 && player.tower.C.Count == 0) ||
                (player.tower.B.Count == 0 && player.tower.C.Count == 0))
            {
                EndGame = true;
                if(player.tower.A.Count == 0 && player.tower.B.Count == 0)
                {
                    for (int i = 0; i < player.tower.C.Count; i++)
                    {
                        if ((player.tower.C.Count - i) != player.tower.C[i])
                        {
                            EndGame = false;
                        }
                    }
                }

                if (player.tower.A.Count == 0 && player.tower.C.Count == 0)
                {
                    for (int i = 0; i < player.tower.B.Count; i++)
                    {
                        if ((player.tower.B.Count - i) != player.tower.B[i])
                        {
                            EndGame = false;
                        }
                    }
                }

                if (player.tower.B.Count == 0 && player.tower.C.Count == 0)
                {
                    for (int i = 0; i < player.tower.A.Count; i++)
                    {
                        if ((player.tower.A.Count - i) != player.tower.A[i])
                        {
                            EndGame = false;
                        }
                    }
                }

            }
            return newTower;
        }

    }
}
