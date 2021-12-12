using BalancedTrees;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    public class Program
    {
        static void Main(string[] args)
        {
            string file = "BTreeAnalysis.txt";
            string file2 = "AVLAnalysis.txt";
            int btreeDegree = 3;
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}", "Input Size", "Insert Time", "Search Time", "Tree Height");
            }

            using (StreamWriter sw = new StreamWriter(file2))
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}", "Input Size", "Insert Time", "Search Time", "Tree Height");
            }

            for (int n = 0; n <= 10000; n+=1000)
            {
                Console.WriteLine("Running for n={0}.", n);
                var btree = new BTree(btreeDegree);
                var avlTree = new AVL();

                List<int> keys = GenerateData(n);
                
                long t1 = DateTime.Now.Ticks;

                // B-Tree insertion operation
                for (int j = 0; j < keys.Count; j++)
                {
                    btree.Insert(keys[j], keys[j]);
                }
                long t2 = DateTime.Now.Ticks;
                long insertTime = t2 - t1;

                t1 = DateTime.Now.Ticks;
                // B-Tree search operation
                for (int j = 0; j < keys.Count; j++)
                {
                    Item entry = btree.Search(keys[j]);
                }
                t2 = DateTime.Now.Ticks;
                long SearchTime = t2 - t1;
                
                using (StreamWriter sw = new StreamWriter(file, true))
                {
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}", n, insertTime, SearchTime, btree.Height);
                }
              
                //AVL insertion
                t1 = DateTime.Now.Ticks;
                for (int j = 0; j < keys.Count; j++)
                {
                    avlTree.Insert(keys[j]);
                }
                t2 = DateTime.Now.Ticks;
                long AvlInsertTime = t2 - t1;

                t1 = DateTime.Now.Ticks;
                // AVL search operation
                for (int j = 0; j < keys.Count; j++)
                {
                    var entry = avlTree.Search(keys[j]);
                }
                t2 = DateTime.Now.Ticks;
                long AvlSearchTime = t2 - t1;

                using (StreamWriter sw = new StreamWriter(file2, true))
                {
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}", n, AvlInsertTime, AvlSearchTime, avlTree.GetHeight());
                }
            }
            Console.ReadKey();
        }

        public static List<int> GenerateData(int size)
        {
            List<int> list = new List<int>();
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                int r = rand.Next();
                list.Add(r);
            }
            return list;
        }
    }
}
