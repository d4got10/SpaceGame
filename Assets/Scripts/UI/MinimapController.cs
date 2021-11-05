using Gameplay;
using Gameplay.Models;
using Gameplay.Planets;
using Gameplay.Ships;
using PlanetGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MinimapController : MonoBehaviour
    {
        [SerializeField] private Button _planetOnMinimapPrefab;
        [SerializeField] private RectTransform _parent;


        private OrbitsService _orbitsService;
        private SpaceShipsService _shipsService;
        private PlanetsService _planetsService;
        private World _world;


        [Inject]
        public void Construct(World world, SpaceShipsService shipsService, OrbitsService orbitsService, PlanetsService planetsService)
        {
            _orbitsService = orbitsService;
            _shipsService = shipsService;
            _planetsService = planetsService;
            _world = world;
        }

        private void Awake()
        {
            _orbitsService.ShipChangedOrbit += OnShipChangedOrbit;
        }
        private void OnDestroy()
        {
            _orbitsService.ShipChangedOrbit -= OnShipChangedOrbit;
        }


        private void OnShipChangedOrbit(SpaceShip ship, Planet newPlanet)
        {
            foreach(Transform child in _parent)
            {
                Destroy(child.gameObject);
            }

            var shipView = _shipsService.GetView(ship);
            var newPlanetView = _planetsService.GetView(newPlanet);
            float halfExtent = _world.DistanceBetweenPlanets / 2;
            var hits = Physics.BoxCastAll(newPlanetView.transform.position, Vector3.one * halfExtent, _world.transform.forward); 
            foreach(var hit in hits)
            {
                var planetView = hit.collider.gameObject.GetComponentInParent<PlanetView>();
                if (planetView == newPlanetView) continue;

                if(planetView != null)
                {
                    var planetOnMinimap = Instantiate(_planetOnMinimapPrefab, _parent);
                    var rect = planetOnMinimap.GetComponent<RectTransform>();

                    var position = (hit.collider.transform.position - newPlanetView.transform.position) / halfExtent;
                    var anchorPosition = 0.5f * (new Vector2(position.x, position.z) + Vector2.one);
                    rect.anchorMin = anchorPosition;
                    rect.anchorMax = anchorPosition;
                    rect.anchoredPosition = Vector2.zero;

                    planetOnMinimap.onClick.AddListener(OnPlanetOnMinimapClick);

                    void OnPlanetOnMinimapClick()
                    {
                        _orbitsService.RegisterOrbiting(ship, planetView.Model);
                    }
                }
            }
        }
    }
}