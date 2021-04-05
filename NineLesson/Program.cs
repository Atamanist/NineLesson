using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Xml.Linq;

namespace NineLesson
{
    class Program
    {
        static TelegramBotClient bot;

        static void Main(string[] args)
        {

            string token = File.ReadAllText("tekken.txt");

            #region exc

            //// https://hidemyna.me/ru/proxy-list/?maxtime=250#list

            // Содержит параметры HTTP-прокси для System.Net.WebRequest класса.
            //var proxy = new WebProxy()
            //{
            //    Address = new Uri($"http://77.87.240.74:3128"),
            //    UseDefaultCredentials = false,
            //    //Credentials = new NetworkCredential(userName: "login", password: "password")
            //};

            //// Создает экземпляр класса System.Net.Http.HttpClientHandler.
            //var httpClientHandler = new HttpClientHandler() { Proxy = proxy };

            //// Предоставляет базовый класс для отправки HTTP-запросов и получения HTTP-ответов 
            //// от ресурса с заданным URI.
            //HttpClient hc = new HttpClient(httpClientHandler);

            //bot = new TelegramBotClient(token, hc);

            #endregion

            bot = new TelegramBotClient(token);
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");


            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                Console.WriteLine(e.Message.Document.FileId);
                Console.WriteLine(e.Message.Document.FileName);
                Console.WriteLine(e.Message.Document.FileSize);

                DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
            }

            //if (e.Message.Text == null) return;

            //var messageText = e.Message.Text;


            //bot.SendTextMessageAsync(e.Message.Chat.Id,
            //    $"{messageText}"
            //    );

            if (e.Message.Text != null)
            {
                e.Message.Text=e.Message.Text.ToUpper();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                XDocument xml = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp");
                xml.
                if ((xml.Elements("ValCurs")
                                    .Elements("Valute")
                                    .FirstOrDefault(x => x.Element("CharCode")
                                    .Value == $"{e.Message.Text}")
                                    .Elements("Value")
                                    .FirstOrDefault()
                                    .Value) != null)
                {
                    var messageText = (xml.Elements("ValCurs")
                    .Elements("Valute")
                    .FirstOrDefault(x => x.Element("CharCode")
                    .Value == $"{e.Message.Text}")
                    .Elements("Value")
                    .FirstOrDefault()
                    .Value) ?? "nope";


                    bot.SendTextMessageAsync(e.Message.Chat.Id,
                                            $"{messageText}"
                                            );
                }
                else
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id,
                        $"{e.Message.Text}"
                        );
                }


            }

            //return;


        }

        static async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream("_" + path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
        }
    }
}
