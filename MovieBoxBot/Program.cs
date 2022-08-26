using MovieBoxBot.Models;
using MovieBoxBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using var cancellationToken = new CancellationTokenSource();

var botClient = new TelegramBotClient("");

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
    if (update.Message is not { } message)
        return;

    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    var command = messageText.Split(" ")[0];

    var help = Actions.Help(chatId);
    var list = Actions.List(chatId);
    var start = Actions.Start(chatId, $"{message?.From?.FirstName} {message?.From?.LastName}");

    _ = command switch
    {
        "/start" => await botClient.SendTextMessageAsync(start.ChatId, start.Text, cancellationToken: cancellationToken),
        "/help" => await botClient.SendTextMessageAsync(help.ChatId, help.Text, help.ParseMode, cancellationToken: cancellationToken),
        "/search" => await botClient.SendTextMessageAsync(help.ChatId, help.Text, help.ParseMode, cancellationToken: cancellationToken),
        "/list" => await ProcessMessages(list, chatId, cancellationToken),

        _ => default!
    };
}

async Task<Message> ProcessMessages(PhotoMessageModel model, ChatId chatId, CancellationToken cancellationToken)
{
    foreach (var message in model.PhotoMessages)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl(text: "See on MovieBox", url: $"https://moviebox.site/movie?mid={message.MovieId}"));

        await botClient.SendPhotoAsync(
            chatId: message.ChatId,
            photo: message.Photo,
            caption: message.Caption,
            parseMode: ParseMode.Html,
            captionEntities: null,
            disableNotification: false,
            protectContent: null,
            replyToMessageId: null,
            allowSendingWithoutReply: null,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
        );
    }

    if (model.Pages > 1)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(text: "Next Page", callbackData: "page=2"));

        Message message = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "See more",
                parseMode: ParseMode.MarkdownV2,
            disableNotification: true,
            replyToMessageId: null,
            replyMarkup: inlineKeyboard
        );
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
