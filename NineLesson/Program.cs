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
using Telegram.Bot.Types.InputFiles;

namespace NineLesson
{
    class Program
    {
        static TelegramBotClient bot;

        static void Main(string[] args)
        {

            string token = File.ReadAllText("tekken.txt");
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


            if (e.Message.Text == "123")
            {
                FileStream fs = File.OpenRead($"{e.Message.Text}.txt");


                InputOnlineFile fl = new InputOnlineFile(fs,e.Message.Text);

                bot.SendDocumentAsync(e.Message.Chat.Id, fl, e.Message.Text);

            }

            if (e.Message.Text == "sendnudes")
            {
                string trim = "";

                DirectoryInfo directoryInfo = new DirectoryInfo("");  // Получаем информацию о текущем каталоге

                foreach (var item in directoryInfo.GetFiles())          // Перебираем все файлы текущего каталога
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{trim}{item.Name}"); // Выводим информацию о них
                }

            }



            if (e.Message.Text != null)
            {
                e.Message.Text=e.Message.Text.ToUpper();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                XDocument xml = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp");
                if (e.Message.Text=="USD")
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
