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

    var messageContent = messageText.Split(" ");
    var command = messageContent[0];

    if (messageContent.Length > 1 && command.Equals("/search"))
    {
        var content = messageContent.ToList();

        content.RemoveAt(0);

        var queryTerm = content.Count > 1 ? string.Join(" ", content) : content[0];

        var searches = Actions.List(1, queryTerm);

        _ = await ProcessMessages(searches, chatId, cancellationToken);

        return;
    }

    var help = Actions.Help();
    var list = Actions.List();
    var start = Actions.Start($"{message?.From?.FirstName} {message?.From?.LastName}");

    _ = command switch
    {
        "/start" => await botClient.SendTextMessageAsync(chatId, start.Text, cancellationToken: cancellationToken),
        "/help" => await botClient.SendTextMessageAsync(chatId, help.Text, help.ParseMode, cancellationToken: cancellationToken),
        "/list" => await ProcessMessages(list, chatId, cancellationToken),

        _ => await botClient.SendTextMessageAsync(chatId, "What do you mean? I didn't get it.", cancellationToken: cancellationToken)
    };
}

async Task<Message> ProcessMessages(PhotoMessageModel model, ChatId chatId, CancellationToken cancellationToken)
{
    _ = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: model.Message,
            cancellationToken: cancellationToken
    );

    foreach (var message in model.PhotoMessages)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl(text: "See on MovieBox", url: $"https://moviebox.site/movie?mid={message.MovieId}"));

        _ = await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: message.Photo,
            caption: message.Caption,
            parseMode: ParseMode.Html,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
        );
    }

    if (model.Pages > 1 && model.MoviesCount > 4)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(text: "Next Page", callbackData: "page=2"));

        _ = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "See more",
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
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
