using FluentValidation;
using GameApp.Domain;
using GameApp.WebApi.Services.Rooms.Dto;
using GameApp.WebApi.Utils;

namespace GameApp.WebApi.Services.Validators.Rooms
{
    public class EnterRoomValidator : AbstractValidator<EnterRoomDto>
    {
        public EnterRoomValidator(GameContext context)
        {
            RuleFor(dto => dto.RoomId).Must(roomId => context.Rooms.Any(r => r.Id == roomId)).WithMessage(dto => $"Комната с Id = {dto.RoomId} не существует");
            RuleFor(dto => dto.UserId).Must(userId => context.Users.Any(u => u.Id == userId)).WithMessage(dto => $"Пользователь с Id = {dto.UserId} не существует");
            RuleFor(dto => dto).Must(dto => !context.Users.Any(u => u.CurrentRoomId == dto.RoomId && u.Id == dto.UserId)).WithMessage(dto => $"Пользователь с Id = {dto.UserId} уже находится в комнате");  // TODO login?
            RuleFor(dto => dto.RoomId).Must(roomId => CheckUsersCountInRoom(context, roomId)).WithMessage(dto => "Превышено количество пользователей для комнаты");
            RuleFor(dto => dto).Must(dto => CheckPassword(context, dto)).WithMessage(dto => "Не верный пароль");
        }

        private static bool CheckUsersCountInRoom(GameContext context, int roomId)
        {
            var countPlayersInRoom = context.Users.Count(u => u.CurrentRoomId == roomId);
            return countPlayersInRoom <= Constants.maxNumberOfPlayers;
        }

        private static bool CheckPassword(GameContext context, EnterRoomDto input)
        {
            var room = context.Rooms.Find(input.RoomId);
            return (!string.IsNullOrEmpty(room.Password) && room.Password == input.Password) || string.IsNullOrEmpty(room.Password);
        }
    }
}
