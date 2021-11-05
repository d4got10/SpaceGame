using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Gameplay.Planets;
using Mirror;
using PlanetGenerator;

namespace Gameplay
{
    public class WorldGenerator : NetworkBehaviour
    {
        public event System.Action Finished;


        [SerializeField] private MyPlanetGenerator _planetGenerator;


        private int _planetsLeftToGenerate;
        private HaltonSequence positionsequence = new HaltonSequence();
        private World _world;
        private PlanetsService _planetsService;


        [Inject]
        public void Construct(World world, PlanetsService planetsService)
        {
            _world = world;
            _planetsService = planetsService;
        }


        private void OnValidate()
        {
            if(_planetGenerator == null)
            {
                throw new System.NullReferenceException(nameof(_planetGenerator));
            }
        }

        private void OnDestroy()
        {
            _planetGenerator.GeneratedPlanet -= OnGeneratedPlanet;
        }

        [Server]
        public void Generate()
        {
            GenerateWorld(Random.Range(0, int.MaxValue));
        }


        [ClientRpc]
        private void GenerateWorld(int randomSeed)
        {
            Random.InitState(randomSeed);

            int planetCount = 10;

            _planetsLeftToGenerate = planetCount;
            _planetGenerator.GeneratePlanets(planetCount);
            //for(int i = 0; i < planetCount; i++)
            //{
            //    var view = _planetsService.Register(new Models.Planet());
            //    OnGeneratedPlanet(view);
            //}
            _planetGenerator.GeneratedPlanet += OnGeneratedPlanet;
        }

        private void OnGeneratedPlanet(PlanetView planetView)
        {
            planetView.transform.position = GetAvailablePosition();
            planetView.transform.localScale = Vector3.one * _world.PlanetSize;
            _planetsService.RegisterView(new Models.Planet(), planetView);
            _planetsLeftToGenerate--;
            if (_planetsLeftToGenerate == 0)
                Finished?.Invoke();
        }


        private Vector3 GetAvailablePosition()
        {
            int triesThreshold = 100000;
            int tries = 0;

            var position = GetCurrentPositionInSequence();  
            while(PositionIsAvailable(position) == false)
            {
                positionsequence.Increment();
                position = GetCurrentPositionInSequence();

                if(tries > triesThreshold)
                    throw new System.Exception("Infinite loop");
            }
            return position;
        }

        private Vector3 GetCurrentPositionInSequence()
        {
            var position = positionsequence.m_CurrentPos * _world.DistanceBetweenPlanets;
            position.y = 0;
            return position;
        }

        private bool PositionIsAvailable(Vector3 position)
        {
            return Physics.CheckSphere(position, _world.PlanetSize * 5) == false;
        }
    }
}