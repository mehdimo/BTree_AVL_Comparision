namespace BalancedTrees
{
    using System;

    public class Item : IEquatable<Item>
    {
        public int Key { get; set; }

        public bool Equals(Item a)
        {
            return this.Key.Equals(a.Key);
        }
    }
}
