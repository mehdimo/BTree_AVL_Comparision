using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BalancedTrees
{    
    public class AVL
    {
       
        AVLNode root;
        public AVL()
        {
        }

        public int GetHeight()
        {
            return getHeight(root);
        }

        public void Insert(int data)
        {
            AVLNode newItem = new AVLNode(data);
            if (root == null)
            {
                root = newItem;
            }
            else
            {
                root = RecursiveInsert(root, newItem);//root = null so we dont lose track of the root and we assign a new root if necessary
            }
        }

        public AVLNode Search(int key)
        {
            return Search(root, key);
        }

        private AVLNode Search(AVLNode node, int key)
        {

            if (node.value == key)
                return root;
            else if (key < node.value)
                return Search(node.left, key);
            else
                return Search(node.right, key);
        }

        private AVLNode RecursiveInsert(AVLNode current, AVLNode n)
        {
            if (current == null)//base case, we reach this when we go left or right until current is null
            {
                current = n;
                return current;
            }
            else if (n.value < current.value)//if the new node is less than the current node
            {
                current.left = RecursiveInsert(current.left, n);//go left
                current = balance_tree(current);//calling balance after recursion
            }
            else if (n.value > current.value)//if the new node is greater than the current node
            {
                current.right = RecursiveInsert(current.right, n);
                current = balance_tree(current);
            }
            return current;
        }
        private AVLNode balance_tree(AVLNode current)
        {
            int b_factor = balance_factor(current);
            if (b_factor > 1)
            {
                if (balance_factor(current.left) > 0)
                {
                    current = RotateLL(current);
                }
                else
                {
                    current = RotateLR(current);
                }
            }
            else if (b_factor < -1)
            {
                if (balance_factor(current.right) > 0)
                {
                    current = RotateRL(current);
                }
                else
                {
                    current = RotateRR(current);
                }
            }
            return current;
        }


        public void PrintTree()
        {
            InOrderTraversTree(root);
            Console.ReadLine();
        }
        private void InOrderTraversTree(AVLNode current)
        {
            if (current != null)
            {
                InOrderTraversTree(current.left);
                Console.Write("({0}) ", current.value);
                InOrderTraversTree(current.right);
            }
        }
        private int max(int l, int r)//returns maximum of two integers
        {
            return l > r ? l : r;
        }
        private int getHeight(AVLNode current)
        {
            int height = 0;
            if (current != null)
            {
                int l = getHeight(current.left);
                int r = getHeight(current.right);
                int m = max(l, r);
                height = m + 1;
            }
            return height;
        }
        private int balance_factor(AVLNode current)
        {
            int l = getHeight(current.left);
            int r = getHeight(current.right);
            int b_factor = l - r;
            return b_factor;
        }
        private AVLNode RotateRR(AVLNode parent)
        {
            AVLNode pivot = parent.right;
            parent.right = pivot.left;
            pivot.left = parent;
            return pivot;
        }
        private AVLNode RotateLL(AVLNode parent)
        {
            AVLNode pivot = parent.left;
            parent.left = pivot.right;
            pivot.right = parent;
            return pivot;
        }
        private AVLNode RotateLR(AVLNode parent)
        {
            AVLNode pivot = parent.left;
            parent.left = RotateRR(pivot);
            return RotateLL(parent);
        }
        private AVLNode RotateRL(AVLNode parent)
        {
            AVLNode pivot = parent.right;
            parent.right = RotateLL(pivot);
            return RotateRR(parent);
        }
    }

    
}
