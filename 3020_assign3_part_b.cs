//Names: Khushi Chauhan; Anubhav Mehandru, Farhana Zahan
//Assignment 3 3020 Part b
//To re-implement the binomial heaps in class to create a lazy binomial heap
//References:   https://web.stanford.edu/class/archive/cs/cs166/cs166.1146/lectures/06/Small06.pdf
//              https://www.geeksforgeeks.org/fibonacci-heap-set-1-introduction/?ref=lbp
//              https://blog.csdn.net/weixin_38475220/article/details/106751706
//              http://algosaur.us/binomial-heaps/
//              Program provided in lecture notes

//libraries
using System;
using System.Collections.Generic;

//node class
public class BinomialNode<T>
{
    public T Item { get; set; } 
    public int Degree { get; set; }
    public BinomialNode<T>? LeftMostChild { get; set; }
    public BinomialNode<T>? RightSibling { get; set; }

    //constructor
    public BinomialNode(T item)
    {
        Item = item;
        Degree = 0;
        LeftMostChild = null;
        RightSibling = null;
    }
}

//lazy binomial heap implementation
public class LazyBinomialHeap<T> : IContainer<T> where T : IComparable
{
    private BinomialNode<T>?[] B; //array B to store roots of binomial trees
    private BinomialNode<T>? high;//node with highest priority
    private int size; //no. of items in the heap

    //constructor
    public LazyBinomialHeap()
    {
        B = new BinomialNode<T>?[32];
        size = 0;
        high = null;
    }

    //Method to add an item to the heap
    public void Add(T item)
    {
        //create a node with a degree 0
        BinomialNode<T>? newNode = new BinomialNode<T>(item);
        newNode.Degree = 0;

        //add the new node to B[0] if there already an item in B[0] using link, else assign it directly 
        if (B[0] != null) 
        {
            B[0] = BinomialLink(B[0], newNode);
        }
        else
        {
            B[0] = newNode; // Otherwise, assign directly
        }

        //increment size of B[0]
        size++;

        //update high when needed
        if (high == null || item.CompareTo(high.Item) > 0)
        {
            high = newNode;
        }
    }

    //Binomial Link to make the leftmost child of root(as in lecture notes)
    private static BinomialNode<T> ?BinomialLink(BinomialNode<T>? root, BinomialNode<T>? child)
    {
        if (root == null)
        {
            return child;
        }

        if (root.LeftMostChild == null)
        {
            root.LeftMostChild = child;
        }
        else
        {
            BinomialNode<T>? temp = root.LeftMostChild;
            while (temp.RightSibling != null)
            {
                temp = temp.RightSibling;
            }
            temp.RightSibling = child;
        }

        root.Degree++;
        return root;
    }

    //get the item with the heighest priority
    public T Front()
    {
        if (high == null)
        {
            throw new InvalidOperationException("Heap is empty");
        }

        return high.Item;
    }


    //Remove method
    public void Remove()
    {
        if (high == null)
        {
            throw new InvalidOperationException("Heap is empty");
        }

        // Find and remove the node with the highest priority
        BinomialNode<T>? prev = null;
        BinomialNode<T>? current = high;
        BinomialNode<T>? maxPrev = null;
        BinomialNode<T>? maxNode = high;

        while (current != null)
        {
            if (current.Item.CompareTo(maxNode.Item) > 0)
            {
                maxPrev = prev;
                maxNode = current;
            }
            prev = current;
            current = current.RightSibling;
        }

        // Update high pointer to point to the next highest priority node (if any)
        if (maxPrev != null)
        {
            maxPrev.RightSibling = maxNode.RightSibling;
        }
        else
        {
            // If maxNode was the first element, search its RightSibling for the next highest priority
            BinomialNode<T>? nextHighest = maxNode.RightSibling;
            if (nextHighest != null)
            {
                current = nextHighest;
                maxPrev = null; // Reset maxPrev for searching siblings of nextHighest
                while (current.RightSibling != null)
                {
                    if (current.RightSibling.Item.CompareTo(current.Item) > 0)
                    {
                        maxPrev = current;
                        current = current.RightSibling;
                    }
                    else
                    {
                        current = current.RightSibling;
                    }
                }
                high = current; // Update high to the highest priority among maxNode.RightSibling siblings
            }
            else
            {
                high = null; // If no RightSibling, the heap becomes empty
            }
        }

        // Detach the children of the removed node and store them in appropriate root lists
        int k = maxNode.Degree;
        BinomialNode<T>? child = maxNode.LeftMostChild;
        BinomialNode<T>? reverse = null;
        while (child != null) // Ensure child is initialized correctly
        {
            BinomialNode<T>? next = child.RightSibling;
            child.RightSibling = reverse;
            reverse = child;
            B[k-- - 1] = child; // Store children in corresponding root lists
            child = next;
        }

        size--;
        Coalesce();
    }

