//"8067802878:AAE8sp0urn8ecnPhVRnvM7cw42bldGEseE0";
//"@qadambayevvvvvvvvv", "@qadambayevvvvv"
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static readonly string token = "8067802878:AAE8sp0urn8ecnPhVRnvM7cw42bldGEseE0";
    private static readonly TelegramBotClient bot = new TelegramBotClient(token);

    // Majburiy ulanish kerak bo‘lgan kanallar
    private static readonly string[] requiredChannels = { "@qadambayevvvvvvvvv", "@qadambayevvvvv" };

    static async Task Main()
    {
        Console.WriteLine("✅ Bot ishga tushdi...");

        bot.StartReceiving(UpdateHandler, ErrorHandler);

        Console.ReadLine();
    }

    static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Foydalanuvchi xabar yuborsa
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var chatId = update.Message.Chat.Id;
            var userId = update.Message.From!.Id;

            if (update.Message.Text == "/start")
            {
                bool allJoined = await CheckUserSubscriptions(botClient, userId, cancellationToken);

                if (allJoined)
                {
                    await botClient.SendTextMessageAsync(
                        chatId,
                        "✅ Siz allaqachon barcha kanallarga obuna bo‘lgansiz. Botdan foydalanishingiz mumkin!",
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await AskToJoinChannels(botClient, chatId, cancellationToken);
                }
            }
        }

        // Callback tugmalarni bosganda
        if (update.Type == UpdateType.CallbackQuery)
        {
            var callback = update.CallbackQuery!;
            var chatId = callback.Message!.Chat.Id;
            var userId = callback.From.Id;

            if (callback.Data == "check_subs")
            {
                bool allJoined = await CheckUserSubscriptions(botClient, userId, cancellationToken);

                if (allJoined)
                {
                    await botClient.EditMessageText(
                        chatId: chatId,
                        messageId: callback.Message.MessageId,
                        text: "✅ Siz barcha kanallarga obuna bo‘lgansiz. Botdan foydalanishingiz mumkin!"
                    );
                }
                else
                {
                    await botClient.AnswerCallbackQuery(
                        callback.Id,
                        "❌ Siz hali barcha kanallarga obuna bo‘lmadingiz!",
                        showAlert: true,
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
    }

    // Foydalanuvchidan kanallarga obuna bo‘lishni so‘rash
    static async Task AskToJoinChannels(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        // Kanal tugmalari
        var channelButtons = requiredChannels
            .Select(c => new[] { InlineKeyboardButton.WithUrl($"👉 {c}", $"https://t.me/{c.TrimStart('@')}") })
            .ToList();

        // "Obuna bo‘ldim ✅" tugmasi
        channelButtons.Add(new[] { InlineKeyboardButton.WithCallbackData("📌 Obuna bo‘ldim ✅", "check_subs") });

        var keyboard = new InlineKeyboardMarkup(channelButtons);

        await botClient.SendTextMessageAsync(
            chatId,
            "❌ Botdan foydalanish uchun quyidagi kanallarga obuna bo‘ling va so‘ng '📌 Obuna bo‘ldim ✅' tugmasini bosing:",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }

    // Foydalanuvchi barcha kanallarga obuna bo‘lganini tekshirish
    static async Task<bool> CheckUserSubscriptions(ITelegramBotClient botClient, long userId, CancellationToken cancellationToken)
    {
        foreach (var channel in requiredChannels)
        {
            try
            {
                var member = await botClient.GetChatMemberAsync(channel, userId, cancellationToken);
                if (member.Status == ChatMemberStatus.Left || member.Status == ChatMemberStatus.Kicked)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        return true;
    }

    static Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Xato: {exception.Message}");
        return Task.CompletedTask;
    }
}
