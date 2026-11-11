using Airborne.Pollutants;
using BeauUtil;
using FieldDay.Assets;
using FieldDay.SharedState;

namespace Airborne.Building {
    public sealed class BuildingScenario : SharedStateComponent {
        [AssetName(typeof(PollutantDefinition))] public StringHash32[] Pollutants;
    }
}