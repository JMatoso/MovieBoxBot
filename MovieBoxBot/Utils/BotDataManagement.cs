using MovieBoxBot.Data;
using MovieBoxBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MovieBoxBot.Utils;

internal static class BotDataManagement
{
    public static async Task SearchMovie(ITelegramBotClient botClient, string messageText, ChatId chatId, int messageId, CancellationToken cancellationToken)
    {
        var keyword = messageText.Replace("/search", string.Empty);

        if (string.IsNullOrEmpty(keyword))
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Give me a title.\n\n Try: /search <b>movie name</b>.", 
                parseMode: ParseMode.Html, 
                replyToMessageId: messageId, 
                cancellationToken: cancellationToken);

            return;
        }

        var result = Actions.List(1, keyword);
        await ProcessMessages(botClient, result, chatId, cancellationToken);
    }

    public static async Task ProcessMessages(ITelegramBotClient botClient, PhotoMessageModel model, ChatId chatId, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: model.Message,
                cancellationToken: cancellationToken
        );

        foreach (var message in model.PhotoMessages)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(text: "See on MovieBox", url: $"https://moviebox.site/movie?mid={message.MovieId}"));

            await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: message.Photo!,
                caption: message.Caption,
                parseMode: message.ParseMode,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }

        if (model.Pages > 1 && model.MoviesCount > 4 && model.Pages > UserActivity.ActualPage)
        {
            UserActivity.HasNext = true;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "See more in the /next page.",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken
            );

            return;
        }

        UserActivity.HasNext = false;

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "That's all.",
            cancellationToken: cancellationToken
        );
    }

    public static async Task GetMore(ITelegramBotClient botClient, ChatId chatId, int messageId, CancellationToken cancellationToken)
    {
        if(UserActivity.HasNext)
        {
            UserActivity.ActualPage++;

            var movieList = Actions.List(UserActivity.ActualPage, UserActivity.SearchKeyword);
            await ProcessMessages(botClient, movieList, chatId, cancellationToken);
            return;
        }

        var message = string.IsNullOrEmpty(UserActivity.SearchKeyword) ?
            "You didn't search anything." :
            $"There aren't more results for <b>{UserActivity.SearchKeyword}</b>.";

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: message,
            parseMode: ParseMode.Html,
            replyToMessageId: messageId,
            cancellationToken: cancellationToken
        );
    }
}
