//Names: Khushi Chauhan; Anubhav Mehandru; Farhana Zahan
//Assignment 3 - 3020- Part c
//To modify a 2-3-4 B-Tree so that it behaves as a R-B Tree
//-----------------------------------------------------------------------------

//libraries
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public enum Color { RED, BLACK };       // Colors of the red-black tree

public interface ISearchable<T>
{
    void Add(T item, Color rb);
    void Print();
}

//-------------------------------------------------------------------------

// Implementation:  BSTforRBTree

class BSTforRBTree<T> : ISearchable<T> where T : IComparable
{

    // Common generic node class for a BSTforRBTree

    private class Node
    {
        // Read/write properties

        public T Item;
        public Color RB;
        public Node Left;
        public Node Right;

        public Node(T item, Color rb)
        {
            Item = item;
            RB = rb;
            Left = Right = null;
        }
    }

    private Node root;

    public BSTforRBTree()
    {
        root = null;    // Empty BSTforRBTree
    }

    // Add 
    // Insert an item into a BSTforRBTRee
    // Duplicate items are not inserted
    // Worst case time complexity:  O(log n) 
    // since the maximum depth of a red-black tree is O(log n)

    public void Add(T item, Color rb)
    {
        Node curr;
        bool inserted = false;

        if (root == null)
            root = new Node(item, rb);   // Create a root
        else
        {
            curr = root;
            while (!inserted)
            {
                if (item.CompareTo(curr.Item) < 0)
                {
                    if (curr.Left == null)              // Empty spot
                    {
                        curr.Left = new Node(item, rb);
                        inserted = true;
                    }
                    else
                        curr = curr.Left;               // Move left
                }
                else
                    if (item.CompareTo(curr.Item) > 0)
                {
                    if (curr.Right == null)         // Empty spot
                    {
                        curr.Right = new Node(item, rb);
                        inserted = true;
                    }
                    else
                        curr = curr.Right;          // Move right
                }
                else
                    inserted = true;                // Already inserted
            }
        }
    }

    public void Print()
    {
        Print(root, 0);                // Call private, recursive Print
        Console.WriteLine();
    }

    // Print
    // Inorder traversal of the BSTforRBTree
    // Time complexity:  O(n)

    private void Print(Node node, int k)
    {
        string s;
        string t = new string(' ', k);

        if (node != null)
        {
            Print(node.Right, k + 4);
            s = node.RB == Color.RED ? "R" : "B";
            Console.WriteLine(t + node.Item.ToString() + s);
            Print(node.Left, k + 4);
        }
    }
}

//-----------------------------------------------------------------------------

// Represents a single node in a 2-3-4 tree with a list of children nodes and keys.
class Node<T>
{
    public List<Node<T>> children;
    public List<T> keys;

    //  Constructor for Node class.
    public Node()
    {
        children = new List<Node<T>>();
        keys = new List<T>();
    }

    // Check if this node is a leaf with no children
    public bool isLeafNode()
    {
        return children.Count == 0;
    }
}

class TwoThreeFourTree<T> where T : IComparable
{
    private Node<T> root;   // The root for our tree
    private int maxKeysPerNode;
    private int minKeysPerNode;
    private int maxChildrenPerNode;

    // Constructor for the TwoThreeFourTree class
    public TwoThreeFourTree()
    {
        int t = 2;
        this.root = new Node<T>();
        this.maxKeysPerNode = 2 * t - 1;
        this.minKeysPerNode = t - 1;
        this.maxChildrenPerNode = 2 * t;

    }

    /*  Insert(T key)
     * 
     *  Parameter : key - the key to insert of type T
     *  Result : returns true on success and false if key was already in the tree.
     *
     */
    public bool Insert(T key)
    {
        if (this.root.keys.Count == this.maxKeysPerNode)
        {
            Node<T> newRoot = new Node<T>();

            newRoot.children.Add(this.root);
            SplitNode(newRoot, 0);
            this.root = newRoot;

        }
        return Insert(this.root, key);
    }

