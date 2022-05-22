using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using NetFwTypeLib;
using System.Security.Principal;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public string password { get; set; } = "123456";
        Thread ftpthread;
        string ipx = "192.168.0.1";
        public Form1()
        {
            //Thread thread = new Thread(new ThreadStart(startftp));
            //thread.Start();
            InitializeComponent();
            string pathx = System.AppDomain.CurrentDomain.BaseDirectory + "\\workfloader";
            if (!Directory.Exists(pathx))//�ж��Ƿ��и��ļ�            
                Directory.CreateDirectory(pathx);

            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                ipx = localIP;
            }

            if (Settings1.Default.Isusingselfselect == true)
            {
                ipx = Settings1.Default.ip;
                checkBox2.Checked = true;
               
            }
            else
            {
                checkBox2.Checked = false;
            }

            password = Settings1.Default.Password;
            textBox2.Text = password;
            checkBox1.Checked = Settings1.Default.Ishiding;
            
            PasswordDisplay(label3, "IP:" + ipx);
            ftpthread = new Thread(startftp);  //��һ��  �����̶߳���  ����Ҫ�����߳�ִ�еĺ���  ͨ���������ݸ��߳�
            ftpthread.IsBackground = true;             //�ڶ���  �����߳�
            ftpthread.Start();
            Thread udprec = new Thread(RecivceMsg);
            udprec.IsBackground = true;
            udprec.Start();
            PasswordDisplay(label2, "����:" + password);
            Thread autosearch = new Thread(AutoSearch);
        
            autosearch.IsBackground = true;
            autosearch.Start();
            textBox1.AppendText("����������,˫���˴��鿴����Ŀ¼");
            textBox1.AppendText(System.Environment.NewLine);
            getnetip();
        }
        public void startftp()
        {
            FTP mftp = new FTP();
            mftp.setpassword =password;
            mftp.ippub = ipx;
            mftp.Startftp();
            
        }
       
        
        
        
        private  void getnetip()
        {
            //��ȡ˵��������Ϣ
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            IList<string> list = new List<string>();
            foreach (NetworkInterface adapter in nics)
            {
                
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    //��ȡ������ַ��
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                    //InterNetwork    IPV4��ַ      InterNetworkV6        IPV6��ַ
                    //Max            MAX λַ
                    if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                    //�ж��Ƿ�Ϊipv4
                    {
                        System.Diagnostics.Debug.WriteLine(ipadd.Address.ToString());
                        list.Add(ipadd.Address.ToString());
                    }      
                    

                    }
                  
            }
            Ipbox.DataSource = list;
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void PasswordDisplay(Label lblDisplay,String txtInput)
        {
            var lblWidth = lblDisplay.Width;
            var fontSize = lblDisplay.Font.Size;
            var fontStyle = lblDisplay.Font.Style;
            string content = txtInput;
            FontFamily ff = new FontFamily(lblDisplay.Font.Name);
            lblDisplay.Font = new Font(ff, fontSize, fontStyle, GraphicsUnit.World);
            float size = lblDisplay.Font.Size;

            lblDisplay.AutoSize = true;
            lblDisplay.Text = content;
            while (lblDisplay.Width > lblWidth)
            {
                size -= 0.25F;
                lblDisplay.Font = new Font(ff, size, fontStyle, GraphicsUnit.World);
            }
            lblDisplay.AutoSize = false;
            lblDisplay.Width = lblWidth;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            Settings1.Default.Password = textBox2.Text;
            Settings1.Default.Save();
            PasswordDisplay(label2, "����Ϊ:" + Settings1.Default.Password);
            Settings1.Default.Ishiding = checkBox1.Checked;
            Settings1.Default.Save();
            textBox1.AppendText("�ѱ���������Ϣ");
            textBox1.AppendText(System.Environment.NewLine);
            Settings1.Default.Isusingselfselect = checkBox2.Checked;
            Settings1.Default.Save();
            Settings1.Default.ip = Ipbox.Text;
            Settings1.Default.Save();
            Application.Restart();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Settings1.Default.Ishiding)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.Hide();
                    this.Opacity = 1;
                }));
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {

            WindowUtils.EnableAcrylic(this, Color.FromArgb(0, Color.Orange));

            base.OnHandleCreated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Transparent);
        }


        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        private void RecivceMsg()
        {
            CheckForIllegalCrossThreadCalls = false;
            UdpClient client = null;
            string receiveString = null;
            byte[] receiveData = null;
            //ʵ����һ��Զ�̶˵㣬IP�Ͷ˿ڿ�������ָ�����ȵ���client.Receive(ref remotePoint)ʱ�Ὣ�ö˵�ĳ��������Ͷ˶˵�
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
            String receiveip = null;
            while (true)
            {
                try
                {
                    client = new UdpClient(21211);
                    receiveData = client.Receive(ref remotePoint);//��������
                    String ip = remotePoint.Address.ToString();

                    receiveString = Encoding.Default.GetString(receiveData);
                    switch (receiveString)
                    {
                        case "shot":
                            try
                            {  
                                GetScreenCapture(ip);
                                
                            }
                            catch (Exception) { }; 
                            textBox1.Invoke(new EventHandler(delegate
                            {
                                textBox1.AppendText(DateTime.Now.ToLocalTime().ToString() + "��ȡȫ��" + ip);
                                textBox1.AppendText(System.Environment.NewLine);
                            })); toastit("��ȡȫ��"); Logx("��ȡȫ��" + ip); break;
                        case "shotwindows":
                            try { GetWindowCapture(ip); }catch(Exception) {  };
                             textBox1.Invoke(new EventHandler(delegate
                            {
                                textBox1.AppendText(DateTime.Now.ToLocalTime().ToString() + "��ȡ�ֲ�����" + ip);
                                textBox1.AppendText(System.Environment.NewLine);
                            })); toastit("��ȡ�ֲ�"); Logx("��ȡ�ֲ�" + ip); break;
                        default:
                            if (receiveString == "volumeup" + password)
                            {

                                keybd_event(Keys.VolumeUp, 0, 0, 0);
                            }
                            if (receiveString == "volumedown" + password)
                            {
                                keybd_event(Keys.VolumeDown, 0, 0, 0);

                            }
                            if (receiveString == "mute" + password)
                            {
                                keybd_event(Keys.VolumeMute, 0, 0, 0);

                            }
                            if (receiveString == "premusic" + password)
                            {
                                keybd_event(Keys.MediaPreviousTrack, 0, 0, 0);

                            }
                            if (receiveString == "nextmusic" + password)
                            {
                                keybd_event(Keys.MediaNextTrack, 0, 0, 0);

                            }
                            if (receiveString == "pause" + password)
                            {
                                keybd_event(Keys.MediaPlayPause, 0, 0, 0);

                            }
                            if (receiveString == "taskmanager" + password)
                            {
                                keybd_event(Keys.LControlKey, 0, 0, 0);
                                keybd_event(Keys.LShiftKey, 0, 0, 0);
                                keybd_event(Keys.Escape, 0, 0, 0);
                                keybd_event(Keys.ControlKey, 0, 2, 0);
                                keybd_event(Keys.LShiftKey, 0, 2, 0);

                            }
                            if (receiveString.Contains("��������Ϊ"))
                            {

                                string[] words = receiveString.Split('|');
                                if (words[2] == password)
                                {
                                    String bright = words[1];//����;
                                    textBox1.AppendText("��������Ϊ" + bright);
                                    try
                                    {
                                        SetBrightness(Convert.ToByte(bright));
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("��֧�����ȵ���");
                                    }

                                    textBox1.AppendText(System.Environment.NewLine);
                                    toastit("��������Ϊ" + bright);
                                    Logx("��������Ϊ" + bright + ip);

                                }
                            }
                            break;
                    }

                    client.Close();//�ر�����
                }
                catch (Exception)
                {
                    MessageBox.Show("����ʧ�ܣ�����21211�˿��Ƿ�ռ��");
                    System.Environment.Exit(0);
                }
                
                
            }
        }
        private void Logx(String msg)
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\log";
                if (!Directory.Exists(path))//�ж��Ƿ��и��ļ�            
                    Directory.CreateDirectory(path);
                string logFileName = path + "\\Analysis.log";//������־�ļ�
                if (!File.Exists(logFileName))
                    File.Create(logFileName).Close();//�����ļ�

                StreamWriter writer = File.AppendText(logFileName);//�ļ�������ļ���
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg);
                writer.Flush();
                writer.Close();
            }
            catch (Exception e)
            {
                string path = Path.Combine("./log");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string logFileName = path + "\\Analysis" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                if (!File.Exists(logFileName))
                    File.Create(logFileName);
                StreamWriter writer = File.AppendText(logFileName);
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now.ToString("��־��¼����HH:mm:ss") + " " + e.Message + " " + msg);
                writer.Flush();
                writer.Close();
            }
        }
        static void SetBrightness(byte targetBrightness)
        {
            ManagementScope scope = new ManagementScope("root\\WMI");
            SelectQuery query = new SelectQuery("WmiMonitorBrightnessMethods");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    foreach (ManagementObject mObj in objectCollection)
                    {
                        mObj.InvokeMethod("WmiSetBrightness",
                            new Object[] { UInt32.MaxValue, targetBrightness });
                        break;
                    }
                }
            }
        }
        private void GetScreenCapture(String ip)
        {
            Rectangle tScreenRect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap tSrcBmp = new Bitmap(tScreenRect.Width, tScreenRect.Height); // ������ĻԭʼͼƬ����
            Graphics gp = Graphics.FromImage(tSrcBmp);
            gp.CopyFromScreen(0, 0, 0, 0, tScreenRect.Size);
            gp.DrawImage(tSrcBmp, 0, 0, tScreenRect, GraphicsUnit.Pixel);
            String path = System.AppDomain.CurrentDomain.BaseDirectory + @"\workfloader\tempcap.bmp";
            tSrcBmp.Save(@path, System.Drawing.Imaging.ImageFormat.Bmp);
            gp = null;
            tSrcBmp = null;
            GC.Collect();
            Thread.Sleep(50);
            /*byte[] sendbytes = Encoding.Unicode.GetBytes("fin");
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(ip), 61111); // ���͵���IP��ַ�Ͷ˿ں�
            UdpClient udpcSend = new UdpClient();
            udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();*/
            try {
                Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EndPoint point = new IPEndPoint(IPAddress.Parse(ip), 61123);
                tcpClient.Connect(point);//ͨ��IP�Ͷ˿ں�����λһ����Ҫ���ӵķ�������
                tcpClient.Send(Encoding.UTF8.GetBytes("1"));
                tcpClient.Close();
            }
            catch (Exception)
            {
                
            }            


        }

        private void GetWindowCapture(String ip)
        {
            //��ȡ��ǰ���ھ��
            RECT rc = new RECT();
            GetWindowRect(GetForegroundWindow(), ref rc);
            int width = rc.Right - rc.Left; //���ڵĿ��
            int height = rc.Bottom - rc.Top; //���ڵĸ߶�
            int x = rc.Left;
            int y = rc.Top;
            System.Diagnostics.Debug.WriteLine(x+"x"+y+"y");
            Rectangle tScreenRect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap tSrcBmp = new Bitmap(tScreenRect.Width, tScreenRect.Height); // ������ĻԭʼͼƬ����
            Graphics gp = Graphics.FromImage(tSrcBmp);
            gp.CopyFromScreen(0, 0, 0, 0, tScreenRect.Size);
            gp.DrawImage(tSrcBmp, 0, 0, tScreenRect, GraphicsUnit.Pixel);

            Rectangle cropRegion = new Rectangle(x, y, width, height);
            //�����հ׻�������СΪ�ü������С
            Bitmap result = new Bitmap(cropRegion.Width, cropRegion.Height);
            //����Graphics���󣬲�ָ��Ҫ��result��Ŀ��ͼƬ�������ϻ���ͼ��
            Graphics graphics = Graphics.FromImage(result);
            //ʹ��Graphics�����ԭͼָ������ͼ��ü������������ոմ����Ŀհ׻���
            graphics.DrawImage(tSrcBmp, new Rectangle(0, 0, cropRegion.Width, cropRegion.Height), cropRegion, GraphicsUnit.Pixel);
            String path = System.AppDomain.CurrentDomain.BaseDirectory + @"\workfloader\tempcap.bmp";
            result.Save(@path, System.Drawing.Imaging.ImageFormat.Bmp);
            tSrcBmp = null;
            gp = null;
            result = null;
            graphics = null;
            GC.Collect();
            Thread.Sleep(50);
            Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint point = new IPEndPoint(IPAddress.Parse(ip), 61123);
            tcpClient.Connect(point);//ͨ��IP�Ͷ˿ں�����λһ����Ҫ���ӵķ�������
            tcpClient.Send(Encoding.UTF8.GetBytes("1"));
            tcpClient.Close();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //��������
            public int Top; //��������
            public int Right; //��������
            public int Bottom; //��������
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        public void AutoSearch()
        {
            CheckForIllegalCrossThreadCalls = false;
            UdpClient client = null;
            string receiveString = null;
            byte[] receiveData = null;
            //ʵ����һ��Զ�̶˵㣬IP�Ͷ˿ڿ�������ָ�����ȵ���client.Receive(ref remotePoint)ʱ�Ὣ�ö˵�ĳ��������Ͷ˶˵�
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
            String receiveip = null;
            Boolean canberun = true;
            try { client = new UdpClient(58974); }
            catch (SocketException e)
            {
                MessageBox.Show("�Զ����ַ�������ʧ��,�ɳ�����������:58974�˿�ռ��");
                // recover from exception
                canberun = false;

            }
            
                while (canberun)
                {

                    receiveData = client.Receive(ref remotePoint);//��������
                    String ip = remotePoint.Address.ToString();
                    string HostName = Dns.GetHostName();
                    //��Hostname����ip9832
                    textBox1.Invoke(new EventHandler(delegate
                    {
                        textBox1.AppendText(DateTime.Now.ToLocalTime().ToString() + "�豸��������" + ip);
                        textBox1.AppendText(System.Environment.NewLine);
                    }));
                    byte[] sendbytes = Encoding.Unicode.GetBytes(HostName);
                    IPEndPoint remoteIpep = new IPEndPoint(remotePoint.Address, 9832); // ���͵���IP��ַ�Ͷ˿ں�
                    UdpClient udpcSend = new UdpClient();
                    Thread.Sleep(50);
                    udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
                    udpcSend.Close();
                }
            

            
              
         }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.FlatStyle = FlatStyle.Popup;

            if (checkBox1.Checked == false)
            {
                checkBox1.ForeColor = Color.White;
               
            }
            else
            {
                checkBox1.ForeColor = Color.DarkGreen;
                
            }

        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void showui_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void hideui_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void toastit(String caozuo)
        {
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(1000, "����", caozuo, ToolTipIcon.Info);
            Thread.Sleep(1000);
            notifyIcon1.Visible = false;
            notifyIcon1.Visible = true;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory ;
            string logFileName = path + "\\Analysis.log";
            System.Diagnostics.Process.Start("explorer.exe",path);
        }

        private void Ipbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// ��Ӧ�ó�����ӵ�����ǽ����
        /// </summary>
        /// <param name="name">Ӧ�ó�������</param>
        /// <param name="executablePath">Ӧ�ó����ִ���ļ�ȫ·��</param>
        public static void NetFwAddApps(string ruleName, string appName)
        {
           string FwMgr = "HNetCfg.FwMgr";
         string FwApp = "HNetCfg.FwAuthorizedApplication";
         string FwPolicy = "HNetCfg.FwPolicy2";
        string FwRule = "HNetCfg.FWRule";


            //����firewall�������ʵ��
            var policy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID(FwPolicy));

            // Inbound Rule
            var ruleIn = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID(FwRule));

            ruleIn.Name = ruleName;
            ruleIn.ApplicationName = appName;
            ruleIn.Enabled = true;

            policy.Rules.Add(ruleIn);

            // Outbound Rule
            var ruleOut = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID(FwRule));

            ruleOut.Name = ruleName;
            ruleOut.ApplicationName = appName;
            ruleOut.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
           
            ruleOut.Enabled = true;

           
            policy.Rules.Add(ruleOut);
            MessageBox.Show("���óɹ�");


        }
      

        private void button1_Click_3(object sender, EventArgs e)
        {

            if (IsAdministrator()) {
                try
                {
                    NetFwAddApps("ScreenShotPc", System.Windows.Forms.Application.ExecutablePath);
                }
                catch (Exception)
                {
                    MessageBox.Show("����ʧ��");
                }
               
            }
            else
            {
                MessageBox.Show("���Թ���ԱȨ�����к�����Զ����÷���ǽ");
            }
           
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole����ö�ٳ��ܶ�Ȩ�ޣ�����ϵͳ�û���User��Guest�ȵ�
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}

