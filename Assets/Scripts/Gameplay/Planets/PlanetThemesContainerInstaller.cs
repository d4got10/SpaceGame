using UnityEngine;
using Zenject;

namespace Gameplay.Planets
{
    public class PlanetThemesContainerInstaller : MonoInstaller
    {
        [SerializeField] private PlanetThemesContainer _container;

        public override void InstallBindings()
        {
            Container
                .Bind<PlanetThemesContainer>()
                .FromScriptableObject(_container)
                .AsSingle()
                .NonLazy();
        }
    }
}