using Gameplay.Models;
using Gameplay.Ships;
using Mirror;
using PlanetGenerator;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay.Planets
{
    public class OrbitsService : NetworkBehaviour
    {
        public event System.Action<SpaceShip, Planet> ShipChangedOrbit;


        private SyncDictionary<Planet, List<SpaceShip>> _shipsOnOrbits = new SyncDictionary<Planet, List<SpaceShip>>();
        private SyncDictionary<SpaceShip, Planet> _shipsOrbitingPlanets = new SyncDictionary<SpaceShip, Planet>();


        private SpaceShipsService _shipsService;
        private PlanetsService _planetsService;
        private World _world;


        [Inject]
        public void Construct(World world, SpaceShipsService shipsService, PlanetsService planetsService)
        {
            _shipsService = shipsService;
            _planetsService = planetsService;
            _world = world;
        }


        private void Awake()
        {
            _shipsOrbitingPlanets.Callback += OnShipChangedOrbit; 
        }
        private void OnDestroy()
        {
            _shipsOrbitingPlanets.Callback -= OnShipChangedOrbit;
        }
        private void OnShipChangedOrbit(SyncIDictionary<SpaceShip, Planet>.Operation op, SpaceShip key, Planet item)
        {
            if (op == SyncIDictionary<SpaceShip, Planet>.Operation.OP_ADD || op == SyncIDictionary<SpaceShip, Planet>.Operation.OP_SET)
            {
                ShipChangedOrbit?.Invoke(key, item);
            }
        }


        public void RegisterOrbiting(SpaceShip ship, Planet planet)
        {
            if(_shipsOrbitingPlanets.TryGetValue(ship, out var prevPlanet))
            {
                _shipsOnOrbits[prevPlanet].Remove(ship);
            }

            if(_shipsOnOrbits.TryGetValue(planet, out var ships))
            {
                ships.Remove(ship);
            }
            if (_shipsOnOrbits.ContainsKey(planet))
            {
                _shipsOnOrbits[planet].Add(ship);
            }
            else
            {
                _shipsOnOrbits[planet] = new List<SpaceShip> { ship };
            }
            _shipsOrbitingPlanets[ship] = planet;
        }


        public void Update()
        {
            foreach (var shipsOnOrbits in _shipsOnOrbits)
            {
                var planet = shipsOnOrbits.Key;
                foreach (var ship in shipsOnOrbits.Value)
                {
                    Orbit(_shipsService.GetView(ship), _planetsService.GetView(planet));
                }
            }
        }


        private void Orbit(SpaceShipView shipView, PlanetView planetView)
        {
            if (Vector3.Distance(shipView.transform.position, planetView.transform.position) >= _world.PlanetSize * 2.05f)
            {
                MoveTowardsPlanet(shipView, planetView);
                if(shipView.EngineIsActive == false)
                    shipView.StartEngine();
            }
            else
            {
                if(shipView.EngineIsActive)
                    shipView.StopEngine();
                shipView.transform.RotateAround(planetView.transform.position, _world.transform.up, 10 * Time.deltaTime);
            }
            var rotation = Quaternion.LookRotation(planetView.transform.position - shipView.transform.position, _world.transform.up);
            shipView.transform.rotation = Quaternion.Slerp(shipView.transform.rotation, rotation, 0.03f);
        }

        private void MoveTowardsPlanet(SpaceShipView shipView, PlanetView planetView)
        {
            var direction = planetView.transform.position - shipView.transform.position;
            direction -= direction.normalized * _world.PlanetSize * 2;
            var clampedDirection = Vector3.ClampMagnitude(direction, 100f * Time.deltaTime);
            shipView.transform.position += clampedDirection;
        }
    }
}