    //Coalesce method
    private void Coalesce()
    {
        //create a list that stores the roots of the new binomial trees
        List<BinomialNode<T>?> newB = new List<BinomialNode<T>?>(32); 
        for (int i = 0; i < 32; i++)
        {
            newB.Add(null);//initilize the list with null values
        }

        //for the current binomial trees in B
        for (int i = 0; i < B.Length; i++)
        {
            BinomialNode<T>? current = B[i];

            //each node is visted in the current tree
            while (current != null)
            {
                BinomialNode<T>? next = current.RightSibling;

                //nodes with same degree are coalesced till no other nodes with that degree exists
                while (newB[current.Degree] != null)
                {
                    BinomialNode<T>? other = newB[current.Degree];
                    if (current.Item.CompareTo(other.Item) > 0)
                    {
                        BinomialNode<T>? temp = current;
                        current = other;
                        other = temp;
                    }

                    //remove the node
                    newB[current.Degree] = null;

                    //merge rhe remain nodes
                    current = BinomialLink(current, other);
                }

                //store the nodes in newB and marks next as the curr
                newB[current.Degree] = current;
                current = next;
            }
        }

        // Update B to the new structure
        B = newB.ToArray();
    }

    //To check if heap if empty
    public bool Empty()
    {
        return size == 0;
    }

    //Method to print the heap
    public void Print()
    {
        if (Empty())
        {
            Console.WriteLine("Lazy binomial heap is empty.");
            return;
        }

        Console.WriteLine("Items in the lazy binomial heap:");
        if (high != null)
        {
            Console.WriteLine($"Item with highest priority: {high.Item}");
        }

        //proints each tree with their items in the heap
        for (int i = 0; i < B.Length; i++)
        {
            if (B[i] != null)
            {
                Console.WriteLine($"Degree {i} trees:");
                PrintTree(B[i]);
                Console.WriteLine();
            }
        }   
    }

    //Provate print method called by Print recursively to print all elements inside the tree with a given degree using BFS
    private void PrintTree(BinomialNode<T> root)
    {
        if (root == null)
        {
            return;
        }
        
        Queue<BinomialNode<T>?> queue = new Queue<BinomialNode<T>?>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            int levelNodes = queue.Count;

            for (int i = 0; i < levelNodes; i++)
            {
                var current = queue.Dequeue();
                Console.WriteLine($"Item: {current.Item}");

                var child = current.LeftMostChild;
                while (child != null)
                {
                    queue.Enqueue(child);
                    child = child.RightSibling;
                }
            }
        }
    }
}

//--------------------------------------------------------------------------------------

// Used by class BinomailHeap<T>
// Implements IComparable and overrides ToString (from Object)
public interface IContainer<T>
{
    bool Empty();
}

public class PriorityClass : IComparable
{
    private int priorityValue;
    private char letter;

    public PriorityClass(int priority, char letter)
    {
        this.letter = letter;
        priorityValue = priority;
    }

    public int CompareTo(object? obj)
    {
        PriorityClass? other = (PriorityClass)obj;   // Explicit cast
        return priorityValue - other.priorityValue;  // High values have higher priority
    }

    public override string ToString()
    {
        return $"Item: {letter}, Priority: {priorityValue}";
    }
}

//--------------------------------------------------------------------------------------

//Main Method
public class Assignment3
{
    public static void Main(string[] args)
    {
        LazyBinomialHeap<PriorityClass> BH = new LazyBinomialHeap<PriorityClass>();

        //Test Case 1: show that this is the heap is empty as nothing is added yet
        Console.WriteLine("Heap before adding an item:");
        BH.Print();
        Console.WriteLine();

        /*Test Case 2: try to remove when the heap is empty
        Console.WriteLine("\nRemoving when the heap is empty:");
        BH.Remove();*/

        //Test Case 3 and 4: print the heap after adding items along with the item with highest priority
        BH.Add(new PriorityClass(20, 'a'));
        BH.Add(new PriorityClass(30, 'b'));
        BH.Add(new PriorityClass(25, 'c'));
        BH.Add(new PriorityClass(40, 'd'));
        BH.Add(new PriorityClass(35, 'e'));
        BH.Print();

        //Test Case 5 & 6: remove an item followed by a coalesce
        Console.WriteLine("\nRemoving the highest priority item:");
        BH.Remove();  
        Console.WriteLine("\nHeap after coalesce:");
        BH.Print();
        
        //Test Case 7: add items after a remove, to ensure they are added to B[0]
        BH.Add(new PriorityClass(33, 'f'));
        BH.Add(new PriorityClass(47, 'g'));
        BH.Add(new PriorityClass(45, 'h'));
        Console.WriteLine("\nHeap after adding more items");
        BH.Print();

        //Test Case 8: try to insert item that already exists
        Console.WriteLine("\nHeap after adding duplicate items");
        BH.Add(new PriorityClass(33, 'f'));
        BH.Print();

        //Test Case 9: retest the remove and coalesce method after another add
        Console.WriteLine("\nRemoving the highest priority item:");
        BH.Remove();  // Call the Remove method
        Console.WriteLine("\nHeap after coalesce");
        BH.Print();

        Console.ReadLine();
    }

}





