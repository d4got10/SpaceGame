using UnityEngine;
using Zenject;

namespace Utils
{
    public class ServiceInstaller<T> : MonoInstaller where T : MonoBehaviour
    {
        [SerializeField] private T _service;

        public override void InstallBindings()
        {
            Container
                .Bind<T>()
                .FromInstance(_service)
                .AsSingle()
                .NonLazy();
        }
    }
}