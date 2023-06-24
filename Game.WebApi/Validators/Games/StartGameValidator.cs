using FluentValidation;
using GameApp.Domain;
using GameApp.WebApi.Services.Games.Dto;

namespace GameApp.WebApi.Validators.Games
{
    public class StartGameValidator : AbstractValidator<CreateGameDto>
    {
        public StartGameValidator(GameContext context)
        {
            RuleFor(dto => dto.RoomId).Must(roomId => context.Rooms.Any(r => r.Id == roomId)).WithMessage(dto => $"Комната с Id = {dto.RoomId} не существует");

            // TODO проверить
            RuleFor(dto => dto.RoomId).Must(roomId => CheckPlayersCount(context, roomId)).WithMessage(dto => $"В комнате не достаточно игроков для игры");
            RuleFor(dto => dto).Must(dto => CheckForReadyToPlay(context)).WithMessage(dto => $"Не все игроки готовы играть");
        }

        private static bool CheckPlayersCount(GameContext context, int roomId)
        {
            var playersInRoom = context.Users.Count(u => u.CurrentRoomId == roomId);
            return playersInRoom < Utils.Constants.maxNumberOfPlayers;
        }

        private static bool CheckForReadyToPlay(GameContext context)
        {
            var playersReadyToPlay = context.Users.Count(u => u.isReadyToPlay == true);
            return playersReadyToPlay < Utils.Constants.maxNumberOfPlayers;
        }
    }
}
