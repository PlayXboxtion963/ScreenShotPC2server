using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public string password { get; set; } = "123456";
        Thread ftpthread;
        public Form1()
        {
            //Thread thread = new Thread(new ThreadStart(startftp));
            //thread.Start();
            InitializeComponent();
            string pathx = System.AppDomain.CurrentDomain.BaseDirectory + "\\workfloader";
            if (!Directory.Exists(pathx))//�ж��Ƿ��и��ļ�            
                Directory.CreateDirectory(pathx);

            string ipx = "192.168.0.1";
            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                ipx = localIP;
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
            textBox1.AppendText("����������");
            textBox1.AppendText(System.Environment.NewLine);

        }
        public void startftp()
        {
            FTP mftp = new FTP();
            mftp.setpassword =password;
            mftp.Startftp();
            
        }
       
        async Task Hearyouphone()
        {
            GetScreenCapture();
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
            
            WindowUtils.EnableAcrylic(this, Color.FromArgb(2, Color.DarkGray));

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
                
                client = new UdpClient(21211);
                receiveData = client.Receive(ref remotePoint);//��������
                String ip=remotePoint.Address.ToString();
                
                receiveString = Encoding.Default.GetString(receiveData);
                switch (receiveString){
                    case "shot":GetScreenCapture(); textBox1.Invoke(new EventHandler(delegate
                    {
                        textBox1.AppendText(DateTime.Now.ToLocalTime().ToString()+"��ȡȫ��" + ip);
                        textBox1.AppendText(System.Environment.NewLine);
                    }));toastit("��ȡȫ��");Logx("��ȡȫ��"+ip) ; break;
                    case "shotwindows": GetWindowCapture(); textBox1.Invoke(new EventHandler(delegate
                    {
                        textBox1.AppendText(DateTime.Now.ToLocalTime().ToString()+"��ȡ�ֲ�����" + ip);
                        textBox1.AppendText(System.Environment.NewLine);
                    })); toastit("��ȡ�ֲ�"); Logx("��ȡ�ֲ�" + ip); break;
                    default:
                        if (receiveString =="volumeup"+password)
                        {

                            keybd_event(Keys.VolumeUp, 0, 0, 0);
                        }
                        if(receiveString == "volumedown" + password)
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
                            
                            string[] words=receiveString.Split('|');
                            if (words[2] == password) {
                                String bright = words[1];//����;
                                textBox1.AppendText("��������Ϊ"+bright);
                                SetBrightness(Convert.ToByte(bright));
                                textBox1.AppendText(System.Environment.NewLine);
                                toastit("��������Ϊ"+bright);
                                Logx("��������Ϊ" + bright+ip);
                                
                            }
                        }
                        break;
                }
                
                client.Close();//�ر�����
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
                if (!File.Exists(logFileName))//�ж���־�ļ��Ƿ�Ϊ����
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
        private void GetScreenCapture()
        {
            Rectangle tScreenRect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap tSrcBmp = new Bitmap(tScreenRect.Width, tScreenRect.Height); // ������ĻԭʼͼƬ����
            Graphics gp = Graphics.FromImage(tSrcBmp);
            gp.CopyFromScreen(0, 0, 0, 0, tScreenRect.Size);
            gp.DrawImage(tSrcBmp, 0, 0, tScreenRect, GraphicsUnit.Pixel);
            String path = System.AppDomain.CurrentDomain.BaseDirectory +@"\workfloader\tempcap.bmp";
            tSrcBmp.Save(@path, System.Drawing.Imaging.ImageFormat.Bmp);
            gp = null;
            tSrcBmp = null;
            GC.Collect();

        }

        private void GetWindowCapture()
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
            
            client = new UdpClient(62231);
            while (true)
            {

                receiveData = client.Receive(ref remotePoint);//��������
                String ip = remotePoint.Address.ToString();
                string HostName = Dns.GetHostName();
                //��Hostname����ip9832
                textBox1.Invoke(new EventHandler(delegate
                {
                    textBox1.AppendText(DateTime.Now.ToLocalTime().ToString()+"�豸��������" + ip  );
                    textBox1.AppendText(System.Environment.NewLine);
                }));
                byte[] sendbytes = Encoding.Unicode.GetBytes(HostName);
                IPEndPoint remoteIpep = new IPEndPoint(remotePoint.Address, 9832); // ���͵���IP��ַ�Ͷ˿ں�
                UdpClient udpcSend = new UdpClient();
                Thread.Sleep(1500);
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
    }
}

