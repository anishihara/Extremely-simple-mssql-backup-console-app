using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Topshelf;
using Topshelf.ServiceConfigurators;

namespace SqlBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog.WriteEntry("SQLBackupService", "Started");
            HostFactory.Run(x =>
            {
                x.Service<BackupService>(ConfigureService);
                x.RunAsLocalSystem();
                x.SetDescription("SQL server backup service.");
                x.SetDisplayName("SQL server backup service");
                x.SetServiceName("SQLBackupService");
                x.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(2); // reset service in 2 minutes if fails
                });
                x.StartAutomatically();
            });
        }

        private static void ConfigureService(ServiceConfigurator<BackupService> sc)
        {
            sc.ConstructUsing(() => new BackupService());
            sc.WhenStarted(s => s.Start());
            sc.WhenStopped(s => s.Stop());
        }
    }
}
