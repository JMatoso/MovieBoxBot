using MovieBoxBot.Models;
using MovieBoxBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using var cancellationToken = new CancellationTokenSource();

Console.WriteLine("Starting bot...");

var botClient = new TelegramBotClient("5636817662:AAHY2-Agal672i1RjCqElHKVN7Xi3OD6mVg");

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

Console.WriteLine($"Listening for @{me.Username}");
Console.ReadLine();

cancellationToken.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;

    if (message.Text is not { } messageText)
        return;

    Console.WriteLine($"{message.Chat.FirstName} {message.Chat.LastName} sent: {messageText} at {DateTime.Now.ToLongTimeString()}");

    var help = Actions.Help();
    var list = Actions.List();
    var start = Actions.Start($"{message?.From?.FirstName} {message?.From?.LastName}");

    var chatId = message!.Chat.Id;

    if (message.EntityValues is not null)
    {
        _ = message.EntityValues.First() switch
        {
            "/start" => await botClient.SendTextMessageAsync(chatId, start.Text, replyToMessageId: message!.MessageId, cancellationToken: cancellationToken),
            "/help" => await botClient.SendTextMessageAsync(chatId, help.Text, help.ParseMode, replyToMessageId: message!.MessageId, cancellationToken: cancellationToken),
            "/list" => await ProcessMessages(list, chatId, cancellationToken),
            "/search" => await SearchMovie(messageText, chatId, message.MessageId, cancellationToken),

            _ => await botClient.SendTextMessageAsync(chatId, "I guess it's not a command I know.", replyToMessageId: message!.MessageId, cancellationToken: cancellationToken)
        };

        return;
    }

    await botClient.SendTextMessageAsync(chatId, "What do you mean? I didn't get it.", replyToMessageId: message!.MessageId, cancellationToken: cancellationToken);
}

async Task<Message> SearchMovie(string messageText, ChatId chatId, int messageId, CancellationToken cancellationToken)
{
    var keyword = messageText.Replace("/search", string.Empty);

    if(string.IsNullOrEmpty(keyword))
    {
        await botClient.SendTextMessageAsync(chatId, "Give me a title.\n\n Try: /search <b>movie name</b>.", parseMode: ParseMode.Html, replyToMessageId: messageId, cancellationToken: cancellationToken);
        return default!;
    }

    var result = Actions.List(1, keyword);

    return await ProcessMessages(result, chatId, cancellationToken);
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
            photo: message.Photo!,
            caption: message.Caption,
            parseMode: message.ParseMode,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
        );
    }
    
    if (model.Pages > 1 && model.MoviesCount > 4)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new KeyboardButton[] { "Help me", "Call me ☎️" },
        })
        {
            ResizeKeyboard = true
        };

        /*var inlineKeyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(text: "Next Page", callbackData: "page=2"));*/

        _ = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "See more",
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: cancellationToken
        );
    }

    return default!;
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}
