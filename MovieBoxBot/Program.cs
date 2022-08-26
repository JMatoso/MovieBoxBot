
using Microsoft.VisualBasic;
using MovieBoxBot.Models;
using MovieBoxBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("");

using var cancellationToken = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cancellationToken.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

cancellationToken.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;

    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    Console.WriteLine(messageText);

    var chatId = message.Chat.Id;

    var command = messageText.Split(" ")[0];

    var help = Actions.Help(chatId);
    var list = Actions.List(chatId);
    var start = Actions.Start(chatId, $"{message?.From?.FirstName} {message?.From?.LastName}");

    Message sentMessage = command switch
    {
        "/start" => await botClient.SendTextMessageAsync(start.ChatId, start.Text),
        "/help" => await botClient.SendTextMessageAsync(help.ChatId, help.Text, help.ParseMode),
        "/search" => await botClient.SendTextMessageAsync(help.ChatId, help.Text, help.ParseMode),
        "/list" => await ProcessMessages(list, chatId),

        _ => throw new NotImplementedException()
    };
}

async Task<Message> ProcessMessages(PhotoMessageModel model, ChatId chatId)
{
    foreach(var message in model.PhotoMessages)
    {
        await botClient.SendPhotoAsync(message.ChatId, message.Photo, message.Caption, message.ParseMode);
    }

    if(model.Pages > 1)
    {
        Message message = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "",
                parseMode: ParseMode.MarkdownV2,
            disableNotification: true,
            replyToMessageId: null,
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Next Page", "2")));
    }

    return default!;
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}