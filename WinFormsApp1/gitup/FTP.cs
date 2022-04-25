using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FubarDev.FtpServer.AccountManagement;
using System.Security.Claims;
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

namespace WinFormsApp1
{
 
    public class FTP
    {
        public string setpassword { get; set; } = "100";
        public string ippub { get; set; } = "100";
        public void Startftp()
        {
            string ipx="192.168.0.1";
            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                ipx = localIP;
            }
            ippub= ipx;
            System.Diagnostics.Debug.WriteLine(ipx);
            // Setup dependency injection
            var services = new ServiceCollection();
            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = System.IO.Directory.GetCurrentDirectory()+ @"\workfloader");
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() 
                );
            services.Configure<FtpServerOptions>(opt => opt
            .ServerAddress = ipx);
            services.Configure<FtpServerOptions>(opt => opt
              .Port = 23235);
            CustomMembershipProvider mCustomMembershipProvider = new CustomMembershipProvider();
            mCustomMembershipProvider.CustomPassword = setpassword;
            services.AddSingleton<IMembershipProvider, CustomMembershipProvider>(CustomMembershipProvider=> mCustomMembershipProvider);

            var serviceProvider = services.BuildServiceProvider();
            // Initialize the FTP server
            var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

            // Start the FTP server
            ftpServerHost.StartAsync().Wait();
        }
 
    }
    public class CustomMembershipProvider : IMembershipProvider
    {
        
        public String CustomPassword { get; set; } ="100";
       
        public Task<MemberValidationResult> ValidateUserAsync(string username, string password)
        {
            if (username != "9=n@Yb(thyZ5" || password != CustomPassword)
                return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
            var claims = new[]
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, username),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, username),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
        };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "custom"));
            return Task.FromResult(new MemberValidationResult(MemberValidationStatus.AuthenticatedUser, user));
        }
    }
}
