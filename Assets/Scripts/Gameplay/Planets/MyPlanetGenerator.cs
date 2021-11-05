using UnityEngine;
using PlanetGenerator;
using Zenject;
using System.Linq;
using System.Collections;

namespace Gameplay.Planets {
    public class MyPlanetGenerator : MonoBehaviour
    {
        public event System.Action<PlanetView> GeneratedPlanet;


        private PlanetThemesContainer _planetThemesContainer;
        private World _world;


        [Inject]
        public void Construct(World world, PlanetThemesContainer planetThemesContainer)
        {
            _planetThemesContainer = planetThemesContainer;
            _world = world;
        }


        public void GeneratePlanets(int planetCount)
        {
            StartCoroutine(GeneratePlanetsCoroutine());

            IEnumerator GeneratePlanetsCoroutine()
            {
                for (int i = 0; i < planetCount; i++)
                {
                    bool planetIsCreated = false;

                    var planet = CreatePlanet(Vector3.zero, GetRandomColorSettings());
                    planet.MeshUpdated += OnPlanetCreated;
                    planet.transform.parent = _world.transform;

                    yield return new WaitUntil(() => planetIsCreated);

                    void OnPlanetCreated()
                    {
                        planetIsCreated = true;
                        planet.MeshUpdated -= OnPlanetCreated;
                        GeneratedPlanet?.Invoke(planet);
                    }
                }
            }
        }


        private PlanetView CreatePlanet(Vector3 position, ColorSettings settings)
        {
            GameObject planetGO = new GameObject("Planet");

            planetGO.transform.position = position;

            var planet = planetGO.AddComponent<PlanetView>();

            planet.ColorSettings = settings;
            planet.Resolution = 128;
            planet.FaceCount = 2;
            planet.PlanetNoiseSettings = new NoiseSettings[6];
            if (settings == _planetThemesContainer[PlanetThemes.Hot])
            {
                planet.PlanetNoiseSettings[0] = new NoiseSettings(1, 8, 1, planet.transform.position, NoiseSettings.NoiseTypes.Soft);
                planet.PlanetNoiseSettings[1] = new NoiseSettings(1, 16, 2, planet.transform.position, NoiseSettings.NoiseTypes.Soft);
            }
            planet.UpdatePlanetMesh();

            return planet;
        }

        private ColorSettings GetRandomColorSettings()
        {
            var allPlanetThemes = System.Enum.GetValues(typeof(PlanetThemes));
            var randomPlanetTheme = (PlanetThemes)allPlanetThemes.GetValue(Random.Range(0, allPlanetThemes.Length));
            return _planetThemesContainer[randomPlanetTheme];
        }
    }
}