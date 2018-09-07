namespace Inheritance.MapObjects
{
    public interface IOwnable
    {
        int Owner { get; set; }
    }

    public interface IDefensible
    {
        Army Army { get; set; }
    }

    public interface ICollectable
    {
        Treasure Treasure { get; set; }
    }

    public class Dwelling : IOwnable
    {
        public int Owner { get; set; }
    }

    public class Mine : IOwnable, IDefensible
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
    }

    public class Creeps : ICollectable, IDefensible
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class ResourcePile : ICollectable
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IDefensible)
                if (!player.CanBeat(((IDefensible) mapObject).Army))
                {
                    player.Die();
                    return;
                }

            if (mapObject is IOwnable)
                ((IOwnable) mapObject).Owner = player.Id;
            if (mapObject is ICollectable)
                player.Comsume(((ICollectable) mapObject).Treasure);
        }
    }
}
