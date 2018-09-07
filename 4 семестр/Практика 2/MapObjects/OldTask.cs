//namespace Inheritance.MapObjects
//{
//    public interface IMapObject
//    {
//        void InteractWith(Player player);
//    }

//    public interface IDefensible
//    {
//        void FightWith(Player player);
//    }

//    public class Dwelling : IMapObject
//    {
//        public int Owner { get; set; }

//        public void InteractWith(Player player)
//        {
//            Owner = player.Id;
//        }
//    }

//    public class Mine : IMapObject, IDefensible
//    {
//        public int Owner { get; set; }
//        public Army Army { get; set; }

//        public void InteractWith(Player player)
//        {
//            FightWith(player);
//        }

//        public void FightWith(Player player)
//        {
//            if (player.CanBeat(Army))
//                Owner = player.Id;
//            else player.Die();
//        }
//    }

//    public class Creeps : IMapObject, IDefensible
//    {
//        public Army Army { get; set; }
//        public Treasure Treasure { get; set; }

//        public void InteractWith(Player player)
//        {
//            FightWith(player);
//        }

//        public void FightWith(Player player)
//        {
//            if (player.CanBeat(Army))
//                player.Comsume(Treasure);
//            else
//                player.Die();
//        }
//    }

//    public class ResourcePile : IMapObject
//    {
//        public Treasure Treasure { get; set; }

//        public void InteractWith(Player player)
//        {
//            player.Comsume(Treasure);
//        }
//    }

//    public static class Interaction
//    {
//        public static void Make(Player player, IMapObject mapObject)
//        {
//            mapObject.InteractWith(player);
//        }
//    }
//}{
//    public class OldTask
//    {
        
//    }
//}