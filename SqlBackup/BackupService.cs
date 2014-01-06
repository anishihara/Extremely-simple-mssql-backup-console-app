using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace SqlBackup
{
    public class BackupService
    {

        Backup backup;
        readonly Timer timer;
        public BackupService()
        {
            int intervalInHours = SqlBackup.Properties.Settings.Default.intervalInHours;
            timer = new Timer(intervalInHours * 60 * 60 * 1000) { AutoReset = true };
            timer.Elapsed += (o, e) => Backup();
        }

        public void Backup()
        {
            EventLog.WriteEntry("SQLBackupService", "Starting backup...");
            string serverName = SqlBackup.Properties.Settings.Default.servername;
            string database = SqlBackup.Properties.Settings.Default.database;
            string filePath;

            Server srv = new Server(@serverName);
            if (SqlBackup.Properties.Settings.Default.filepath == "" || SqlBackup.Properties.Settings.Default.filepath == null)
            {
                filePath = srv.BackupDirectory;
            }
            else
            {
                filePath = SqlBackup.Properties.Settings.Default.filepath;
            }

            backup = new Backup();

            // specify what you want to backup
            backup.Action = BackupActionType.Database;

            // specify the name of the database
            backup.Database = database;

            // specify what kind of devides to use, in this example we are using the File Device
            backup.Devices.AddDevice(@filePath, DeviceType.File);
            backup.BackupSetName = SqlBackup.Properties.Settings.Default.backupsetname;
            backup.BackupSetDescription = SqlBackup.Properties.Settings.Default.backupsetdescription;

            // setting the expiration date
            backup.ExpirationDate = DateTime.Today.AddDays(SqlBackup.Properties.Settings.Default.expirationDays);

            // setting incremental backup
            backup.Incremental = SqlBackup.Properties.Settings.Default.incremental;

            // events called to show progress and when the backup finish
            backup.PercentComplete += CompletionStatusInPercent;
            backup.Complete += Backup_Completed;

            // start backup, this method is asynchronous
            backup.SqlBackup(srv);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
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
            EventLog.WriteEntry("SQLBackupService", "Ended backup.");
        }
    }
}
