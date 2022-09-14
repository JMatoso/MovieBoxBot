using MovieBoxBot.Models;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using MovieBoxBot.Data;

namespace MovieBoxBot.Utils;

internal static class BotDataManagement
{
    public static async Task<Message> SearchMovie(ITelegramBotClient botClient, string messageText, ChatId chatId, int messageId, CancellationToken cancellationToken)
    {
        var keyword = messageText.Replace("/search", string.Empty);

        if (string.IsNullOrEmpty(keyword))
        {
            await botClient.SendTextMessageAsync(chatId, "Give me a title.\n\n Try: /search <b>movie name</b>.", parseMode: ParseMode.Html, replyToMessageId: messageId, cancellationToken: cancellationToken);
            return default!;
        }

        var result = Actions.List(1, keyword);

        return await ProcessMessages(botClient, result, chatId, cancellationToken);
    }

    public static async Task<Message> ProcessMessages(ITelegramBotClient botClient, PhotoMessageModel model, ChatId chatId, CancellationToken cancellationToken)
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

        if (model.Pages > 1 && model.MoviesCount > 4 && model.Pages > UserActivity.ActualPage)
        {
            UserActivity.HasNext = true;

            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "See more in the /next page.",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken
            );
        }

        UserActivity.HasNext = false;

        return await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "That's all.",
            cancellationToken: cancellationToken
        );
    }

    public static async Task<Message> GetMore(ITelegramBotClient botClient, ChatId chatId, int messageId, CancellationToken cancellationToken)
    {
        if(UserActivity.HasNext)
        {
            UserActivity.ActualPage++;

            var movieList = Actions.List(UserActivity.ActualPage, UserActivity.SearchKeyword);
            return await ProcessMessages(botClient, movieList, chatId, cancellationToken);
        }

        return await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"There aren't more results for <b>{UserActivity.SearchKeyword}</b>.",
            parseMode: ParseMode.Html,
            replyToMessageId: messageId,
            cancellationToken: cancellationToken
        );
    }
}
