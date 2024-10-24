namespace COIS2020.AidenGomes0801606.Assignment3;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // For NotNull attributes
using System.Diagnostics.Contracts;

public sealed class Node<T>
{
    public T Item { get; set; }

    // "internal" = only things within `A3-DataStructures` have access (AKA can access within LinkedList, but not from
    // within Program.cs)
    public Node<T>? Next { get; internal set; }
    public Node<T>? Prev { get; internal set; }


    public Node(T item)
    {
        Item = item;
    }
}


public class LinkedList<T> : IEnumerable<T>
{
    public Node<T>? Head { get; protected set; }
    public Node<T>? Tail { get; protected set; }


    public LinkedList()
    {
        Head = null;
        Tail = null;
    }


    // IEnumerable is done for you:

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); // Call the <T> version

    public IEnumerator<T> GetEnumerator()
    {
        Node<T>? curr = Head;
        while (curr != null)
        {
            yield return curr.Item;
            curr = curr.Next;
        }
    }


    // This getter is done for you:

    /// <summary>
    /// Determines whether or not this list is empty or not.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Head))] // (these "attributes" tell the `?` thingies that Head and Tail are not
    [MemberNotNullWhen(false, nameof(Tail))] // null whenever this getter returns `false`, which stops the `!` warnings)
    public bool IsEmpty
    {
        get
        {
            bool h = Head == null;
            bool t = Tail == null;
            if (h ^ t) // Can't hurt to do a sanity check while we're here.
                throw new Exception("Head and Tail should either both be null or both non-null.");
            return h;
        }
    }


    // --------------------------------------------------------------
    // Put your code down here:
    // --------------------------------------------------------------

    public void AddFront(T item)
    {
        //Add a new node to the front of the list
        Node<T> newNode = new Node<T>(item);
        if (Head == null)
        {
            Head = newNode;
            Tail = newNode;
        }
        else
        {
            newNode.Next = Head;
            Head.Prev = newNode;
            Head = newNode;
        }
    }

    public void AddFront(Node<T> node)
    {
        //Add a node to the front of the list
        if (Head == null)
        {
            Head = node;
            Tail = node;
        }
        else
        {
            node.Next = Head;
            Head.Prev = node;
            Head = node;
        }
    }

    public void AddBack(T item)
    {
        //Add a new node to the back of the list
        Node<T> newNode = new Node<T>(item);
        if (Tail == null)
        {
            Head = newNode;
            Tail = newNode;
        }
        else
        {
            newNode.Prev = Tail;
            Tail.Next = newNode;
            Tail = newNode;
        }
    }

    public void AddBack(Node<T> node)
    {
        //Add a node to the back of the list
        if (Tail == null)
        {
            Head = node;
            Tail = node;
        }
        else
        {
            node.Prev = Tail;
            Tail.Next = node;
            Tail = node;
        }
    }

    public void InsertAfter(Node<T> node, T item)
    {
        //Insert a new node after the given node
        Node<T> newNode = new Node<T>(item);
        newNode.Next = node.Next;
        newNode.Prev = node;
        node.Next = newNode;
        if (newNode.Next != null)
        {
            newNode.Next.Prev = newNode;
        }
        else
        {
            Tail = newNode;
        }
    }

    public void InsertAfter(Node<T> node, Node<T> newNode)
    {
        //Insert a node after the given node
        newNode.Next = node.Next;
        newNode.Prev = node;
        node.Next = newNode;
        if (newNode.Next != null)
        {
            newNode.Next.Prev = newNode;
        }
        else
        {
            Tail = newNode;
        }
    }

    public void InsertBefore(Node<T> node, T item)
    {
        //Insert a new node before the given node
        Node<T> newNode = new Node<T>(item);
        newNode.Prev = node.Prev;
        newNode.Next = node;
        node.Prev = newNode;
        if (newNode.Prev != null)
        {
            newNode.Prev.Next = newNode;
        }
        else
        {
            Head = newNode;
        }
    }

    public void InsertBefore(Node<T> node, Node<T> newNode)
    {
        //Insert a node before the given node
        newNode.Prev = node.Prev;
        newNode.Next = node;
        node.Prev = newNode;
        if (newNode.Prev != null)
        {
            newNode.Prev.Next = newNode;
        }
        else
        {
            Head = newNode;
        }
    }

    public void Remove(Node<T> node)
    {
        //Remove the given node from the list
        if (node.Prev != null)
        {
            node.Prev.Next = node.Next;
        }
        else
        {
            Head = node.Next;
        }
        if (node.Next != null)
        {
            node.Next.Prev = node.Prev;
        }
        else
        {
            Tail = node.Prev;
        }
    }

    public void Remove(T item)
    {
        //Remove the first node with the given item
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        Node<T>? curr = Head;
        while (curr != null)
        {
            if (comparer.Equals(curr.Item!, item))
            {
                Remove(curr);
                return;
            }
            curr = curr.Next;
        }
    }

    public LinkedList<T> SplitAfter(Node<T> node)
    {
        //Split the list from the node and all nodes after it
        LinkedList<T> newList = new LinkedList<T>();
        newList.Head = node.Next;
        newList.Tail = Tail;
        Tail = node;
        node.Next = null;
        return newList;
    }

    public void AppendAll(LinkedList<T> otherList)
    {
        //Remove all nodes from otherList and add them to the end of the current list
        if (otherList.Head != null)
        {
            if (Head == null)
            {
                Head = otherList.Head;
                Tail = otherList.Tail;
            }
            else
            {
                Tail!.Next = otherList.Head;
                otherList.Head.Prev = Tail;
                Tail = otherList.Tail;
            }
            otherList.Head = null;
            otherList.Tail = null;
        }
    }

    public Node<T>? Find(T item)
    {
        //Find the first node with the given item
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        Node<T>? curr = Head;
        while (curr != null)
        {
            if (comparer.Equals(curr.Item!, item))
            {
                return curr;
            }
            curr = curr.Next;
        }
        return null;
    }
}
    