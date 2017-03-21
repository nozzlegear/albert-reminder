using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using albert_extensions.Models;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace albert_extensions
{
    class Program
    {
        static string GetAppDirectory()
        {
            return Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        }

        static string GetFullPath(string path)
        {
            return Path.Combine(GetAppDirectory(), path);
        }

        static async Task Log(string message)
        {
            using (var file = File.AppendText(GetFullPath("log")))
            {
                await file.WriteLineAsync($"{DateTime.Now.ToString()} {message}");
            }
        }

        static void Main(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = "albert-extensions",
                Description = "An Albert extension for reminders."
            };
            app.HelpOption("-? | -h | --help");
            app.VersionOption("-v | --version", "1.0.0");

            app.OnExecute(async () =>
            {
                var op = Environment.GetEnvironmentVariable("ALBERT_OP");
                var metaData = new AlbertMetadata()
                {
                    iid = "org.albert.extension.external/v2.0",
                    name = "Reminder Boi",
                    version = "1.0",
                    author = "Joshua Harms",
                    trigger = "remind"
                };

                //await Log($"Handling Albert Op '{op}'.");

                if (op == "METADATA")
                {
                    var output = JsonConvert.SerializeObject(metaData);

                    Console.Write(output);

                    //await Log($"Initialize output: {output}");
                }
                else if (op == "QUERY")
                {
                    var query = Environment.GetEnvironmentVariable("ALBERT_QUERY").Substring(metaData.trigger.Length + 1);
                    var words = query.Split(' ');

                    // TODO: Don't process the reminder until a time is found

                    var reminder = Parser.Process(new Queue<string>(query.Split(' ')));
                    
                    //await Log($"Handling query '{query}'.");
                    //await Log($"Reminder: {reminder.Title} at {reminder.DueDate.ToString("g")}.");

                    var output = JsonConvert.SerializeObject(new
                    {
                        items = new AlbertItem[]
                        {
                            new AlbertItem()
                            {
                                id = "com.nozzlegear.albert-extensions.item-1",
                                description = $"{reminder.Title} at {reminder.DueDate.ToString("g")}",
                                name = "Create Reminder",
                                icon = "unknown",
                                actions = new List<AlbertItemAction>()
                                {
                                    
                                }   
                            }
                        }
                    });

                    Console.Write(output);

                    //await Log($"Query output: {output}.");
                }
                
                return 0;
            });

            try
            {
                app.Execute(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Environment.Exit(1);
            }
        }
    }
}