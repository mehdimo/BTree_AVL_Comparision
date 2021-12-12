namespace BalancedTrees
{
    public class AVLNode : INode
    {
        public int value;
        public AVLNode left;
        public AVLNode right;
        public AVLNode(int value)
        {
            this.value = value;
        }

        public bool IsLeaf
        {
            get { return left == null && right == null; }
        }
    }
}
