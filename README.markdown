#Extremely simple mssql backup console app
This is a simple console application using Sql Server Management Objects to perform backups.

##Usage
sqlbackup.exe [server name] [database name] [filepath]

Filepath is optional, if you omit, it makes the backup on the default database backup path. It is important to check if the mssql server service setted account has permission in the path specified.