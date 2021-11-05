using UnityEngine;
using PlanetGenerator;

namespace Gameplay.Planets
{
    [CreateAssetMenu(fileName = "Planet Themes Container", menuName = "Gameplay/Planets/Themes Container")]
    public class PlanetThemesContainer : ScriptableObject
    {
        [SerializeField] private PlanetThemesColorSettingsDictionary _data;

        public ColorSettings this[PlanetThemes theme] => _data[theme];
    }
}