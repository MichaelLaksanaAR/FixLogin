using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using System.Xml.Linq;
using Timer = System.Timers.Timer;
using System.Timers;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace FixLogin
{
    public partial class Form1 : Form
    {
        string LoginResult = "";
        string S21Conn = "Data Source = svrbap07; Initial Catalog = General; User ID = sa; Password = sa";
        private Socket _clientSocketBCA = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        TcpClient _socketTcpClient;
        //SslStream _sslStream;
        NetworkStream _sslStream;
        int SeqNumber = 1;

        bool TryConnect(Socket clientsocket, int port)
        {
            bool tmpval = true;
            while (!clientsocket.Connected)
            {
                try
                {
                    clientsocket.Connect(XmlPath("ServerURL"), port);
                    tmpval = true;
                    break;
                }

                catch (SocketException ex)
                {
                    ex.ToString();
                    tmpval = false;
                    break;
                }
            }
            return tmpval;
        }
        string XmlPath(string tmp)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load("path.xml");
                var xmlresult = xmlDoc.Descendants("List").Elements(tmp).Select(r => r.Value).ToArray();
                string result = string.Join("", xmlresult);
                return result;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return "";
            }

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CustDataButton_Click(object sender, EventArgs e)
        {
            if (TryConnect(_clientSocketBCA, 12500) == true)
            {
                byte[] buffer;
                string[] sqlByte;
                string sql;
                string Product;
                string CustType = "";
                string CustStatus = "";
                string CustAff;
                string AccType;

                sql = "107,99,254,2,101,100,99,117,115,2,49,51,1,49,49,2,"
                    + ",49,57,2," + GetStringByte(CustType) + ",50,48,2," + GetStringByte(CustStatus)
                    + ",50,51,2,1,50,52,2,117,112,100,97,116,101,32,102,114,111,109,32,100,97,116,97,32,109,97,105,110,116,101,110,97,110,99,101,1,3,1,13,10";

                sqlByte = sql.Split(',');

                buffer = new byte[sqlByte.Length];
                for (int i = 0; i < sqlByte.Length; i++)
                {
                    buffer[i] = Convert.ToByte(sqlByte[i]);
                }

                _clientSocketBCA.Send(buffer);
            }

            MessageBox.Show("Sent");
        }

        private string GetStringByte(string word)
        {
            char[] words;
            byte[] byteResult = new byte[word.Length];
            string stringResult = "";

            words = word.ToCharArray();

            for (int i = 0; i < words.Length; i++)
            {
                byteResult[i] = Convert.ToByte(words[i]);
            }

            for (int i = 0; i < byteResult.Length; i++)
            {
                stringResult += byteResult[i].ToString() + ",";
            }

            return stringResult.Trim(new char[] { ',' }) + ",1";
        }

        private void Login()
        {
            if (TryConnect(_clientSocketBCA, 12500) == true)
            {
                byte[] buffer = new byte[] { 107, 99, 254, 2, 97, 2, 50, 1, 53, 56, 117, 2, 97, 100, 109, 105, 110, 1, 53, 56, 112, 2, 112, 97, 115, 115, 119, 111, 114, 100, 1, 3, 1, 13, 10 };
                _clientSocketBCA.Send(buffer);

                MessageBox.Show("Sent");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (SeqNoBox.Text == "")
            {
                MessageBox.Show("Input Seq No");
                return;
            }
            else
            {
                SeqNumber = Convert.ToInt32(SeqNoBox.Text);
            }

            _socketTcpClient = new TcpClient();
            _socketTcpClient.NoDelay = true;
            _socketTcpClient.Connect("192.168.42.9", 9600);

            LoginResult = "";
            FixSendMessage(1);
            if (LoginResult == "Error")
                return;

            MessageBox.Show("Logged In");
            
            SeqNumber += 1;
            FixSendMessage(3);
            //SeqNumber += 1;
            //FixSendMessage(2);
            //Order Buy
            SeqNumber += 1;
            FixSendMessage(4);
            //Order Sell
            //SeqNumber += 1;
            //FixSendMessage(6);
            //SeqNumber += 1;
            //FixSendMessage(8);
            //SeqNumber += 1;
            //FixSendMessage(9);
            //SeqNumber += 1;
            //FixSendMessage(10);
            //SeqNumber += 1;
            //FixSendMessage(11);

            //SeqNumber += 1;
            //FixSendMessage(11);

            while (!backgroundWorker1.CancellationPending && !backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {


                SeqNumber += 1;
                FixSendMessage(2);

                SeqNumber += 1;
                FixSendMessage(3);

                Thread.Sleep(30000);
            }
        }

        private void FixSendMessage(int flag)
        {
            string prog = "";
            string response = "";
            string responseTag = "";
            var CurrDateTime = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");
            var CurrTime = DateTime.Now.ToString("HH:mm:ss:ms");
            var dt = DateTime.Now.ToString("yyyy-MM-dd");
            int resend = 0;

            string pub_s_Log = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Logs\";

            if (Directory.Exists(pub_s_Log) == false)
                Directory.CreateDirectory(pub_s_Log);

            string pub_s_LogPath = pub_s_Log + dt + ".Log";

            try
            {
                resend = 0;
                response = SendData(SeqNumber.ToString(), CurrDateTime, pub_s_LogPath, flag);
                while (response.Substring(response.LastIndexOf("35=") + 3, 1) == "2")
                {
                    prog = "1";
                    string s1 = response.Substring(response.LastIndexOf("\u00017=") + 3);
                    string s2 = "";
                    for (int i = 0; i < s1.Length; i++)
                    {
                        if (s1.Substring(i, 1) == "\u0001")
                            break;

                        s2 += s1.Substring(i, 1);
                    }

                    SeqNumber = Convert.ToInt32(s2);
                    resend += 1;
                    response = SendData(s2, CurrDateTime, pub_s_LogPath, flag);

                    //MessageBox.Show("Wrong Sequence Number");
                }

                if (response.Substring(response.LastIndexOf("35=") + 3, 1) == "5")
                {
                    if (response.Substring(response.LastIndexOf("58=") + 3, 8) == "Acceptor")
                    {
                        prog = "2";
                        string keyString = "expected sequence number ";
                        string s1 = response.Substring(response.LastIndexOf(keyString) + keyString.Length);
                        string s2 = "";
                        for (int i = 0; i < s1.Length; i++)
                        {
                            if (s1.Substring(i, 1) == ".")
                                break;

                            s2 += s1.Substring(i, 1);
                        }

                        SeqNumber = Convert.ToInt32(s2);
                        MessageBox.Show("Expected Seq Num = " + s2);
                        LoginResult = "Error";
                    }
                    else
                    {
                        LoginResult = "Error";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + " " + prog);
            }
        }

        private string SendData(string seq, string CurrTime, string path, int flag)
        {
            var fMsg = "";
            var fHeader = "";
            var fMsgLength = "";
            var fSumMsg = "";
            string ResponseData = "";

            switch (flag)
            {
                case 1: //Login
                    {
                        fMsg = "35=A#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#95=17#96=Manager1|5452344:#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 2: //Test Connection
                    {
                        fMsg = "35=1#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 3: //Heartbeat
                    {
                        fMsg = "35=0#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#112=" + CurrTime + "#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 4: //Order Buy
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=1#1=I#" +
                               "55=AALI#59=0#60=" + CurrTime + "#38=100#44=11000#65=RG#23= #8021=0001#";
                        //fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=20190109-13:39:03#11=#21=1#40=I#54=1#1=I#55=ASII#59=0#60=20190109-13:39:03#38=100#44=5000#65=RG#23= #8021=0056";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 5: //Log Out
                    {
                        fMsg = "35=5#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#58=manual disconnect#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 6: //Order Sell
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=2#1=I#" +
                               "55=AALI#59=0#60=" + CurrTime + "#38=100#44=11000#65=RG#23= #8021=1096#";
                        //fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=20190109-13:39:03#11=#21=1#40=I#54=1#1=I#55=ASII#59=0#60=20190109-13:39:03#38=100#44=5000#65=RG#23= #8021=0056";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 7: //Order Reg
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=1#1=I#" +
                               "55=CTRA#59=0#60=" + CurrTime + "#38=100#44=1260#65=RG#23= #8021=1096#";
                        //fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=20190109-13:39:03#11=#21=1#40=I#54=1#1=I#55=ASII#59=0#60=20190109-13:39:03#38=100#44=5000#65=RG#23= #8021=0056";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 8: //Order Cancel
                    {
                        fMsg = "35=F#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime +
                               "#11=AR0R000000001172#41=AR0R000000001172#37=937390275#1=I#55=CTRA#54=1#60=" + CurrTime + "#58=#";
                        //fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=20190109-13:39:03#11=#21=1#40=I#54=1#1=I#55=ASII#59=0#60=20190109-13:39:03#38=100#44=5000#65=RG#23= #8021=0056";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 9: //Amend Price
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=2#1=I#" +
                               "55=ALMI#59=0#60=" + CurrTime + "#38=200#44=194#65=RG#23= #8021=1096#";
                        //fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=20190109-13:39:03#11=#21=1#40=I#54=1#1=I#55=ASII#59=0#60=20190109-13:39:03#38=100#44=5000#65=RG#23= #8021=0056";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 10: //Amend Price
                    {
                        fMsg = "35=G#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#11=0#" +
                               "37=937390436#54=2#21=1#40=S#1=I#55=ALMI#41=AR0R000000001173#59=0#60=" + CurrTime + "#38=200#44=195#58=0#440= #432=GTD#376= #";
                        //fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=20190109-13:39:03#11=#21=1#40=I#54=1#1=I#55=ASII#59=0#60=20190109-13:39:03#38=100#44=5000#65=RG#23= #8021=0056";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 11: //Amend Price
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=1#1=I#" +
                               "55=ALMI#59=0#60=" + CurrTime + "#38=200#44=195#65=RG#23= #8021=0009#";
                        //fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=20190109-13:39:03#11=#21=1#40=I#54=1#1=I#55=ASII#59=0#60=20190109-13:39:03#38=100#44=5000#65=RG#23= #8021=0056";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 12: //Amend Qty
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=sales10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=2#1=I#55=BNLI#59=0#60=" + CurrTime + "#38=200#44=620#65=RG#23= #8021=0001#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 13: //Amend Qty
                    {
                        fMsg = "35=G#34=" + SeqNumber.ToString() + "#49=sales10#56=INT_ENGINE#52=" + CurrTime + "#11=0#37=937380695#54=2#21=1#40=S#1=I#55=BNLI#41=AR0R000000000753#59=0#60=" + CurrTime + "#38=100#44=620#58=0#440= #432=GTD#376= #";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 14: //Amend Qty
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=sales10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=1#1=I#55=BNLI#59=0#60=" + CurrTime + "#38=100#44=620#65=RG#23= #8021=1096#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 15: //Partial
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES26#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=1#1=I#" +
                               "55=BRPT#59=0#60=" + CurrTime + "#38=200#44=2600#65=RG#23= #8021=0001#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }
                case 16: //Partial
                    {
                        fMsg = "35=D#34=" + SeqNumber.ToString() + "#49=SALES10#56=INT_ENGINE#52=" + CurrTime + "#11=#21=1#40=I#54=1#1=I#" +
                               "55=ALMI#59=0#60=" + CurrTime + "#38=100#44=195#65=RG#23= #8021=0001#";
                        fHeader = "8=FIX.4.2#";
                        fMsgLength = "9=" + fMsg.Length.ToString() + "#";
                        fSumMsg = fHeader + fMsgLength + fMsg;
                        break;
                    }

            }

            fSumMsg = fSumMsg.Replace('#', '\u0001');

            int CharLength = 0;
            for (int i = 0; i < fSumMsg.Length; i++)
            {
                CharLength += Convert.ToChar(fSumMsg.Substring(i, 1));
            }
            CharLength = CharLength % 256;

            var fCheckSum = "10=" + CharLength.ToString("000") + "\u0001";
            
            var messageWithChecksum = fSumMsg + fCheckSum;

            _sslStream = _socketTcpClient.GetStream();
            byte[] buffer = new byte[2048];
            buffer = Encoding.ASCII.GetBytes(messageWithChecksum);
            _sslStream.Write(buffer, 0, buffer.Length);
            if (flag == 1)
                WriteLog(path, "\r\n" + CurrTime + ": S - " + messageWithChecksum);
            else
                WriteLog(path, CurrTime + ": S - " + messageWithChecksum);

            Int32 bytes = 0;
            buffer = new byte[2048];
            bytes = _sslStream.Read(buffer, 0, buffer.Length);
            ResponseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
            WriteLog(path, CurrTime + ": R - " + ResponseData);

            if (flag == 4)
            {
                //for (int i = 0; i < 5; i++ )
                //{
                    buffer = new byte [2048];
                    bytes = _sslStream.Read(buffer, 0, buffer.Length);
                    ResponseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
                    if (ResponseData != "")
                        WriteLog(path, CurrTime + ": R - " + ResponseData);
                //}
            }

            //buffer = new byte[8192];
            //Int32 bytes = 1;
            //while (bytes > 0)
            //{
            //    bytes = _sslStream.Read(buffer, 0, buffer.Length);
            //    if (bytes > 0)
            //    {
            //        ResponseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
            //        WriteLog(path, CurrTime + ": R - " + ResponseData);
            //    }
            //}

            return ResponseData;
        }
    

        private void WriteLog(string sLogPath, string sMessage)
        {
            StreamWriter sw;
            if (File.Exists(sLogPath) == false)
            {
                sw = File.CreateText(sLogPath);
                sw.WriteLine(sMessage);
                sw.Flush();
                sw.Close();
            }
            else
            {
                sw = File.AppendText(sLogPath);
                sw.WriteLine(sMessage);
                sw.Flush();
                sw.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string a = "'8=FIX.4.2'#1'9=85'#1'35=A'#1'34=984'#1'49=SALES10'#1'56=INT_ENGINE'#1'52=20181214-10:03:10'#1'95=17'#1'96=Manager1|5452344:'#1'10=045'#1";
            string b = "'8=FIX.4.2|9=85|35=A|34=984|49=SALES10|56=INT_ENGINE|52=20181214-10:03:10|95=17|96=Manager1|5452344:|10=045|";
            string c = "|35=A|34=984|49=SALES10|56=INT_ENGINE|52=20181214-10:03:10|95=17|96=Manager1|5452344:";
            var d = "8=FIX.4.29=8535=A34=98949=SALES1056=INT_ENGINE52=20190109-04:25:3695=1796=Manager1|5452344:";
            var d2 = "35=A34=98949=SALES1056=INT_ENGINE52=20190109-04:17:5895=1796=Manager1|5452344:";
            var f = "8=FIX.4.2#9=85#35=A#34=987#49=SALES10#56=INT_ENGINE#52=20190109-10:06:25#95=17#96=Manager1|5452344:#";
            var f2 = "35=A#34=987#49=SALES10#56=INT_ENGINE#52=20190109-10:06:25#95=17#96=Manager1|5452344:#";
            var x = "8=FIX.4.29=16135=A34=152=20180124-00:50:34.08349=426123d789fa8e5c3782c549kj9de06e56=Coinbase98=0108=30554=outswrt96=qkE5KPMLjn+Ef9Zgk1/kvL0Etem6bK2llINwMjOkDy9=8013=Y";

            f = d.Replace('#', '\u0001');
            f2 = d2.Replace('#', '\u0001');
            int CharLength = 0;
            int SumLength = f.Length;
            int SumLength2 = f2.Length;
            for (int i = 0; i < SumLength; i++)
            {
                CharLength += Convert.ToChar(f.Substring(i, 1));
            }
            CharLength = CharLength % 256;

            string key1 = "expected sequence number";
            string key2 = ". Session";
            string xyz = "expected sequence number 1025. Session";
            int seq = xyz.Length - (key1.Length + 1) - (xyz.Length - xyz.LastIndexOf(key2));
            string abc = xyz.Substring(xyz.LastIndexOf(key1) + key1.Length + 1, seq);
            //string abc = xyz.LastIndexOf(key2).ToString();

            MessageBox.Show(SumLength2.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SeqNumber += 1;
            FixSendMessage(5);
            MessageBox.Show("Logout");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _socketTcpClient = new TcpClient();
            _socketTcpClient.NoDelay = true;
            _socketTcpClient.Connect("192.168.42.9", 9600);

            SeqNumber = Convert.ToInt32(SeqNoBox.Text);
            FixSendMessage(1);
            SeqNumber += 1;            
            FixSendMessage(5);
            MessageBox.Show("Logout");
        }

        private void ReceiveFromBloomberg(string Bloomberg)
        {
            string[] BloombergTypes;
            string sql = "";
            Bloomberg = Bloomberg.Replace("\u0001", "#");
            BloombergTypes = Bloomberg.Split('#');
            string[] Type = {"35=", "34=" ,"49=", "52=", "56=", "11=", "21=", "38=", "40=", "44=", "54=", "55=", "59=", "60=", "1=", "37="};

            for (int i = 0; i <= Type.Length - 1; i++)
            {
                for (int j = 0; j <= BloombergTypes.Length - 1; j++)
                {
                    if (BloombergTypes[j].Contains(Type[i]))
                    {
                        if (sql == "")
                            sql += "'" + BloombergTypes[j].Substring(Type[i].Length) + "'";
                        else
                            sql += ", '" + BloombergTypes[j].Substring(Type[i].Length) + "'";
                    }
                    else
                    {
                        if (sql == "")
                            sql += "''";
                        else
                            sql += ", ''";
                    }
                }
            }
            InsertDB(sql);
        }
        private void InsertDB(string sql)
        {
            ARDataServices.cDataServices cds = new ARDataServices.cDataServices();
            string ssql = "INSERT INTO Bloomberg values (" + sql + ", 0, null, null)";
            cds.Open(S21Conn);
            cds.ExecuteNonQuerySql(ssql);
            cds.Close();
        }

        private void UpdateDB(string ID)
        {
            ARDataServices.cDataServices cds = new ARDataServices.cDataServices();
            string ssql = "Update Bloomberg set Processed = 1 where ID = '" + ID + "'";
            cds.Open(S21Conn);
            cds.ExecuteNonQuerySql(ssql);
            cds.Close();
        }

        private string ReadDB()
        {
            string BloombergString = "";
            ARDataServices.cDataServices cds = new ARDataServices.cDataServices();
            SqlDataReader coRdr;
            string ssql = "select top 1 * from Bloomberg where Processed = 0";
            cds.Open(S21Conn);
            coRdr = cds.GetReader(ssql);
            if (coRdr.HasRows)
            {
                while (coRdr.Read())
                {
                    BloombergString = "35=" + coRdr["35"].ToString() + "#34=" + coRdr["34"].ToString() + "#49=" + coRdr["49"].ToString() +
                                "#52=" + coRdr["52"].ToString() + "#56=" + coRdr["56"].ToString() + "#11=" + coRdr["11"].ToString() +
                                "#21=" + coRdr["21"].ToString() + "#38=" + coRdr["38"].ToString() + "#40=" + coRdr["40"].ToString() +
                                "#44=" + coRdr["44"].ToString() + "#54=" + coRdr["54"].ToString() + "#55=" + coRdr["55"].ToString() +
                                "#59=" + coRdr["59"].ToString() + "#60=" + coRdr["60"].ToString() + "#ID=" + coRdr["ID"].ToString();
                }
            }
            coRdr.Close();
            cds.Close();

            return BloombergString;
        }

        private string ConvertBloombergToPKN()
        {
            string Bloomberg = ReadDB();
            if (Bloomberg == "")
                return Bloomberg;

            //string Bloomberg = "35=D#34=5#49=BIPAUAT#52=20190115-07:13:50.008#56=BLPUAT#11=125444#21=1#38=1#40=2#44=40#54=1#55=aali#59=0#60=20190115-14:13:18.821#ID=1";
            var CurrDateTime = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");
            var CurrTime = DateTime.Now.ToString("HH:mm:ss:ms");
            var dt = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "";
            string[] BloombergTypes = Bloomberg.Split('#');
            string ID = Bloomberg.Substring(Bloomberg.IndexOf("ID=") + 3);
            string OrderType = Bloomberg.Substring(Bloomberg.IndexOf("35=") + 3, 1);
            string ClordID = "";
            string b49 = "";
            string b56 = "";
            string NewOrdNo = "";
            string AmendOrdNo = "";
            string temp = "";
            string Final = "";
            string fHeader = "";
            string fMsgLength = "";
            string fSumMsg = "";
            string ResponseData = "";
            string ResponseToBloomberg = "";
            string msgToBloomberg = "";
            string[] Type;
            string[] RegType = { "35=", "34=", "49=", "56=", "52=", "11=", "21=", "40=", "54=", "1=", "55=", "59=", "60=", "38=", "44=",
                                 "65=", "23=", "8021" };
            string[] AmendType = { "35=", "34=", "49=", "56=", "52=", "11=", "37=", "54=", "21=", "40=", "1=", "55=", "41=", "59=", "60=", 
                                   "38=", "44=", "58=", "440=", "432=", "376=" };
            string[] CancelType = { "35=", "34=", "49=", "56=", "52=", "11=", "41=", "37=", "1=", "55=", "54=", "60=", "58=" };

            UpdateDB(ID);

            string pub_s_Log = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Logs\";

            if (Directory.Exists(pub_s_Log) == false)
                Directory.CreateDirectory(pub_s_Log);

            string pub_s_LogPath = pub_s_Log + dt + ".Log";

            if (OrderType == "G")
            {
                Type = AmendType;
            }
            else if (OrderType == "F")
            {
                Type = CancelType;
            }
            else
            {
                Type = RegType;
            }

            ClordID = Array.Find(BloombergTypes, s => s.StartsWith("11=", StringComparison.Ordinal));
            ClordID = ClordID.Substring(3);
            b49 = Array.Find(BloombergTypes, s => s.StartsWith("49=", StringComparison.Ordinal));
            b49 = b49.Substring(3);
            b56 = Array.Find(BloombergTypes, s => s.StartsWith("56=", StringComparison.Ordinal));
            b56 = b56.Substring(3);

            for (int i = 0; i <= Type.Length - 1; i++)
            {
                temp = Array.Find(BloombergTypes, s => s.StartsWith(Type[i], StringComparison.Ordinal));
                if (Type[i] == "11=")
                    Final += Type[i] + "#";
                else if (Type[i] == "1=")
                    Final += Type[i] + "I#";
                else if (Type[i] == "52=" || Type[i] == "60=") //Time
                    Final += Type[i] + CurrDateTime + "#";
                else if (Type[i] == "49=") //Ex: Sales26
                    Final += Type[i] + "SALES10#";
                else if (Type[i] == "56=") //Ex: INT_ENGINE
                    Final += Type[i] + "INT_ENGINE#";
                else if (Type[i] == "34=")
                    Final += Type[i] + SeqNumber + "#";
                else if (Type[i] == "65=")
                    Final += Type[i] + "RG#";
                else if (Type[i] == "23=")
                    Final += Type[i] + " #";
                else if (Type[i] == "8021=")
                    Final += Type[i] + "0001#";
                else if (Type[i] == "58=")
                    Final += Type[i] + "0#";
                else if (Type[i] == "440=" || Type[i] == "376=")
                    Final += Type[i] + " #";
                else if (Type[i] == "432=")
                    Final += Type[i] + "GTD#";
                else
                    Final += temp + "#";
            }

            fHeader = "8=FIX.4.2#";
            fMsgLength = "9=" + Final.Length.ToString() + "#";
            fSumMsg = fHeader + fMsgLength + Final;

            fSumMsg = fSumMsg.Replace('#', '\u0001');

            int CharLength = 0;
            for (int i = 0; i < fSumMsg.Length; i++)
            {
                CharLength += Convert.ToChar(fSumMsg.Substring(i, 1));
            }
            CharLength = CharLength % 256;

            var fCheckSum = "10=" + CharLength.ToString("000") + "\u0001";

            var messageWithChecksum = fSumMsg + fCheckSum;

            msgToBloomberg = SendData2(pub_s_LogPath, CurrDateTime, messageWithChecksum, ClordID, b49, b56);

            return msgToBloomberg;
        }
        private string SendData2(string path, string CurrDateTime, string messageWithChecksum, string ClordID, string b49, string b56)
        {
            string ResponseData = "";
            string msgToBloomberg = "";

            _sslStream = _socketTcpClient.GetStream();
            byte[] buffer = new byte[2048];
            buffer = Encoding.ASCII.GetBytes(messageWithChecksum);
            _sslStream.Write(buffer, 0, buffer.Length);

            WriteLog(path, CurrDateTime + ": S - " + messageWithChecksum);

            string[] splitResult;
            Int32 bytes = 0;

            Thread.Sleep(1000);

            while (ResponseData == "")
            {
                buffer = new byte[2048];
                bytes = _sslStream.Read(buffer, 0, buffer.Length);
                ResponseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
            }
            
            while (_sslStream.DataAvailable)
            {
                buffer = new byte[2048];
                bytes = _sslStream.Read(buffer, 0, buffer.Length);
                ResponseData += System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
            }

            if (ResponseData != "")
            {
                splitResult = ResponseData.Split(new string[] { "8=FIX.4.2" }, StringSplitOptions.None);
                foreach (string word in splitResult)
                {
                    if (word != "")
                    {
                        WriteLog(path, CurrDateTime + ": R - 8=FIX.4.2" + word);
                        if (CheckResponse("8=FIX.4.2" + word) == true)
                        {
                            msgToBloomberg = ReceiveResponse(ResponseData, ClordID, b49, b56);
                            WriteLog(path, "Response For Bloomberg - 8=FIX.4.2" + msgToBloomberg);
                        }

                    }
                }
            }
            return msgToBloomberg;
        }

        private string ReceiveResponse(string Response, string ClordID, string b49, string b56) //Convert From Internal to Bloomberg
        {
            var CurrDateTime = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");
            var CurrTime = DateTime.Now.ToString("HH:mm:ss:ms");
            var dt = DateTime.Now.ToString("yyyy-MM-dd");
            string[] BloombergTypes;
            string Bloomberg = Response;
            string temp = "";
            string Final = "";
            string fHeader = "";
            string fMsgLength = "";
            string fSumMsg = "";

            Bloomberg = Bloomberg.Replace("\u0001", "#");
            BloombergTypes = Bloomberg.Split('#');
            string[] Type = {"35=", "34=", "49=", "52=", "56=", "6=", "11=", "14=", "17=", "20=", "31=",
                             "32=", "37=", "38=", "39=", "54=", "55=", "150=", "151="};

            for (int i = 0; i <= Type.Length - 1; i++)
            {
                temp = Array.Find(BloombergTypes, s => s.StartsWith(Type[i], StringComparison.Ordinal));
                if (temp == "" || Type[i] == "1=")
                    Final += Type[i] + "#";
                else if (Type[i] == "52=" || Type[i] == "60=") //Time
                    Final += Type[i] + CurrDateTime + "#";
                else if (Type[i] == "49=") //Ex: Sales26
                    Final += Type[i] + b49 + "#";
                else if (Type[i] == "56=") //Ex: INT_ENGINE
                    Final += Type[i] + b56 + "#";
                else if (Type[i] == "11=") //Ex: INT_ENGINE
                    Final += Type[i] + ClordID + "#";
                else
                    Final += temp + "#";
            }

            fHeader = "8=FIX.4.2#";
            fMsgLength = "9=" + Final.Length.ToString() + "#";
            fSumMsg = fHeader + fMsgLength + Final;

            fSumMsg = fSumMsg.Replace('#', '\u0001');

            int CharLength = 0;
            for (int i = 0; i < fSumMsg.Length; i++)
            {
                CharLength += Convert.ToChar(fSumMsg.Substring(i, 1));
            }
            CharLength = CharLength % 256;

            var fCheckSum = "10=" + CharLength.ToString("000") + "\u0001";

            var messageWithChecksum = fSumMsg + fCheckSum;

            return messageWithChecksum;
        }

        private Boolean CheckResponse(string Response)
        {    
            Boolean CheckResult = false;
            
            if (Response == "")
                return CheckResult;

            string[] checkArray = Response.Split('\u0001');
            string x = Array.Find(checkArray, s => s.StartsWith("37=", StringComparison.Ordinal));

            if (x.Length > 5)
                CheckResult = true;
            else
                CheckResult = false;

            return CheckResult;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SeqNumber = Convert.ToInt32(SeqNoBox.Text);

                _socketTcpClient = new TcpClient();
                _socketTcpClient.NoDelay = true;
                _socketTcpClient.Connect("192.168.42.9", 9600);

                LoginResult = "";
                FixSendMessage(1);
                if (LoginResult == "Error")
                    return;
                MessageBox.Show("Logged In");
                SeqNumber += 1;
                FixSendMessage(3);
                if (LoginResult == "Error")
                    return;

                SeqNumber += 1;
                string x = "";
                ReceiveFromBloomberg("8=FIX.4.29=13735=D34=549=BIPAUAT52=20190115-07:13:50.00856=BLPUAT11=12544421=138=10040=I44=1100054=155=AALI59=060=20190115-14:13:18.82110=004");
                x = ConvertBloombergToPKN();
                MessageBox.Show(x);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

    }
}
