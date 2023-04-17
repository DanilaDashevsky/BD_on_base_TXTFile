
using AngleSharp;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using AngleSharp.Dom;
using Telegram.Bot.Types;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using AngleSharp.Io;

internal class Program
{
    public static SqliteConnection connection;
    public static SqliteCommand command;
    public static string muvie;
    public static string token = "6264990741:AAGJW2woUsJlh4GZdQnWGfBbCboVocz1JpE";
    public static TelegramBotClient client = new TelegramBotClient(token);
    private static async Task Main(string[] args)
    {

        connection = new SqliteConnection("Data Source=C:\\Users\\DanilaLearn\\Desktop\\ShopBot\\ShopBot\\shopBot.db");
        connection.Open();
        client.StartReceiving();
        client.OnMessage += OnMessageHandler;

        client.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
        {
            var message = ev.CallbackQuery.Message;
            priceMy += price;
            if (ev.CallbackQuery.Data == "Bay")
            {
                if (basket == null)
                { basket += custom; await client.SendTextMessageAsync(msg.Chat.Id, "Ваш товар успешно добавлен в корзину!"); }
                else
                    if (basket.Contains(custom) == false)
                {
                    basket += ',' + custom;
                    await client.SendTextMessageAsync(msg.Chat.Id, "Ваш товар успешно добавлен в корзину!");
                }
                else
                    await client.SendTextMessageAsync(msg.Chat.Id, "Этот товар в карзине уже есть");
            }
            if (ev.CallbackQuery.Data == "Order")
            {
                command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = $"INSERT INTO Orders(Id,Number ,Custom, Price,Data,Place) VALUES ('{msg.Chat.Id}',\'{number1}\',\'{basket}\',{price},\'{DateTime.Now}\',\'{place}\')";
                command.ExecuteNonQuery();
                await client.SendTextMessageAsync(msg.Chat.Id, "Отлично, как только заказ будет доставлен, Вам будет отправлено СМС на этот номер😎", replyMarkup: await GetMenu());
                basket = null;
            }
        };

        Console.ReadKey();
    }

    public static Message msg;
    public static string custom;
    public static double price;
    public static string place;
    public static double priceMy;
    public static string basket;
    public static string number1;

