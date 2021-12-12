namespace BalancedTrees
{
    using System.Collections.Generic;

    public class BTreeNode : INode
    {
        private int degree;

        public BTreeNode(int degree)
        {
            this.degree = degree;
            this.Children = new List<BTreeNode>(degree);
            this.Items = new List<Item>(degree);
        }

        public List<BTreeNode> Children { get; set; }

        public List<Item> Items { get; set; }

        public bool IsLeaf
        {
            get
            {
                return this.Children.Count == 0;
            }
        }

        public bool HasReachedMaxEntries
        {
            get
            {
                return this.Items.Count == (2 * this.degree) - 1;
            }
        }

        public bool HasReachedMinEntries
        {
            get
            {
                return this.Items.Count == this.degree - 1;
            }
        }
    }
}