    /*  Insert(Node<T> node, T key)
     *  
     *  Description: Private method to help the public insert method
     *  Parameter: node - the current node to attempt to insert the key into.
     *             key - the key to be inserted.
     *  Result: Returns true if the key was successfully inserted, false if the key already exists in this subtree.
     *
     */
    private bool Insert(Node<T> node, T key)
    {
        if (node.isLeafNode())
        {
            int index = 0;
            for (; index <= node.keys.Count; index++)
            {
                if (index == node.keys.Count ||
                    key.CompareTo(node.keys[index]) < 0)
                {
                    break;
                }
            }

            if (!node.keys.Contains(key))
            {
                node.keys.Insert(index, key);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            int childIndex = this.getChildIndex(node, key);
            if (childIndex == -1)
            {
                return false;
            }
            this.SplitNode(node, childIndex);
            childIndex = this.getChildIndex(node, key);
            return this.Insert(node.children[childIndex], key);
        }

    }

    /*  Delete(T key)
     * 
     *  Parameter: key - the key to delete from the tree.
     *  Result: Returns true if the key was found and deleted, false otherwise.
     *
     */
    public bool Delete(T key)
    {
        bool keyFound = Find(this.root, key);

        if (!keyFound)
        {
            Console.WriteLine("\nError : Item not found in tree");
            return false;
        }

        Delete(this.root, key);

        if (this.root.keys.Count == 0 && this.root.children.Count > 0)
        {
            this.root = this.root.children[0];
        }

        return true;
    }

    /*  Delete(Node<T> node, T key)
     * 
     * Description: Private method to help the public delete method
     * Parameter: node - the current node to attempt to delete the key from.
     *             key - the key to be deleted.
     * Result: Returns true if the key was successfully deleted, false if the key does not exist in this subtree.
     *
     */
    private void Delete(Node<T> node, T key)
    {
        if (node.isLeafNode())
        {
            node.keys.Remove(key);
            return;
        }

        int index = 0;
        for (; index < node.keys.Count; index++)
        {
            if (index == node.keys.Count || key.CompareTo(node.keys[index]) < 0)
            {
                break;
            }
        }

        Node<T> child = node.children[index];
        Delete(child, key);

        if (child.keys.Count < this.minKeysPerNode)
        {
            ensureTValues(node, index);
        }
    }

    /*  Search(T key)
     * 
     * Parameter: key - the key to search for within the tree.
     * Result: Returns true if the key exists in the tree, false otherwise.
     *
     */
    public bool Search(T key)
    {
        return Search(this.root, key);

    }

    /*  Search(Node<T> node, T key)
     *      
     *  Description: Private method to help the public Search method
     *  Parameter: node - the node to start the search from.
     *             key - the key to search for.
     *  Result: Returns true if the key exists in this subtree, false otherwise.
     *
     */
    private bool Search(Node<T> node, T key)
    {
        if (node == null)
        {
            return false;
        }
        else if (node.keys.Contains(key))
        {
            return true;
        }

        else
        {
            if (node.children.Count == 0)
            {
                return false;
            }
            int index = 0;
            for (; index < node.children.Count; index++)
            {
                if (index == node.keys.Count ||
                    key.CompareTo(node.keys[index]) < 0)
                {
                    break;
                }
            }
            return Search(node.children[index], key);

        }
    }

    /*  Print(bool suppressOutput)
     * 
     *  Parameter: suppressOutput - if true, suppresses the printing of the tree's structure to the console.
     *  Result: Returns a string representation of the tree.
     *
     */
    public String Print(bool suppressOutput = false)
    {
        String printout = "";
        if (this.root == null || root.keys.Count == 0)
        {
            return printout;
        }
        this.Print(this.root, ref printout);

        printout = printout.Substring(0, printout.Length - 2);
        if (!suppressOutput)
        {
            Console.WriteLine(printout);
        }
        return printout;
    }

    /*  Print(Node<T> node, ref string printout)
     *    
     *  Description: Private method to help the public Print method
     *  Parameter: node - the node to start printing from.
     *             printout - the string to append the output to.
     *  Result: Appends the subtree's keys to the printout string.
     *
     */
    private void Print(Node<T> node, ref String printout)
    {
        if (node.isLeafNode())
        {
            foreach (T key in node.keys)
            {
                printout += key.ToString() + ", ";
            }
        }
        else
        {
            for (int index = 0; index < node.children.Count; index++)
            {
                Print(node.children[index], ref printout);
                if (index < node.keys.Count)
                {
                    printout += node.keys[index].ToString() + ", ";
                }

            }
        }
    }

    /*  Convert()
     * 
     *  Result: Converts the entire 2-3-4 tree into a Red-Black tree and returns it.
     *
     */
    public BSTforRBTree<T> Convert()
    {
        BSTforRBTree<T> rbTree = new BSTforRBTree<T>();
        if (root == null) return rbTree;

        ConvertNode(root, ref rbTree, Color.BLACK);
        return rbTree;
    }

    /*  ConvertNode(Node<T> node, ref BSTforRBTree<T> rbTree, Color color)
    * 
    *  Description: Private method to help the public Convert method
    *  Parameter: node - the node to convert.
    *             rbTree - the Red-Black tree being constructed.
    *             color - the color to assign to the node in the Red-Black tree.
    *  Result: Recursively converts nodes from the 2-3-4 tree to the Red-Black tree.
    *
    */
    private void ConvertNode(Node<T> node, ref BSTforRBTree<T> rbTree, Color color)
    {
        if (node == null) return;

        Color nodeColor = color;

        // Convert based on the type of node in the 2-3-4 tree.
        switch (node.keys.Count)
        {
            case 1: // 2-node
                rbTree.Add(node.keys[0], nodeColor);
                break;
            case 2: // 3-node
                rbTree.Add(node.keys[1], Color.BLACK);
                rbTree.Add(node.keys[0], Color.RED);
                break;
            case 3: // 4-node
                rbTree.Add(node.keys[1], Color.BLACK);
                rbTree.Add(node.keys[0], Color.RED);
                rbTree.Add(node.keys[2], Color.RED);
                break;
        }

        Color childColor = (nodeColor == Color.RED) ? Color.BLACK : Color.RED;
        for (int i = 0; i < node.children.Count; i++)
        {
            ConvertNode(node.children[i], ref rbTree, childColor);
        }
    }

//---------------------------------------------------------------------------------------------------------

    // Additional Helper Methods for 2-3-4 Tree

    /*  getChildIndex(Node<T> node, T key)
     * 
     *  Parameters:  node - The node whose children to search through.
     *               key - The key to find the appropriate child index for.
     *  Result:  Returns the child index where the key should be placed or continued to be searched for.
     *           Returns -1 if the node has no children suitable for the key or if the key is already present.
     */
    private int getChildIndex(Node<T> node, T key)
    {
        if (node.children.Count == 0 || node.keys.Contains(key))
        {
            return -1;
        }

        int index = 0;
        for (; index < node.children.Count; index++)
        {
            if (index == node.keys.Count ||
                key.CompareTo(node.keys[index]) < 0)
            {
                break;
            }
        }

        return index;
    }

    /*  SplitNode(Node<T> node, int index)
     * 
     *  Parameters:  node - The parent node containing the child to be split.
     *               index - The index of the child within the parent node's children list that needs to be split.
     *  Result: Splits the child node at the given index, redistributing keys and possibly children between two nodes, and updates the parent node accordingly.  
     */
    private void SplitNode(Node<T> node, int index)
    {
        Node<T> childNodeToSplit = node.children[index];
        if (childNodeToSplit == null)
        {
            Console.WriteLine("Error => child node to split is null");
            return;
        }
        if (childNodeToSplit.keys.Count == this.maxKeysPerNode)
        {
            int middleIndex = this.maxKeysPerNode / 2;

            T middleKey = childNodeToSplit.keys[middleIndex];

            Node<T> newBigChild = new Node<T>();

            for (int keyIndex = this.maxKeysPerNode - 1; keyIndex > middleIndex; keyIndex--)
            {
                newBigChild.keys.Insert(0, childNodeToSplit.keys[keyIndex]);
                childNodeToSplit.keys.RemoveAt(keyIndex);
            }

            if (!childNodeToSplit.isLeafNode())
            {
                for (int childIndex = this.maxChildrenPerNode - 1; childIndex > middleIndex; childIndex--)
                {
                    newBigChild.children.Insert(0, childNodeToSplit.children[childIndex]);
                    childNodeToSplit.children.RemoveAt(childIndex);
                }
            }

            childNodeToSplit.keys.RemoveAt(middleIndex);

            node.keys.Insert(index, middleKey);

            node.children.Insert(index + 1, newBigChild);
        }

    }

    /*  Find(T key)
     * 
     *  Parameter:  key - The key to search for in the tree.
     *  Result:  Returns true if the key is found in the tree, false otherwise.
     */
    public bool Find(T key)
    {
        return Find(this.root, key);
    }

    /*  Find(Node<T> node, T key)
     * 
     *  Parameters: node - The node to start the search from.
     *              key - The key to search for.
     *  Result: Returns true if the key is found within the subtree starting at node, false otherwise.
     */
    private bool Find(Node<T> node, T key)
    {
        if (node == null)
            return false;

        int index = node.keys.BinarySearch(key);

        if (index >= 0)
        {
            return true;
        }
        else
        {
            index = ~index;

            if (node.isLeafNode())
            {
                return false;
            }
            else
            {
                return Find(node.children[index], key);
            }
        }

    }

    /*  ensureTValues(Node<T> parentNode, int childIndex)
     * 
     *  Parameters: parentNode - The parent node of the child node that might need adjustment.
     *              childIndex - The index of the child in the parentNode's child array.
     *  Result: Ensures that the tree maintains the 2-3-4 tree properties after deletion by potentially merging or borrowing nodes.
     *          Returns true if adjustments were made that might affect parent nodes, false otherwise.
     */
    private bool ensureTValues(Node<T> parentNode, int childIndex)
    {
        bool mergeResult;

        if (parentNode == null || childIndex >= parentNode.children.Count)
        {
            Console.WriteLine("ensureTValues: we got bad input!");
            return false;
        }

        if (parentNode.children[childIndex].keys.Count > this.minKeysPerNode)
        {
            return true;
        }

        if (childIndex > 0 &&
            parentNode.children[childIndex - 1].keys.Count > this.minKeysPerNode)
        {
            return this.borrowFromLeftSibling(parentNode, childIndex - 1);

        }

        else if (childIndex < parentNode.children.Count - 1 &&
            parentNode.children[childIndex + 1].keys.Count > this.minKeysPerNode)
        {
            return this.boorowFromRightSibling(parentNode, childIndex);
        }

        else if (childIndex > 0)
        {
            mergeResult = this.MergeNodes(parentNode, childIndex - 1);
        }
        else
        {
            mergeResult = this.MergeNodes(parentNode, childIndex);
        }

        if (mergeResult && parentNode == root && parentNode.keys.Count == 0)
        {
            this.root = this.root.children[0];
        }
        return mergeResult;

    }

    /*  borrowFromLeftSibling(Node<T> parentNode, int index)
     * 
     *  Parameters:  parentNode - The node whose children are involved in the borrowing.
     *               index - The index of the child that will receive a key from its left sibling.
     *  Result:  Adjusts keys and children between two sibling nodes to maintain tree balance. 
     *           Returns true if borrowing was successful, false otherwise.
     */
    private bool borrowFromLeftSibling(Node<T> parentNode, int index)
    {
        if (parentNode == null || index >= parentNode.children.Count || index == 0)
        {
            Console.WriteLine("boorowFromLeftSibling: bad input!");
            return false;
        }

        Node<T> leftSibling = parentNode.children[index];
        Node<T> rightSibling = parentNode.children[index + 1];

        if (leftSibling.keys.Count < this.minKeysPerNode + 1)
        {
            Console.WriteLine("boorowFromLeftSibling: not enough keys to borrow from!");
            return false;
        }


        rightSibling.keys.Insert(0, parentNode.keys[index]);

        parentNode.keys[index] = leftSibling.keys[leftSibling.keys.Count - 1];

        leftSibling.keys.RemoveAt(leftSibling.keys.Count - 1);

        if (!leftSibling.isLeafNode())
        {
            rightSibling.children.Insert(0, leftSibling.children[leftSibling.children.Count - 1]);

            leftSibling.children.RemoveAt(leftSibling.children.Count - 1);
        }

        return true;
    }

    /*  borrowFromRightSibling(Node<T> parentNode, int index)
     * 
     *  Parameters: parentNode - The node whose children are involved in the borrowing.
     *              index - The index of the child that will borrow a key from its right sibling.
     *  Result: Adjusts keys and children between two sibling nodes to maintain tree balance.
     *          Returns true if borrowing was successful, false otherwise.
     */
    private bool boorowFromRightSibling(Node<T> parentNode, int index)
    {
        if (parentNode == null || index > parentNode.children.Count - 2)
        {
            Console.WriteLine("boorowFromRightSibling: bad input!");
            return false;
        }

        Node<T> leftSibling = parentNode.children[index];
        Node<T> rightSibling = parentNode.children[index + 1];

        if (rightSibling.keys.Count < this.minKeysPerNode + 1)
        {
            Console.WriteLine("boorowFromRightSibling: not enough keys to borrow from!");
            return false;
        }

        leftSibling.keys.Add(parentNode.keys[index]);

        parentNode.keys[index] = rightSibling.keys[0];

        rightSibling.keys.RemoveAt(0);

        if (!rightSibling.isLeafNode())
        {
            leftSibling.children.Add(rightSibling.children[0]);

            rightSibling.children.RemoveAt(0);
        }
        return true;
    }

    /*  MergeNodes(Node<T> parentNode, int index)
     * 
     *  Parameters: parentNode - The node whose children are to be merged.
     *              index - The index of the first of the two children to merge.
     *  Result: Merges two child nodes into one to maintain tree balance after deletions.
     *          Returns true if the merge was successful, false otherwise.
     */
    private bool MergeNodes(Node<T> parentNode, int index)
    {
        if (parentNode.isLeafNode() || index >= parentNode.keys.Count)
        {
            Console.WriteLine("MergeNodes: bad child index!");
            return false;
        }

        if (parentNode.children[index].keys.Count != this.minKeysPerNode ||
            parentNode.children[index + 1].keys.Count != this.minKeysPerNode)
        {
            Console.WriteLine("MergeNodes: left and rigth are too big!!");
            return false;
        }

        int middleKeyIndex = this.maxKeysPerNode / 2;
        for (int keyIndex = middleKeyIndex - 1; keyIndex >= 0; keyIndex--)
        {
            parentNode.children[index + 1].keys.Insert(0, parentNode.children[index].keys[keyIndex]);
        }

        if (!parentNode.children[index].isLeafNode())
        {
            int middleChildIndex = this.maxChildrenPerNode / 2;
            for (int childIndex = middleChildIndex - 1; childIndex >= 0; childIndex--)
            {
                parentNode.children[index + 1].children.Insert(0, parentNode.children[index].children[childIndex]);
            }
        }

        parentNode.children[index + 1].keys.Insert(middleKeyIndex, parentNode.keys[index]);

        parentNode.keys.RemoveAt(index);
        parentNode.children.RemoveAt(index);


        return true;
    }
}

public class Assignment3_Part3
{
    static void Main(string[] args)
    {
        // Create a 2-3-4 tree and insert some items.
        TwoThreeFourTree<int> tree234 = new TwoThreeFourTree<int>();
        int[] valuesToInsert = { 50, 40, 60, 30, 45, 55, 70, 20, 35, 48, 58, 65, 80, 38 };
        foreach (var value in valuesToInsert)
        {
            tree234.Insert(value);
        }

        // Print the 2-3-4 tree before conversion.
        Console.WriteLine("2-3-4 Tree before conversion:");
        tree234.Print();


        // Convert the 2-3-4 tree to a Red-Black tree.
        var rbTree = tree234.Convert();

        // Print the Red-Black tree after conversion.
        Console.WriteLine("\nRed-Black Tree after conversion:");
        rbTree.Print();

        /*      // Test - Trying to insert an item already in the tree
                if (tree234.Insert(38))
                    Console.WriteLine("\n Item Inserted");
                else
                    Console.WriteLine("\n Error : Item not Inserted");
        */


        /*      // Test - Delete from 2-3-4 tree and print R-B tree
                if (tree234.Delete(38))
                {
                    Console.WriteLine("\nItem deleted.");

                    Console.WriteLine("\n2-3-4 Tree After delete:");
                    tree234.Print();
                    rbTree = tree234.Convert();
                    Console.WriteLine("\nR-B Tree After delete:");
                    rbTree.Print();
                }

         */
        Console.ReadKey();
    }
}
