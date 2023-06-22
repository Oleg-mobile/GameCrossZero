using FluentValidation;
using GameApp.Domain;
using GameApp.WebApi.Services.Rooms.Dto;

namespace GameApp.WebApi.Validators.Rooms
{
    public class ExitRoomValidator : AbstractValidator<ExitRoomDto>
    {
        public ExitRoomValidator(GameContext context)
        {
            RuleFor(dto => dto.RoomId).Must(roomId => context.Rooms.Any(r => r.Id == roomId)).WithMessage(dto => $"Комната с Id = {dto.RoomId} не существует");
            RuleFor(dto => dto.UserId).Must(userId => context.Users.Any(u => u.Id == userId)).WithMessage(dto => $"Пользователь с Id = {dto.UserId} не существует");
            RuleFor(dto => dto).Must(dto => context.Users.Any(u => u.CurrentRoomId == dto.RoomId && u.Id == dto.UserId)).WithMessage(dto => $"Пользователь с Id = {dto.UserId} не находится в комнате");
        }
    }
}
