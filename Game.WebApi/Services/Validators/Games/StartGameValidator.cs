using FluentValidation;
using GameApp.Domain;

namespace GameApp.WebApi.Services.Validators.Games
{
    public class StartGameValidator : AbstractValidator<int>
    {
        public StartGameValidator(GameContext context)
        {
            RuleFor(roomId => roomId).Must(roomId => context.Rooms.Any(r => r.Id == roomId)).WithMessage(roomId => $"Комната с Id = {roomId} не существует");
            RuleFor(roomId => roomId).Must(roomId => CheckPlayersCount(context, roomId)).WithMessage(roomId => $"В комнате не достаточно игроков для игры");
            RuleFor(roomId => roomId).Must(roomId => CheckForReadyToPlay(context, roomId)).WithMessage(roomId => $"Не все игроки готовы играть");
        }

        private static bool CheckPlayersCount(GameContext context, int roomId)
        {
            var playersInRoom = context.Users.Count(u => u.CurrentRoomId == roomId);
            return playersInRoom == Utils.Constants.maxNumberOfPlayers;
        }

        private static bool CheckForReadyToPlay(GameContext context, int roomId)
        {
            var playersReadyToPlay = context.Users.Count(u => u.IsReadyToPlay == true && u.CurrentRoomId == roomId);
            return playersReadyToPlay == Utils.Constants.maxNumberOfPlayers;
        }
    }
}
