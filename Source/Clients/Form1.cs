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



namespace Clients
{
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class Form1 : Form
    {
        public bool END;
        public int Score = 0;
        public String step = "";//ghi lại lệnh dịch chuyển 
        public static int checkStart=1;
        String userNameClient;
        public bool Start = true;
        [Serializable]
        public class Tower
        {

            public List<int> A = new List<int>();
            public List<int> B = new List<int>();
            public List<int> C = new List<int>();
        }
        public Form1()
        {
            InitializeComponent();
            connect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(txtBox.Text!="")
            {
                if (txtBox.Text[0] > '0' && txtBox.Text[0] < '7' && txtBox.Text[1] == '-' && txtBox.Text[2] >= 'A' && txtBox.Text[2] <= 'C')
                {
                    step = txtBox.Text;
                    SendCMD();
                    txtBox.Clear();
                }
                else
                    MessageBox.Show("Cú pháp không đúng!/nVí dụ:\"1-A\" để di chuyển đĩa 1 qua cột A");
            }
             
        }
        IPEndPoint iPAddress;
        Socket client;
        void connect()
        {
            iPAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            client = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.IP);
            try {

                client.Connect(iPAddress);            }
            catch {
                MessageBox.Show("Cannot connect to server","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            lvThongBao.Items.Add(new ListViewItem() { Text = "Kết nối server thành công!" });
            lvThongBao.Items.Add(new ListViewItem() { Text = "Vui lòng đăng ký trước khi bắt đầu trò chơi!"});
            Thread listen = new Thread(receive);
            listen.IsBackground = true;
            listen.Start();
        }
        void close()
        {
            client.Close();
        }
        void SendCMD()
        {     
            if(txtBox.Text!=String.Empty)
                client.Send(Serialize("C!"+txtBox.Text));
        }
        void SendMsg (String msg)
        {
            if (msg != String.Empty)
                client.Send(Serialize(msg));
        }
        void receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024];
                    
                    client.Receive(data);
                    String dataString= (String)Deserialize(data);

                    if(dataString[0]=='W' && dataString[1]=='!')
                    {
                        dataString=dataString.Remove(0, 2);
                        MessageBox.Show(dataString+"");
                        lvThongBao.Items.Add(new ListViewItem() { Text =  dataString });

                    }
                    if (dataString[0]=='R')
                    {
                        if (dataString == "R0")
                        {
                            MessageBox.Show("Username đã có người sử dụng! Vui lòng chọn tên khác");
                        }
                          
                        else
                        {
                             lvThongBao.Items.Add(new ListViewItem() { Text = "Đăng ký thành công username:"+userNameClient+"!" });
                            lvThongBao.Items.Add(new ListViewItem() { Text = "Trò chơi sẽ tự động bắt đầu khi có đủ người chơi!" });
                            MessageBox.Show("Username của bạn là: "+userNameClient+"","Đăng ký thành công!");
                            txtUsername.Invoke((Action)delegate { txtUsername1.Text = userNameClient; });

                        }

                    }
                    if(dataString[0]=='A')
                    {
                        Tower tower = new Tower();
                 
                        tower = stringToTower(dataString);
                        towerChange(tower);
                        if (Start)
                        {
                            lvThongBao.Items.Add(new ListViewItem() { Text = "Trò chơi BẮT ĐẦU!" });
                            Start = false;
                        }

                    }
                    if (dataString == "Error1")
                        MessageBox.Show("Đĩa đang nằm trên cột!", "Lỗi");
                    if (dataString == "Error2")
                        MessageBox.Show("Đĩa di chuyển không hợp lệ!", "Lỗi");
                    if (dataString[0] == 'C' && dataString[1]=='!')
                    {
                        Score += 1;
                        if (step != "")
                            lvLichSu.Items.Add(new ListViewItem() { Text = step });
                        txtScore.Invoke((Action)delegate { txtScore.Text = Score + ""; });
                        dataString = dataString.Remove(0, 2);
                        
                        if(dataString[0]=='A')
                        {
                            Tower tower = new Tower();
                            tower = stringToTower(dataString);
                            towerChange(tower);
                        }
                        dataString = "";
                        step = "";
                    }
                    if(dataString=="End")
                    {
                        lvThongBao.Items.Add(new ListViewItem() { Text = "Trò chơi kết thúc!" });
                        
                    }
                    
                }
            }
            catch { client.Close(); }
            
        }

        private void towerChange(Tower tower)
        {
            A1.Invoke((Action)delegate { A1.Visible = false; });
            A2.Invoke((Action)delegate { A2.Visible = false; });
            A3.Invoke((Action)delegate { A3.Visible = false; });
            A4.Invoke((Action)delegate { A4.Visible = false; });
            A5.Invoke((Action)delegate { A5.Visible = false; });
            A6.Invoke((Action)delegate { A6.Visible = false; });
            B1.Invoke((Action)delegate { B1.Visible = false; });
            B2.Invoke((Action)delegate { B2.Visible = false; });
            B3.Invoke((Action)delegate { B3.Visible = false; });
            B4.Invoke((Action)delegate { B4.Visible = false; });
            B5.Invoke((Action)delegate { B5.Visible = false; });
            B6.Invoke((Action)delegate { B6.Visible = false; });
            C1.Invoke((Action)delegate { C1.Visible = false; });
            C2.Invoke((Action)delegate { C2.Visible = false; });
            C3.Invoke((Action)delegate { C3.Visible = false; });
            C4.Invoke((Action)delegate { C4.Visible = false; });
            C5.Invoke((Action)delegate { C5.Visible = false; });
            C6.Invoke((Action)delegate { C6.Visible = false; });





            if (tower.A[0] != 0)
            {
                A1.Invoke((Action)delegate { A1.Text = (tower.A[0] + ""); });
                A1.Invoke((Action)delegate { A1.Visible = true; });
            }

            if (tower.A[1] != 0)
            {
                A2.Invoke((Action)delegate { A2.Text = (tower.A[1] + ""); });
                A2.Invoke((Action)delegate { A2.Visible = true; });
            }

            if (tower.A[2] != 0)
            {
                A3.Invoke((Action)delegate { A3.Text = (tower.A[2] + ""); });
                A3.Invoke((Action)delegate { A3.Visible = true; });
            }
            if (tower.A[3] != 0)
            {
                A4.Invoke((Action)delegate { A4.Text = (tower.A[3] + ""); });
                A4.Invoke((Action)delegate { A4.Visible = true; });
            }
            if (tower.A[4] != 0)
            {
                A5.Invoke((Action)delegate { A5.Text = (tower.A[4] + ""); });
                A5.Invoke((Action)delegate { A5.Visible = true; });
            }
            if (tower.A[5] != 0)
            {
                A6.Invoke((Action)delegate { A6.Text = (tower.A[5] + ""); });
                A6.Invoke((Action)delegate { A6.Visible = true; });
            }


            if (tower.B[0] != 0)
            {
                B1.Invoke((Action)delegate { B1.Text = (tower.B[0] + ""); });
                B1.Invoke((Action)delegate { B1.Visible = true; });
            }

            if (tower.B[1] != 0)
            {
                B2.Invoke((Action)delegate { B2.Text = (tower.B[1] + ""); });
                B2.Invoke((Action)delegate { B2.Visible = true; });
            }

            if (tower.B[2] != 0)
            {
                B3.Invoke((Action)delegate { B3.Text = (tower.B[2] + ""); });
                B3.Invoke((Action)delegate { B3.Visible = true; });
            }
            if (tower.B[3] != 0)
            {
                B4.Invoke((Action)delegate { B4.Text = (tower.B[3] + ""); });
                B4.Invoke((Action)delegate { B4.Visible = true; });
            }
            if (tower.B[4] != 0)
            {
                B5.Invoke((Action)delegate { B5.Text = (tower.B[4] + ""); });
                B5.Invoke((Action)delegate { B5.Visible = true; });
            }
            if (tower.B[5] != 0)
            {
                B6.Invoke((Action)delegate { B6.Text = (tower.B[5] + ""); });
                B6.Invoke((Action)delegate { B6.Visible = true; });
            }


            if (tower.C[0] != 0)
            {
                C1.Invoke((Action)delegate { C1.Text = (tower.C[0] + ""); });
                C1.Invoke((Action)delegate { C1.Visible = true; });
            }

            if (tower.C[1] != 0)
            {
                C2.Invoke((Action)delegate { C2.Text = (tower.C[1] + ""); });
                C2.Invoke((Action)delegate { C2.Visible = true; });
            }

            if (tower.C[2] != 0)
            {
                C3.Invoke((Action)delegate { C3.Text = (tower.C[2] + ""); });
                C3.Invoke((Action)delegate { C3.Visible = true; });
            }
            if (tower.C[3] != 0)
            {
                C4.Invoke((Action)delegate { C4.Text = (tower.C[3] + ""); });
                C4.Invoke((Action)delegate { C4.Visible = true; });
            }
            if (tower.C[4] != 0)
            {
                C5.Invoke((Action)delegate { C5.Text = (tower.C[4] + ""); });
                C5.Invoke((Action)delegate { C5.Visible = true; });
            }
            if (tower.C[5] != 0)
            {
                C6.Invoke((Action)delegate { C6.Text = (tower.C[5] + ""); });
                C6.Invoke((Action)delegate { C6.Visible = true; });
            }
        }

        byte []Serialize(object obj)
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
            stream.Position = 0;
            return formatter.Deserialize(stream);
        }
        //Close connection
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            close();
            Close();
            
        }

        private void lvMess_SelectedIndexChanged(object sender, EventArgs e)
        {

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
        public String towerToString(Tower tower)
        {
            String s = "A";
            foreach (int item in tower.A)
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

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if(txtRegister.Text!="")
            {
                bool check = true;
                if (txtRegister.Text.Length > 10)
                {
                    check = false;
                }
                foreach (var item in txtRegister.Text)
                {
                    if ((item < 48) || (item >= 58 && item <= 64) || (item >= 91 && item <= 96) || item >= 123)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    SendMsg("R!" + txtRegister.Text);
                    userNameClient = txtRegister.Text + "";
                }
                else
                {
                    MessageBox.Show("Usernam bao gồm:‘a’…’z’, ‘A’…’Z’, ‘0’..’9’ và dài không quá 10 ký tự", "Username không hợp lệ");
                }
            }
            
               
            
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnEndGame_Click(object sender, EventArgs e)
        {
            lvThongBao.Items.Add(new ListViewItem() { Text = "Trò chơi kết thúc!" });
            SendMsg("END");
            END = true;
        }
    }
}
