using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class WorldInstaller : MonoInstaller
    {
        [SerializeField] private World _world;

        public override void InstallBindings()
        {
            Container
                .Bind<World>()
                .FromInstance(_world)
                .AsSingle()
                .NonLazy();
        }
    }
}