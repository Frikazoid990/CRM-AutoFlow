using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CRM_AutoFlow.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task JoinToChat(int chatId)
        {
            // Добавляем подключение в группу чата
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

            // Получаем историю сообщений
            var messages = await _messageService.GetAllMessagesForChat(chatId);
             
            // Отправляем историю только подключившемуся клиенту
            await Clients.Caller.SendAsync( "ReceiveMessageHistory", messages);

            // Уведомляем других участников чата о новом пользователе
        }

        public async Task SendMessage(int chatId, string content, Guid userId)
        {

            // Создаём DTO для нового сообщения
            var messageDto = new CreateMessageDto
            {
                ChatId = chatId,
                Content = content,
                SenderId = userId
            };

            // Сохраняем сообщение в БД
            Guid messageGuid = await _messageService.AddMessage(messageDto);

            // Получаем полные данные сообщения (с информацией об отправителе)
            var fullMessage = (await _messageService.GetAllMessagesForChat(chatId))
                .FirstOrDefault(m => m.Id == messageGuid);

            if (fullMessage != null)
            {
                // Отправляем сообщение всем участникам чата
                await Clients.Group(chatId.ToString())
                    .SendAsync("ReceiveMessage", fullMessage);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Можно добавить логику при отключении пользователя
            await base.OnDisconnectedAsync(exception);
        }


    }
}
