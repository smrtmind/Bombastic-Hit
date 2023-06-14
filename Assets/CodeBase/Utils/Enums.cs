namespace Codebase.Utils
{
    public static class Enums
    {
        public enum ParticleType
        {
            None,
            Explosion,
            EnemyDead
        }

        public enum ResourceType
        {
            Ammo,
            WallCrack,
            WallDamage,
            PopUp
        }

        public enum ColorType
        {
            Random,
            red,
            green,
            blue
        }

        public enum TutorStep
        {
            None,
            FirstStep,
            SecondStep
        }

        public enum LocationType
        {
            Single, 
            Double,
            Triple
        }

        public enum UIPanelType
        {
            None,
            Promo,
            Game
        }
    }
}
