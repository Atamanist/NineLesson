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
            DirectoryInfo directoryInfo = new DirectoryInfo(@"V:\BotFiles");  // Получаем информацию о текущем каталоге
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            string g = "gimme+";
            string s = "send+";
            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                Console.WriteLine(e.Message.Document.FileId);
                Console.WriteLine(e.Message.Document.FileName);
                Console.WriteLine(e.Message.Document.FileSize);

                DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
            }
            
            if(e.Message.Text!=null)
            {
                if (e.Message.Text.Contains(g))
                {
                    GimmeMoney(e);
                }

                if (e.Message.Text.Contains(s))
                {
                    Nudes(e);
                }

                if (e.Message.Text == "start")
                {
                    StartMessage(e);
                }

                if (e.Message.Text == "sendnudes")
                {
                    SendNudes(e, directoryInfo);
                }

            }


        }

        /// <summary>
        /// Сохраняем посланый файл в общую папку
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        static async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(@$"V:\BotFiles\{path}", FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
            
        }

        /// <summary>
        /// Выдаем тип команд
        /// </summary>
        /// <param name="e"></param>
        static void StartMessage(Telegram.Bot.Args.MessageEventArgs e)
        {
            bot.SendTextMessageAsync(e.Message.Chat.Id,
                        "\n'gimme+USD'  курс на текущий день." +
                        "\n'sendnudes' перечень файлов для скачивания." +
                        "\n'send+filename' файл скачать." +
                        "\nХочешь посылай файлы сохраним."
                        );
        }

        /// <summary>
        /// Выдаем курс валют 
        /// </summary>
        /// <param name="e"></param>
        static void GimmeMoney(Telegram.Bot.Args.MessageEventArgs e)
        {
            int position = e.Message.Text.IndexOf("+");
            e.Message.Text = e.Message.Text.Substring(position+1);
            e.Message.Text = e.Message.Text.ToUpper();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                XDocument xml = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp");

            try
            {           

                var messageText = xml.Elements("ValCurs").
                    Elements("Valute").
                    FirstOrDefault(x => x.Element("CharCode").
                    Value == $"{e.Message.Text}").
                    Elements("Value").
                    FirstOrDefault().Value;
                bot.SendTextMessageAsync(e.Message.Chat.Id,$"{messageText}");
            }
            catch 
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"Нет такой валюты");
            }


        }

        /// <summary>
        /// Посылаем список файлов из папки
        /// </summary>
        /// <param name="e"></param>
        static void SendNudes(Telegram.Bot.Args.MessageEventArgs e, DirectoryInfo directoryInfo)
        {

            foreach (var item in directoryInfo.GetFiles())          // Перебираем все файлы текущего каталога
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"{item.Name}"); // Выводим информацию о них
            }
        }

        /// <summary>
        /// Посылаем файл из папки
        /// </summary>
        /// <param name="e"></param>
        static void Nudes(Telegram.Bot.Args.MessageEventArgs e)
        {
            int position = e.Message.Text.IndexOf("+");
            e.Message.Text = e.Message.Text.Substring(position+1);
            try
            {
                FileStream fs = File.OpenRead($@"V:\BotFiles\{e.Message.Text}");
                 
                InputOnlineFile fl = new InputOnlineFile(fs, e.Message.Text);
                bot.SendDocumentAsync(e.Message.Chat.Id, fl, e.Message.Text);
            }
            catch
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"Нет такого файла");
            }

        }

    }
}
