using System;
using Microsoft.Extensions.Configuration;
using Cocona;
using Cocona.Filters;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SQLTemporalCmdLine
{
    class Program
    {
        const string DEFAULT_SETTINGS_JSON_FILENAME = "appSettings.json";
        static void Main(string[] args)
        {
            (string settingsFile, string[] remainingArgs) = ExtractGlobalOptions(args);

            if (!System.IO.File.Exists(settingsFile))
            {
                Console.WriteLine($"ERROR: Cannot build configuration using '{settingsFile}' settings file");
                return;
            }

            CoconaApp.Create()
                .ConfigureAppConfiguration(cfg => {
                    if (settingsFile != DEFAULT_SETTINGS_JSON_FILENAME)
                        cfg.AddJsonFile(settingsFile);
                    cfg.AddCommandLine(remainingArgs);
                    cfg.Build();
                    })
                .Run<Program>(remainingArgs, options => {
                    options.TreatPublicMethodsAsCommands = false;
                    options.EnableConvertCommandNameToLowerCase = false;
                    options.EnableConvertOptionNameToLowerCase = false;
            });
        }
        
        private static (string settingsFile, string[] remainingArgs) ExtractGlobalOptions(string[] args)
        {
            string settingsFile = DEFAULT_SETTINGS_JSON_FILENAME;
            List<string> newArgs = new List<string>();
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {  
                    string arg = args[i].ToLower();   //peel off verbose flag, assign to global state
                    if (arg.Contains("--appSettings") || arg.Contains("-a"))  
                    {
                        var arr = arg.Split('=');  //--appSettings=somefile.json
                        if (arr != null && arr.Length > 1 && !string.IsNullOrEmpty(arr[0]) && !string.IsNullOrEmpty(arr[1]))
                        {
                            settingsFile = arr[1];
                        }
                    }
                    else
                    {
                        newArgs.Add(arg);
                    }
                }

            }
            return (settingsFile, newArgs.ToArray());
        }

        //[IgnoreUnknownOptions]
        [Command(Description = "add a column to a specific table")]
        public void add( [FromService] IConfiguration config,
                         [Argument (Description = "connection string key in configuration")] string ConnectionStringKey,
                         [Argument (Description = "name of table")] string TableName,
                         [Argument (Description = "name of column to add")] string ColumnName,
                         [Argument (Description = "type of column")] string ColumnType,
                         [Option('n', Description = "is column nullable? (default=no)")] bool nullable = false,
                         [Option('d', Description = "default value")] string @default = ""
                         )
        {
            var connectionString = config.GetConnectionString(ConnectionStringKey);
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"Cannot find ConnectionStringKey '{ConnectionStringKey}'");
            }
            else
            {            
                var sql = new SQLServer(config, connectionString);
                sql.AddColumn(TableName, ColumnName, ColumnType, nullable, @default);
            }
        }

        //[IgnoreUnknownOptions]
        [Command(Description = "delete a column from a specific table")]
        public void delete( [FromService] IConfiguration config,
                            [Argument (Description = "connection string key in configuration")] string ConnectionStringKey,
                            [Argument (Description = "name of table")] string TableName,
                            [Argument(Description = "name of column to delete")]string ColumnName)
        {
            var connectionString = config.GetConnectionString(ConnectionStringKey);
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"Cannot find ConnectionStringKey '{ConnectionStringKey}'");
            }
            else
            {            
                var sql = new SQLServer(config, connectionString);
                sql.DeleteColumn(TableName, ColumnName);
            }
            Console.WriteLine($"deleted column name [{ColumnName}] from table [{TableName}]");
        }

    }

}
