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
using Telegram.Bot.Types.ReplyMarkups;



namespace NineLesson
{
    class Program
    {
        public static string pathline;
        static TelegramBotClient bot;

        static void Main(string[] args)
        {
            ShowTokken();
            pathline = ShowDaWay();
            Console.WriteLine("Bot start");

            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(pathline);  // Получаем информацию о текущем каталоге
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            string g = "gimme+";
            string s = "send+";
            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

            #region для работы
            //            var keyboard = new InlineKeyboardMarkup(new[]
            //            {
            //    new [] // first row
            //    {
            //        InlineKeyboardButton.WithUrl("1.1"),
            //        InlineKeyboardButton.WithCallbackData("1.2"),
            //    },
            //    new [] // second row
            //    {
            //        InlineKeyboardButton.WithCallbackData("2.1"),
            //        InlineKeyboardButton.WithCallbackData("2.2"),
            //    }
            //});
            //bot.SendTextMessageAsync(e.Message.Chat.Id, "text", replyMarkup:keyboard) ;
            #endregion

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
            FileStream fs = new FileStream(@$"{pathline}{path}", FileMode.Create);
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
                FileStream fs = File.OpenRead($@"{pathline}{e.Message.Text}");
                 
                InputOnlineFile fl = new InputOnlineFile(fs, e.Message.Text);
                bot.SendDocumentAsync(e.Message.Chat.Id, fl, e.Message.Text);
            }
            catch
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"Нет такого файла");
            }

        }

        /// <summary>
        /// Проверка пути к токену в формате тхт и возврат бота
        /// </summary>
        /// <returns></returns>
        static TelegramBotClient ShowTokken()
        {
            bool r=true;
            string ptoken;
            while (r)
            {
                Console.WriteLine("Path tokken:");
                ptoken = Console.ReadLine();
                //ptoken = @$"tekken.txt";
                try
                {

                    bot = new TelegramBotClient(File.ReadAllText(ptoken));

                    r = false;
                }
                catch
                {
                    Console.WriteLine("Wrong path or tokken");
                }

            }
            return (bot);

        }

        /// <summary>
        /// Проверка пути к папке для сохранения выгрузки
        /// </summary>
        /// <returns></returns>
        static string ShowDaWay()
        {
            bool r = false;
            string pfolder = "";
            while (r==false)
            {
                Console.WriteLine("Path folder:");
                pfolder = Console.ReadLine();
                //pfolder = @$"V:\BotFiles";
                DirectoryInfo directoryInfo = new DirectoryInfo(pfolder);
                if(r = directoryInfo.Exists)
                {
                    break;
                }
                Console.WriteLine("Wrong path");

            }
            return (pfolder);

        }

    }
}
