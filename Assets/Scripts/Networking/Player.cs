using Gameplay;
using Mirror;
using UnityEngine;
using Zenject;
using Gameplay.Ships;
using Gameplay.Models;
using Gameplay.Planets;
using PlanetGenerator;
using Camera;

namespace Networking
{
    public class Player : NetworkBehaviour
    {
        private WorldGenerator _worldGenerator;
        private SpaceShipsService _shipsService;
        private UsersService _usersService;
        private PlanetsService _planetsService;
        private OrbitsService _orbitsService;
        private CameraSystem _cameraSystem;


        [Inject]
        public void Construct(WorldGenerator worldGenerator, SpaceShipsService shipsService, 
            UsersService usersService, PlanetsService planetsService, OrbitsService orbitsService,
            CameraSystem cameraSystem)
        {
            _worldGenerator = worldGenerator;
            _shipsService = shipsService;
            _usersService = usersService;
            _planetsService = planetsService;
            _orbitsService = orbitsService;
            _cameraSystem = cameraSystem;
        }


        private void Start()
        {
            if (!isLocalPlayer) return;

            if (isServer)
                InitializeAsServer();

            InitializeAsClient();
        }

        [Server]
        private void InitializeAsServer()
        {
            Debug.Log("Initialized as server.");
            SendGenerateWorldRPC();
            _worldGenerator.Finished += OnWorldGenerated;
        }

        private void OnWorldGenerated()
        {
            var players = FindObjectsOfType<Player>();
            foreach (var player in players)
            {
                var user = new User();
                var ship = new SpaceShip();
                _usersService.Register(player, user);
                _shipsService.Register(user, ship);

                var shipView = _shipsService.CreateView(ship);
                var randomPlanet = FindObjectOfType<PlanetView>().Model;

                _orbitsService.RegisterOrbiting(ship, randomPlanet);
                _cameraSystem.SetTarget(shipView.transform);
            }
        }

        [Client]
        private void InitializeAsClient()
        {
        }

        [Command]
        private void SendRegistrationCommand()
        {
            
        }

        [ClientRpc]
        private void SendGenerateWorldRPC()
        {
            Debug.Log("Started generating world.");
            _worldGenerator.Generate();
        }
    }
}
