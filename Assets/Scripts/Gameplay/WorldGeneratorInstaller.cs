using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class WorldGeneratorInstaller : MonoInstaller
    {
        [SerializeField] private WorldGenerator _worldGenerator;


        public override void InstallBindings()
        {
            Container
                .Bind<WorldGenerator>()
                .FromInstance(_worldGenerator)
                .AsSingle()
                .NonLazy();
        }
    }
}