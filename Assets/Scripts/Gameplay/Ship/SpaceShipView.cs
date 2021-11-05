using System.Collections;
using UnityEngine;
using Gameplay.Models;
using Mirror;

namespace Gameplay.Ships
{
    public class SpaceShipView : NetworkBehaviour
    {
        [SerializeField] private ParticleSystem _engineParticles;


        public SpaceShip Model { get; private set; }
        public bool EngineIsActive => _engineParticles.isPlaying;


        public void Init(SpaceShip model)
        {
            Model = model;
        }


        public void StartEngine()
        {
            _engineParticles.Play();
        }

        public void StopEngine()
        {
            _engineParticles.Stop();
        }
    }
}