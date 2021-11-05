using Gameplay.Models;
using Mirror;
using PlanetGenerator;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Planets
{
    public class PlanetsService : NetworkBehaviour
    {
        [SerializeField] private List<PlanetView> _prefabs;


        private SyncDictionary<Planet, PlanetView> _planetViews = new SyncDictionary<Planet, PlanetView>();


        public PlanetView Register(Planet planet)
        {
            var view = Instantiate(_prefabs[Random.Range(0, _prefabs.Count)]);
            RegisterView(planet, view);
            return view;
        }


        public void RegisterView(Planet planet, PlanetView view)
        {
            _planetViews[planet] = view;
            view.Init(planet);
        }

        public PlanetView GetView(Planet planet)
        {
            return _planetViews[planet];
        }
    }
}