namespace BalancedTrees
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public class BTree
    {
        public BTree(int degree)
        {
            this.Root = new BTreeNode(degree);
            this.Degree = degree;
            this.Height = 1;
        }

        public BTreeNode Root { get; private set; }

        public int Degree { get; private set; }

        public int Height { get; private set; }

        public Item Search(int key)
        {
            return SearchInternal(this.Root, key);
        }

        public void Insert(int newKey, int newPointer)
        {
            if (!this.Root.HasReachedMaxEntries)
            {
                InsertNonFull(this.Root, newKey, newPointer);
                return;
            }

            BTreeNode oldRoot = this.Root;
            this.Root = new BTreeNode(this.Degree);
            this.Root.Children.Add(oldRoot);
            SplitChild(this.Root, 0, oldRoot);
            InsertNonFull(this.Root, newKey, newPointer);

            this.Height++;
        }

        public void Delete(int keyToDelete)
        {
            DeleteInternal(this.Root, keyToDelete);

            if (this.Root.Items.Count == 0 && !this.Root.IsLeaf)
            {
                this.Root = this.Root.Children.Single();
                this.Height--;
            }
        }

        private void DeleteInternal(BTreeNode node, int keyToDelete)
        {
            int i = node.Items.TakeWhile(entry => keyToDelete.CompareTo(entry.Key) > 0).Count();

            if (i < node.Items.Count && node.Items[i].Key.CompareTo(keyToDelete) == 0)
            {
                DeleteKeyFromNode(node, keyToDelete, i);
                return;
            }

            if (!node.IsLeaf)
            {
                DeleteKeyFromSubtree(node, keyToDelete, i);
            }
        }

        private void DeleteKeyFromSubtree(BTreeNode parentNode, int keyToDelete, int subtreeIndexInNode)
        {
            BTreeNode childNode = parentNode.Children[subtreeIndexInNode];

            if (childNode.HasReachedMinEntries)
            {
                int leftIndex = subtreeIndexInNode - 1;
                BTreeNode leftSibling = subtreeIndexInNode > 0 ? parentNode.Children[leftIndex] : null;

                int rightIndex = subtreeIndexInNode + 1;
                BTreeNode rightSibling = subtreeIndexInNode < parentNode.Children.Count - 1
                                                ? parentNode.Children[rightIndex]
                                                : null;
                
                if (leftSibling != null && leftSibling.Items.Count > this.Degree - 1)
                {
                    childNode.Items.Insert(0, parentNode.Items[subtreeIndexInNode]);
                    parentNode.Items[subtreeIndexInNode] = leftSibling.Items.Last();
                    leftSibling.Items.RemoveAt(leftSibling.Items.Count - 1);

                    if (!leftSibling.IsLeaf)
                    {
                        childNode.Children.Insert(0, leftSibling.Children.Last());
                        leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                    }
                }
                else if (rightSibling != null && rightSibling.Items.Count > this.Degree - 1)
                {
                    childNode.Items.Add(parentNode.Items[subtreeIndexInNode]);
                    parentNode.Items[subtreeIndexInNode] = rightSibling.Items.First();
                    rightSibling.Items.RemoveAt(0);

                    if (!rightSibling.IsLeaf)
                    {
                        childNode.Children.Add(rightSibling.Children.First());
                        rightSibling.Children.RemoveAt(0);
                    }
                }
                else
                {
                    if (leftSibling != null)
                    {
                        childNode.Items.Insert(0, parentNode.Items[subtreeIndexInNode]);
                        var oldEntries = childNode.Items;
                        childNode.Items = leftSibling.Items;
                        childNode.Items.AddRange(oldEntries);
                        if (!leftSibling.IsLeaf)
                        {
                            var oldChildren = childNode.Children;
                            childNode.Children = leftSibling.Children;
                            childNode.Children.AddRange(oldChildren);
                        }

                        parentNode.Children.RemoveAt(leftIndex);
                        parentNode.Items.RemoveAt(subtreeIndexInNode);
                    }
                    else
                    {
                        Debug.Assert(rightSibling != null, "Node should have at least one sibling");
                        childNode.Items.Add(parentNode.Items[subtreeIndexInNode]);
                        childNode.Items.AddRange(rightSibling.Items);
                        if (!rightSibling.IsLeaf)
                        {
                            childNode.Children.AddRange(rightSibling.Children);
                        }

                        parentNode.Children.RemoveAt(rightIndex);
                        parentNode.Items.RemoveAt(subtreeIndexInNode);
                    }
                }
            }

            this.DeleteInternal(childNode, keyToDelete);
        }
        
        private void DeleteKeyFromNode(BTreeNode node, int keyToDelete, int keyIndexInNode)
        {
            if (node.IsLeaf)
            {
                node.Items.RemoveAt(keyIndexInNode);
                return;
            }

            BTreeNode predecessorChild = node.Children[keyIndexInNode];
            if (predecessorChild.Items.Count >= this.Degree)
            {
                Item predecessor = this.DeletePredecessor(predecessorChild);
                node.Items[keyIndexInNode] = predecessor;
            }
            else
            {
                BTreeNode successorChild = node.Children[keyIndexInNode + 1];
                if (successorChild.Items.Count >= this.Degree)
                {
                    Item successor = this.DeleteSuccessor(predecessorChild);
                    node.Items[keyIndexInNode] = successor;
                }
                else
                {
                    predecessorChild.Items.Add(node.Items[keyIndexInNode]);
                    predecessorChild.Items.AddRange(successorChild.Items);
                    predecessorChild.Children.AddRange(successorChild.Children);

                    node.Items.RemoveAt(keyIndexInNode);
                    node.Children.RemoveAt(keyIndexInNode + 1);

                    this.DeleteInternal(predecessorChild, keyToDelete);
                }
            }
        }

        private Item DeletePredecessor(BTreeNode node)
        {
            if (node.IsLeaf)
            {
                var result = node.Items[node.Items.Count - 1];
                node.Items.RemoveAt(node.Items.Count - 1);
                return result;
            }

            return this.DeletePredecessor(node.Children.Last());
        }

        private Item DeleteSuccessor(BTreeNode node)
        {
            if (node.IsLeaf)
            {
                var result = node.Items[0];
                node.Items.RemoveAt(0);
                return result;
            }

            return this.DeletePredecessor(node.Children.First());
        }

        private Item SearchInternal(BTreeNode node, int key)
        {
            int i = node.Items.TakeWhile(entry => key.CompareTo(entry.Key) > 0).Count();

            if (i < node.Items.Count && node.Items[i].Key.CompareTo(key) == 0)
            {
                return node.Items[i];
            }

            return node.IsLeaf ? null : this.SearchInternal(node.Children[i], key);
        }

        private void SplitChild(BTreeNode parentNode, int nodeToBeSplitIndex, BTreeNode nodeToBeSplit)
        {
            var newNode = new BTreeNode(this.Degree);

            parentNode.Items.Insert(nodeToBeSplitIndex, nodeToBeSplit.Items[this.Degree - 1]);
            parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

            newNode.Items.AddRange(nodeToBeSplit.Items.GetRange(this.Degree, this.Degree - 1));
            
            nodeToBeSplit.Items.RemoveRange(this.Degree - 1, this.Degree);

            if (!nodeToBeSplit.IsLeaf)
            {
                newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(this.Degree, this.Degree));
                nodeToBeSplit.Children.RemoveRange(this.Degree, this.Degree);
            }
        }

        private void InsertNonFull(BTreeNode node, int newKey, int newPointer)
        {
            int positionToInsert = node.Items.TakeWhile(entry => newKey.CompareTo(entry.Key) >= 0).Count();
            
            if (node.IsLeaf)
            {
                node.Items.Insert(positionToInsert, new Item() { Key = newKey });
                return;
            }

            BTreeNode child = node.Children[positionToInsert];
            if (child.HasReachedMaxEntries)
            {
                this.SplitChild(node, positionToInsert, child);
                if (newKey.CompareTo(node.Items[positionToInsert].Key) > 0)
                {
                    positionToInsert++;
                }
            }

            this.InsertNonFull(node.Children[positionToInsert], newKey, newPointer);
        }
    }
}
