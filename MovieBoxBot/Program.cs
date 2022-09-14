using MovieBoxBot.Data;
using MovieBoxBot.Utils;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using var cancellationToken = new CancellationTokenSource();

Console.WriteLine("Starting bot...");

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

Console.WriteLine($"Listening for @{me.Username}");
Console.ReadLine();

cancellationToken.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;

    if (message.Text is not { } messageText)
        return;

    var chatId = message!.Chat.Id;

    if (message.EntityValues is not null)
    {
        if (!message.EntityValues.First().Equals("/next"))
        {
            UserActivity.ActualPage = 1;
            UserActivity.HasNext = false;
            UserActivity.SearchKeyword = "";
        }

        switch (message.EntityValues.First())
        {
            case "/start":
                var start = Actions.Start($"{message?.From?.FirstName} {message?.From?.LastName}");
                await botClient.SendTextMessageAsync(chatId, start.Text, replyToMessageId: message!.MessageId, cancellationToken: cancellationToken);
                break;

            case "/help":
                var help = Actions.Help();
                await botClient.SendTextMessageAsync(chatId, help.Text, help.ParseMode, replyToMessageId: message!.MessageId, cancellationToken: cancellationToken);
                break;

            case "/list":
                var movieList = Actions.List(UserActivity.ActualPage);
                await BotDataManagement.ProcessMessages(botClient, movieList, chatId, cancellationToken);
                break;

            case "/search":
                await BotDataManagement.SearchMovie(botClient, messageText, chatId, message.MessageId, cancellationToken);
                break;

            case "/next": 
                await BotDataManagement.GetMore(botClient, chatId, message.MessageId, cancellationToken);
                break;

            default:
                await botClient.SendTextMessageAsync(chatId, "I guess it's not a command I know.", replyToMessageId: message!.MessageId, cancellationToken: cancellationToken);
                break;
        }

        return;
    }

    await botClient.SendTextMessageAsync(chatId, "What do you mean? I didn't get it.", replyToMessageId: message!.MessageId, cancellationToken: cancellationToken);
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
