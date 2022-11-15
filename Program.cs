using System.Data;
using MySql.Data.MySqlClient;
using Renci.SshNet;

internal class Program
{
    private static void Main(string[] args)
    {
        // see: https://phoenixnap.com/kb/generate-ssh-key-windows-10        
        var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var keyFilePath = profilePath + Path.DirectorySeparatorChar + ".ssh" + Path.DirectorySeparatorChar + "id_rsa";
        var key = new PrivateKeyFile(keyFilePath);
        using (var sshClient = new SshClient("server", 22, "user", key))
        {
            sshClient.Connect();
            if (sshClient.IsConnected)
            {

                var forwardedPortLocal = new ForwardedPortLocal("127.0.0.1", "127.0.0.1", 3306);
                sshClient.AddForwardedPort(forwardedPortLocal);
                forwardedPortLocal.Start();
                var port = forwardedPortLocal.BoundPort;
                string db = $"Server=127.0.0.1;Port={port};Database=;Uid=;Pwd=;";
                {
                    var mySQLCon = new MySqlConnection(db);

                    mySQLCon.Open();

                    if (mySQLCon.State == ConnectionState.Open)
                    {
                        Console.WriteLine("connected");
                    }
                }
                Console.WriteLine(port);

            }
        }
    }
}