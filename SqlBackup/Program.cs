using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace SqlBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3) return;
            
            string serverName = args[0];
            string database = args[1];
            string filePath;

            Server srv = new Server(@serverName);
            if (args.Length == 3)
            {
                filePath = args[2];
            }
            else
            {
                filePath = srv.BackupDirectory;
            }

            Backup backup = new Backup();

            // specify what you want to backup
            backup.Action = BackupActionType.Database;

            // specify the name of the database
            backup.Database = database;

            // specify what kind of devides to use, in this example we are using the File Device
            backup.Devices.AddDevice(@filePath, DeviceType.File);
            backup.BackupSetName = "Example database backup";
            backup.BackupSetDescription = "Example database full backup";

            // setting the expiration date
            backup.ExpirationDate = DateTime.Today.AddDays(10);

            // setting incremental backup
            backup.Incremental = true;

            // events called to show progress and when the backup finish
            backup.PercentComplete += CompletionStatusInPercent;
            backup.Complete += Backup_Completed;

            // start backup, this method is asynchronous
            backup.SqlBackup(srv);
        }

        private static void CompletionStatusInPercent(object sender, PercentCompleteEventArgs args)
        {
            Console.Clear();
            Console.WriteLine("Percent completed: {0}%.", args.Percent);
        }
        private static void Backup_Completed(object sender, ServerMessageEventArgs args)
        {
            Console.WriteLine("Backup completed.");
            Console.WriteLine(args.Error.Message);
        }
    }
}
