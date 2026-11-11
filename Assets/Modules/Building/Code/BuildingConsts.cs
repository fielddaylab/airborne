namespace Airborne.Building {
    static public class BuildingConsts {
        public const int MaxFloors = 3;
        public const int MaxRoomsPerFloor = 4;
        public const int MaxRooms = 2 + (MaxFloors * MaxRoomsPerFloor);

        public const int MaxPostsPerRoom = 32;

        public const int MaxFeaturesPerRoom = 3;
        public const int MaxFeatures = MaxFeaturesPerRoom * MaxRooms;

        public const int MaxResidents = 8;

        public const int MaxPassages = MaxRooms * 2;

        public const int MaxPollutants = 4;

        public const int MaxHours = 8;
    }
}