    private static async void OnMessageHandler(object? sender, MessageEventArgs e)
    {
        try
        {

            msg = e.Message;
            Username1 = msg.Chat.Username;
            Firstname1 = msg.Chat.FirstName;
            Lastname1 = msg.Chat.LastName;
            if (msg.Text != null)
            {
                if (counter_question > 0 && msg.Text != "")
                {
                    counter_question = 0;
                    if (msg.Text != "😎Товары😎" && msg.Text != "😍Связаться с разработчиком😍" && msg.Text != "😎Эл.чайники😎")
                    {
                        await client.SendTextMessageAsync("1918705001", $"ЧатID {msg.Chat.Id} Cообщение:{msg.Text} Username: {msg.Chat.Username} Имя: {msg.Chat.FirstName} Фамилия: {msg.Chat.LastName} Время:{DateTime.Now.ToString()}");
                        await client.SendTextMessageAsync(msg.Chat.Id, "Хорошо, ожидайте. Администратор ответит вам в личном сообщении в ближайшее время!");
                    }
                    else
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, "Простите, нельзя использовать команды из меню, чтобы написать разработчику");
                    }
                }
                else
                if ((table == "Teapots1" || table == "PowerSockets" || table == "Irons1") && msg.Text != "")
                {
                    command = new SqliteCommand($"SELECT * FROM {table} Where Name = \'{msg.Text}\'", connection);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            reader.Read();
                            await client.SendPhotoAsync(msg.Chat.Id, reader.GetValue(2).ToString());
                            await client.SendTextMessageAsync(msg.Chat.Id, reader.GetValue(3).ToString());
                            await client.SendTextMessageAsync(msg.Chat.Id, reader.GetValue(4).ToString());
                            await client.SendTextMessageAsync(msg.Chat.Id, "Вы хотите купить?", replyMarkup: await GetSite());
                            custom = reader.GetValue(1).ToString();
                            price = double.Parse(reader.GetValue(4).ToString().Replace(" ₽",""));
                        }
                    }
                }
              
                switch (msg.Text)
                {

                    case "/start": { await client.SendTextMessageAsync(msg.Chat.Id, $"О,{msg.Chat.FirstName} привет😍!", replyMarkup: await GetMenu()); await client.SendTextMessageAsync(msg.Chat.Id, $"Рад видеть тебя в своём чат боте!"); break; }
                    case "😎Товары😎": { await client.SendTextMessageAsync(msg.Chat.Id, ":)", replyMarkup: await GetMenuDeveloper()); break; }
                    case "⬅Назад⬅": { list_ref.Clear(); table = ""; counter_question = 0; await client.SendTextMessageAsync(msg.Chat.Id, ":)", replyMarkup: await GetMenu()); break; }
                    case "🕹Играть🕹": { await client.SendTextMessageAsync(msg.Chat.Id, "Отлично, давай поиграем.😏 Напиши что-нибудь, а я попробую найти картинку по твоему запросу", replyMarkup: await GetMenuPalyYoutube()); table = "play"; break; }
                    case "😎Эл.чайники😎": { table = "Teapots1"; await client.SendTextMessageAsync(msg.Chat.Id, "Отличный выбор", replyMarkup: await GetMenuSite(table)); break; }
                    case "😍Розетки😍": { table = "PowerSockets"; await client.SendTextMessageAsync(msg.Chat.Id, "Отличный выбор", replyMarkup: await GetMenuSite(table)); break; }
                    case "😍Утюги😍": { table = "Irons1"; await client.SendTextMessageAsync(msg.Chat.Id, "Отличный выбор", replyMarkup: await GetMenuSite(table)); break; }
                    case "🗑Корзина🗑": { await client.SendTextMessageAsync(msg.Chat.Id, $"Корзина: {basket}" + " " + priceMy.ToString() + '₽', replyMarkup: await GetMenu()); break; }
                    case "🧹Отчистить корзину🧹": { basket = null; priceMy = 0; await client.SendTextMessageAsync(msg.Chat.Id, $"Корзина успешно отчищена", replyMarkup: await GetMenu()); break; }
                    case "💸Сделать заказ💸": { if (basket != null) { table = "Order"; await client.SendTextMessageAsync(msg.Chat.Id, "Отправте город, в котором вы хотите получить товар", replyMarkup: await GetMenuPalyYoutube()); } else await client.SendTextMessageAsync(msg.Chat.Id, "Извините, ваша карзина пуста🤷‍♂️");  break; } 
                    case "😍Связаться с разработчиком😍": { await client.SendTextMessageAsync(msg.Chat.Id, "Отправте ваш вопрос:", replyMarkup: await GetMenu()); ++counter_question; break; }
                    case "🙉Мои заказы🙉":
                        {
                            command = new SqliteCommand($"SELECT * FROM Orders Where Id = \'{msg.Chat.Id}\'", connection);
                           
                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows) // если есть данные
                                {
                                    while (reader.Read())   // построчно считываем данные
                                    {
                                        await client.SendTextMessageAsync(msg.Chat.Id, $"Ваш заказ {reader.GetValue(2)} на сумму {reader.GetValue(3)}₽. Дата заказа: {reader.GetValue(4)}; Адрес получения: {reader.GetValue(5)}");
                                    }
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(msg.Chat.Id, "У вас пока нет заказов.");
                                }

                            }
                            break; }
                }
                if(table == "InputNumber" && Regex.IsMatch(msg.Text, "^((8|\\+7)[\\- ]?)?(\\(?\\d{3}\\)?[\\- ]?)?[\\d\\- ]{7,10}$"))
                {
                    await client.SendTextMessageAsync(msg.Chat.Id, "Оформляем заказ?", replyMarkup: await Order());
                    number1 = msg.Text;
                } else
                if (table == "Adres" && list_ref.Any(u => u.Contains(msg.Text)))
                {
                    place = msg.Text;
                    await client.SendTextMessageAsync(msg.Chat.Id, $"Отлично! Ваш заказ будет ожидать вас по адресу: {place}");
                    await client.SendTextMessageAsync(msg.Chat.Id, "Введите ваш номер телефона: ");
                    table = "InputNumber";
                }
                else
                if (table=="Order" && msg.Text != null &&  msg.Text != "💸Сделать заказ💸" )
                {
                    var config = Configuration.Default.WithDefaultLoader();
                    string request = msg.Text;
                    
                    var address = "https://e-ecolog.ru/entity/2540167061/filial?ysclid=lfttongoxh714544329";
                    var document = await BrowsingContext.New(config).OpenAsync(address);
                    foreach (var element in document.QuerySelectorAll("li"))
                    {
                        if(element.Text().Contains(msg.Text + ",") || element.Text().Contains(msg.Text.ToUpper()) || element.Text().Contains(msg.Text.ToLower()))
                            list_ref.Add(element.Text());
                    }
                    if (list_ref.Count >= 1)
                    {
                        for (int i = 0; i < list_ref.Count; i++)
                        {
                            list_ref[i] = list_ref[i].Remove(0, list_ref[i].IndexOf(":"));
                            list_ref[i] = list_ref[i].Replace(":", "");
                        }
                        await client.SendTextMessageAsync(msg.Chat.Id, $"Выберите номер адреса из следующего списка:", replyMarkup: await GetAdres());
                        table = "Adres";
                    }
                    else
                        await client.SendTextMessageAsync(msg.Chat.Id, "Извините, мы не имеем филлиалов в этом городе");
                }
                else
                if (table == "play" && msg.Text != null && msg.Text != "🕹Играть🕹" && msg.Text != "⬅Назад⬅")
                {
                    var config = Configuration.Default.WithDefaultLoader();
                    string request = msg.Text;
                    Random rnd = new Random();
                    var address = "https://yandex.ru/images/search?text=" + request.Replace(" ", "%20");
                    var document = await BrowsingContext.New(config).OpenAsync(address);
                    foreach (var element in document.QuerySelectorAll("img"))
                    {
                        if (element.ClassName == "serp-item__thumb justifier__thumb")
                        {
                            list_ref.Add("https:" + element.GetAttribute("src").ToString());
                        }
                    }

                    if (list_ref.Count >= 1)
                        await client.SendPhotoAsync(msg.Chat.Id, list_ref[rnd.Next(0, list_ref.Count - 1)]);
                    else
                        await client.SendTextMessageAsync(msg.Chat.Id, "Извините, по вашему запросу ничего не найдено");
                
            }
        }
        }
        catch (SystemException ex)
        {
            await client.SendTextMessageAsync("1918705001", $"{Username1} {Firstname1} {Lastname1} Ошибка {ex.Message}");
        }
    }
    private static async Task<IReplyMarkup> GetMenu()
    {
        return new ReplyKeyboardMarkup
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton> { new KeyboardButton { Text = "😎Товары😎" }, new KeyboardButton { Text = "😍Связаться с разработчиком😍" }, new KeyboardButton { Text = "🗑Корзина🗑" } },
                new List<KeyboardButton> { new KeyboardButton { Text = "🕹Играть🕹" }, new KeyboardButton { Text = "💸Сделать заказ💸" } , new KeyboardButton { Text = "🧹Отчистить корзину🧹" } },
                new List<KeyboardButton> { new KeyboardButton { Text = "🙉Мои заказы🙉" } }
            }
        };
    }
    private static async Task<IReplyMarkup> GetMenuDeveloper()
    {
        return new ReplyKeyboardMarkup
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton> { new KeyboardButton { Text = "😎Эл.чайники😎" }, new KeyboardButton { Text = "😍Утюги😍" }, new KeyboardButton { Text = "😍Розетки😍" }  },
                new List<KeyboardButton> { new KeyboardButton { Text = "⬅Назад⬅" } }
            }
        };
    }
    private static async Task<IReplyMarkup> GetMenuPalyYoutube()
    {
        return new ReplyKeyboardMarkup
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton> { new KeyboardButton { Text = "⬅Назад⬅" } }
            }
        };
    }
    private static async Task<IReplyMarkup> GetMenuSite(string table)
    {
        ReplyKeyboardMarkup Keyboard1 = new ReplyKeyboardMarkup();
        List<List<KeyboardButton>> list = new List<List<KeyboardButton>>();
        command = new SqliteCommand($"SELECT * FROM '{table}'", connection);
        list.Add(new List<KeyboardButton> { new KeyboardButton { Text = "⬅Назад⬅" } });
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows) // если есть данные
            {
                while (reader.Read())   // построчно считываем данные
                {
                    list.Add(new List<KeyboardButton> { new KeyboardButton { Text = reader.GetValue(1).ToString() } });
                }
            }

        }
        Keyboard1.Keyboard = list;
        return Keyboard1;
    }
    private static async Task<IReplyMarkup> GetAdres()
    {
        ReplyKeyboardMarkup Keyboard1 = new ReplyKeyboardMarkup();
        List<List<KeyboardButton>> list = new List<List<KeyboardButton>>();
        command = new SqliteCommand($"SELECT * FROM '{table}'", connection);
        list.Add(new List<KeyboardButton> { new KeyboardButton { Text = "⬅Назад⬅" } });

        for (int i = 0; i < list_ref.Count(); i++)  // построчно считываем данные
        {
            list_ref[i]= list_ref[i].Remove(list_ref[i].IndexOf("ОКПО"), 167);
            list.Add(new List<KeyboardButton> { new KeyboardButton { Text = list_ref[i] } }); 
        }
        Keyboard1.Keyboard = list;
        return Keyboard1;
    }

    public static async Task WriteUser(string? Username1, string? Firstname1, string? Lastname1)
    {
        int u = 0;
        command = new SqliteCommand($"SELECT * FROM Users Where Username = \'{Username1}\'", connection);
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows == false) // если есть данные
            {
                u++;
            }
        }
        if (u > 0)
        {
            if (Lastname1 == null)
                Lastname1 = "none";
            if (Username1 == "")
                Username1 = "none";
            command.CommandText = $"INSERT INTO Users (Username,Firstname,Lastname) values (\'{Username1}\',\'{Firstname1}\',\'{Lastname1}\')";
            await client.SendTextMessageAsync("1918705001", $" Вот этот человек начал пользоваться вашим ботом UserName:{Username1} Имя: {Firstname1} Фамилия: {Lastname1}");
            command.ExecuteNonQuery();
        }
    }
    public static async Task<InlineKeyboardMarkup> GetSite()
    {
        InlineKeyboardButton urlButton = new InlineKeyboardButton();

        urlButton.Text = "Купить";
        urlButton.CallbackData = "Bay";

        InlineKeyboardButton[] buttons = new InlineKeyboardButton[] { urlButton };

        InlineKeyboardMarkup inline = new InlineKeyboardMarkup(buttons);

        return inline;
    }
    public static async Task<InlineKeyboardMarkup> Order()
    {
        InlineKeyboardButton urlButton = new InlineKeyboardButton();

        urlButton.Text = "Купить";
        urlButton.CallbackData = "Order";

        InlineKeyboardButton[] buttons = new InlineKeyboardButton[] { urlButton };

        InlineKeyboardMarkup inline = new InlineKeyboardMarkup(buttons);

        return inline;
    }

    public static int counter_question;
    public static List<string> list_ref = new List<string>();
    public static int counter_play;
    public static string Username1;
    public static string Firstname1;
    public static string Lastname1;
    public static string table;
}
