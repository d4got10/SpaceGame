using UnityEngine;
using Zenject;

namespace Gameplay.Ships
{
    public class SpaceShipsServiceInstaller : MonoInstaller
    {
        [SerializeField] private SpaceShipsService _service;


        public override void InstallBindings()
        {
            Container
                .Bind<SpaceShipsService>()
                .FromInstance(_service)
                .AsSingle()
                .NonLazy();
        }
    }
}