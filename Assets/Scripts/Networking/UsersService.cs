using Mirror;
using Gameplay.Models;

namespace Networking
{
    public class UsersService : NetworkBehaviour
    {
        private SyncDictionary<Player, User> _usersByPlayers = new SyncDictionary<Player, User>();


        public void Register(Player player, User user)
        {
            _usersByPlayers[player] = user;
        }
    }
}
