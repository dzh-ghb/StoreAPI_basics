namespace Api.Common
{
    public static class SharedData
    {
        public static class Roles
        {
            public const string Admin = "admin";
            public const string Consumer = "consumer";

            // свойство
            public static IReadOnlyList<string> AllRoles
            {
                get => new List<string>() { Admin, Consumer };
            }

            #region test
            // public static List<string> GetAllRoles()
            // {
            //     return new List<string>() { Admin, Consumer };
            // }
            #endregion
        }

        public static class OrderStatuses
        {
            public const string Pending = "pending";
            public const string ReadyToShip = "ready_to_ship";
            public const string Completed = "completed";

            public static IReadOnlyList<string> AllStatuses
            {
                get => new List<string>() { Pending, ReadyToShip, Completed };
            }
        }
    }
}