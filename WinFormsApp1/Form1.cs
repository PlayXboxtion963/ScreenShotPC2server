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
            if (!Directory.Exists(pathx))//判断是否有该文件            
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
            ftpthread = new Thread(startftp);  //第一步  创建线程对象  并把要交给线程执行的函数  通过参数传递给线程
            ftpthread.IsBackground = true;             //第二步  配置线程
            ftpthread.Start();
            Thread udprec = new Thread(RecivceMsg);
            udprec.IsBackground = true;
            udprec.Start();
            PasswordDisplay(label2, "密码:" + password);
            Thread autosearch = new Thread(AutoSearch);
        
            autosearch.IsBackground = true;
            autosearch.Start();
            textBox1.AppendText("服务运行中");
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
            PasswordDisplay(label2, "密码为:" + Settings1.Default.Password);
            Settings1.Default.Ishiding = checkBox1.Checked;
            Settings1.Default.Save();
            textBox1.AppendText("已保存隐藏信息");
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
            //实例化一个远程端点，IP和端口可以随意指定，等调用client.Receive(ref remotePoint)时会将该端点改成真正发送端端点
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
            String receiveip = null;
            while (true)
            {
                
                client = new UdpClient(21211);
                receiveData = client.Receive(ref remotePoint);//接收数据
                String ip=remotePoint.Address.ToString();
                
                receiveString = Encoding.Default.GetString(receiveData);
                switch (receiveString){
                    case "shot":GetScreenCapture(); textBox1.Invoke(new EventHandler(delegate
                    {
                        textBox1.AppendText(DateTime.Now.ToLocalTime().ToString()+"截取全屏" + ip);
                        textBox1.AppendText(System.Environment.NewLine);
                    }));toastit("截取全屏");Logx("截取全屏"+ip) ; break;
                    case "shotwindows": GetWindowCapture(); textBox1.Invoke(new EventHandler(delegate
                    {
                        textBox1.AppendText(DateTime.Now.ToLocalTime().ToString()+"截取局部窗口" + ip);
                        textBox1.AppendText(System.Environment.NewLine);
                    })); toastit("截取局部"); Logx("截取局部" + ip); break;
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
                        if (receiveString.Contains("亮度设置为"))
                        {
                            
                            string[] words=receiveString.Split('|');
                            if (words[2] == password) {
                                String bright = words[1];//亮度;
                                textBox1.AppendText("亮度设置为"+bright);
                                SetBrightness(Convert.ToByte(bright));
                                textBox1.AppendText(System.Environment.NewLine);
                                toastit("亮度设置为"+bright);
                                Logx("亮度设置为" + bright+ip);
                                
                            }
                        }
                        break;
                }
                
                client.Close();//关闭连接
            }
        }
        private void Logx(String msg)
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\log";
                if (!Directory.Exists(path))//判断是否有该文件            
                    Directory.CreateDirectory(path);
                string logFileName = path + "\\Analysis.log";//生成日志文件
                if (!File.Exists(logFileName))//判断日志文件是否为当天
                    File.Create(logFileName).Close();//创建文件

                StreamWriter writer = File.AppendText(logFileName);//文件中添加文件流
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
                writer.WriteLine(DateTime.Now.ToString("日志记录错误HH:mm:ss") + " " + e.Message + " " + msg);
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
            Bitmap tSrcBmp = new Bitmap(tScreenRect.Width, tScreenRect.Height); // 用于屏幕原始图片保存
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
            //获取当前窗口句柄
            RECT rc = new RECT();
            GetWindowRect(GetForegroundWindow(), ref rc);
            int width = rc.Right - rc.Left; //窗口的宽度
            int height = rc.Bottom - rc.Top; //窗口的高度
            int x = rc.Left;
            int y = rc.Top;
            System.Diagnostics.Debug.WriteLine(x+"x"+y+"y");
            Rectangle tScreenRect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap tSrcBmp = new Bitmap(tScreenRect.Width, tScreenRect.Height); // 用于屏幕原始图片保存
            Graphics gp = Graphics.FromImage(tSrcBmp);
            gp.CopyFromScreen(0, 0, 0, 0, tScreenRect.Size);
            gp.DrawImage(tSrcBmp, 0, 0, tScreenRect, GraphicsUnit.Pixel);

            Rectangle cropRegion = new Rectangle(x, y, width, height);
            //创建空白画布，大小为裁剪区域大小
            Bitmap result = new Bitmap(cropRegion.Width, cropRegion.Height);
            //创建Graphics对象，并指定要在result（目标图片画布）上绘制图像
            Graphics graphics = Graphics.FromImage(result);
            //使用Graphics对象把原图指定区域图像裁剪下来并填充进刚刚创建的空白画布
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
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
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
            //实例化一个远程端点，IP和端口可以随意指定，等调用client.Receive(ref remotePoint)时会将该端点改成真正发送端端点
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
            String receiveip = null;
            
            client = new UdpClient(62231);
            while (true)
            {

                receiveData = client.Receive(ref remotePoint);//接收数据
                String ip = remotePoint.Address.ToString();
                string HostName = Dns.GetHostName();
                //把Hostname发到ip9832
                textBox1.Invoke(new EventHandler(delegate
                {
                    textBox1.AppendText(DateTime.Now.ToLocalTime().ToString()+"设备尝试搜索" + ip  );
                    textBox1.AppendText(System.Environment.NewLine);
                }));
                byte[] sendbytes = Encoding.Unicode.GetBytes(HostName);
                IPEndPoint remoteIpep = new IPEndPoint(remotePoint.Address, 9832); // 发送到的IP地址和端口号
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
            notifyIcon1.ShowBalloonTip(1000, "操作", caozuo, ToolTipIcon.Info);
            Thread.Sleep(1000);
            notifyIcon1.Visible = false;
            notifyIcon1.Visible = true;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
    }
}

