using Gameplay.Models;
using Mirror;
using UnityEngine;
using Zenject;

namespace Gameplay.Ships
{
    public class SpaceShipsService : NetworkBehaviour
    {
        [SerializeField] private SpaceShipView _viewPrefab;


        private SyncDictionary<User, SpaceShip> _usersShips = new SyncDictionary<User, SpaceShip>();
        private SyncDictionary<SpaceShip, SpaceShipView> _shipsViews = new SyncDictionary<SpaceShip, SpaceShipView>();

        private World _world;


        [Inject]
        public void Construct(World world)
        {
            _world = world;
        }


        public void Register(User user, SpaceShip ship)
        {
            _usersShips[user] = ship;
        }

        public SpaceShipView CreateView(SpaceShip ship)
        {
            var view = Instantiate(_viewPrefab, _world.transform);
            _shipsViews[ship] = view;
            view.Init(ship);
            return view;
        }

        public SpaceShipView GetView(SpaceShip ship)
        {
            return _shipsViews[ship];
        }
    }
}