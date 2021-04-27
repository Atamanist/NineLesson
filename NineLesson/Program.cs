using System;
using System.IO;
using System.Linq;
using System.Text;
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
        static Telegram.Bot.Args.MessageEventArgs E;

        static void Main(string[] args)
        {
            ShowTokken();
            pathline = ShowDaWay();
            Console.WriteLine("Bot start");

            bot.OnMessage += MessageListener;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            bot.StartReceiving();
            Console.ReadKey();
        }


        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(pathline);  // Получаем информацию о текущем каталоге
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            string g = "gimme+";
            string s = "send+";
            string c = "current+";
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

                #region для работы

                if (e.Message.Text.Contains(c))
                {

                    TemperatureKeyboard(e);
                    E = e;
                }

                #endregion

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
                        "\n'gimme+валюта'  курс на текущий день(пример gimme+USD)." +
                        "\n'sendnudes' перечень файлов для скачивания." +
                        "\n'send+файл' файл скачать(пример send+Voina_i_mir.txt)." +
                        "\n'current+кол-во+ток' найдет ближайшее сечение(пример current+2+32)."+
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
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(pfolder);
                    if (r = directoryInfo.Exists)
                    {
                        break;
                    }

                }
                catch
                {
                    Console.WriteLine("Wrong path");
                }


            }
            return (pfolder);

        }



        #region для работы


        static void WireNumberKeyboard(Telegram.Bot.Args.MessageEventArgs e)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
{
                new [] // first row
                {
                    InlineKeyboardButton.WithCallbackData("откр."),
                    InlineKeyboardButton.WithCallbackData("2х1ж"),
                    InlineKeyboardButton.WithCallbackData("3х1ж"),
                },
                new [] // second row
                {
                    InlineKeyboardButton.WithCallbackData("4х1ж"),
                    InlineKeyboardButton.WithCallbackData("1х2ж"),
                    InlineKeyboardButton.WithCallbackData("1х3ж"),
                }
            });
            bot.SendTextMessageAsync(e.Message.Chat.Id, "Сколько проводов", replyMarkup: keyboard);
        }

        static void TemperatureKeyboard(Telegram.Bot.Args.MessageEventArgs e)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
{
                new [] // first row
                {
                    InlineKeyboardButton.WithCallbackData("-5"),
                    InlineKeyboardButton.WithCallbackData("0"),
                    InlineKeyboardButton.WithCallbackData("+5"),
                    InlineKeyboardButton.WithCallbackData("+10"),
                    InlineKeyboardButton.WithCallbackData("+15"),
                    InlineKeyboardButton.WithCallbackData("+20"),

                },
                new [] // second row
                {
                    InlineKeyboardButton.WithCallbackData("+25"),
                    InlineKeyboardButton.WithCallbackData("+30"),
                    InlineKeyboardButton.WithCallbackData("+35"),
                    InlineKeyboardButton.WithCallbackData("+40"),
                    InlineKeyboardButton.WithCallbackData("+45"),
                    InlineKeyboardButton.WithCallbackData("+50"),                }
            });
            bot.SendTextMessageAsync(e.Message.Chat.Id, "Какая температура", replyMarkup: keyboard);
        }

        private static void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {

            TemperatureI(e);


        }
        static void WireNumberI(Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery.Data;
            int k = 0;
            switch (message)
            {
                case "откр.": k = 1; break;
                case "2х1ж": k = 2; break;
                case "3х1ж": k = 3; break;
                case "4х1ж": k = 4; break;
                case "1х2ж": k = 5; break;
                case "1х3ж": k = 6; break;
            }

        }
        static void TemperatureI(Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery.Data;
            float i = 0;
            switch (message)
            {
                case "-5": i = 1.29f;
                    SendCurrent(i, e);
                    break;
                case "0": i = 1.24f;
                    SendCurrent(i, e);
                    break;
                case "+5": i = 1.20f;
                    SendCurrent(i, e);
                    break;
                case "+10": i = 1.15f;
                    SendCurrent(i, e);
                    break;
                case "+15": i = 1.11f;
                    SendCurrent(i, e);
                    break;
                case "+20": i = 1.05f;
                    SendCurrent(i, e);
                    break;
                case "+25": i = 1.00f;
                    SendCurrent(i, e);
                    break;
                case "+30": i = 0.94f;
                    SendCurrent(i, e);
                    break;
                case "+35": i = 0.88f;
                    SendCurrent(i, e);
                    break;
                case "+40": i = 0.81f;
                    SendCurrent(i, e);
                    break;
                case "+45": i = 0.74f;
                    SendCurrent(i, e);
                    break;
                case "+50": i = 0.67f;
                    SendCurrent(i, e);
                    break;

            }
        }

        static int[] Current(Telegram.Bot.Args.MessageEventArgs e)
        {
            int[] n = new int[2];
            try
            {
                int position1 = e.Message.Text.IndexOf("+");
                string f = e.Message.Text.Substring(position1 + 1);
                int position2 = f.IndexOf("+");
                n[0] = Convert.ToInt32(f.Substring(0, position2));
                n[1] = Convert.ToInt32(f.Substring(position2 + 1));
            }
            catch
            {
                try
                {

                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Я думаю что-то написано не правильно");
                }
                catch
                {

                }

            }
            return (n);

        }


        static float CurrentCalc(float t, int[] cn)
        {
            float[,] table = new float[,] {
                { 0.5f,11,0,0,0,0,0 },
                { 0.75f,15,0,0,0,0,0 },
                { 1,17,16,15,14,15,14 },
                { 1.2f,20,18,16,15,16,14.5f},
                { 1.5f,23,19,17,16,18,15},
                { 2,26,24,22,20,23,19 },
                { 2.5f,30,27,25,25,25,21 },
                { 3,34,32,28,26,28,24 },
                { 4,41,38,35,30,32,27 },
                { 5,46,42,39,34,37,31 },
                { 6,50,46,42,40,40,34 },
                { 8,62,54,51,46,48,43 },
                { 10,80,70,60,50,55,50 },
                { 16,100,85,80,75,80,70 },
                { 25,140,115,100,90,100,85 },
                { 35,170,135,125,115,125,100 },
                { 50,215,185,170,150,160,135 },
                { 70,270,225,210,185,195,175 },
                { 95,330,275,255,225,245,215 },
                { 120,385,315,290,260,295,250 },
                { 150,440,360,330,0,0,0},
                { 185,510,0,0,0,0,0},
                { 240,605,0,0,0,0,0},
                { 300,695,0,0,0,0,0},
                { 400,830,0,0,0,0,0}
            };
            float size = 0;
            try
            {
                //foreach (int i in table)
                //{
                //    if (t * table[i, cn[0]] > cn[1])
                //    {
                //        size = table[i, 0];
                //        break;
                //    }

                //}
                for(int i=0; i<=25; i++)
                {
                    if (t * table[i, cn[0]] > cn[1])
                    {
                        size = table[i, 0];
                        break;
                    }

                }

            }
            catch
            {

            }

            return size;
        }

        static void SendCurrent(float i, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, CurrentCalc(i, Current(E)).ToString()+" кв.мм");

        }

        #endregion


    }
}
