namespace COIS2020.AidenGomes0801606.Assignment3;

using System.Collections;
using System.Collections.Generic;


public class Queue<T> : IEnumerable<T>
{
    public const int DefaultCapacity = 8;

    private T?[] buffer;
    private int start;
    private int end;
    public int Count { get => (end - start + Capacity) % Capacity;}
    public int Capacity { get => buffer.Length;}
    public bool IsEmpty { get => Count == 0; }


    public Queue() : this(DefaultCapacity)
    { }

    public Queue(int capacity)
    {
        //Constructor for Queue
        buffer = new T?[capacity];
        start = 0;
        end = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return buffer[(start + i) % Capacity]!;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Your code here...

    protected void Grow()
    {
        //Increase the buffer size if the queue is full
        T?[] newBuffer = new T?[Capacity * 2];
        for (int i = 0; i < Count; i++)
            newBuffer[i] = buffer[(start + i) % Capacity];
        buffer = newBuffer;
    }

    public void Enqueue(T item)
    {
        //Add an item to the end of the queue
        if (Count == Capacity)
            Grow();
        buffer[end] = item;
        end = (end + 1) % Capacity;
    }

    public T Dequeue()
    {
        //Remove and return the first item in the queue
        if (IsEmpty)
            throw new InvalidOperationException("Queue is empty");
        T item = buffer[start]!;
        buffer[start] = default;
        start = (start + 1) % Capacity;
        return item;
    }

    public T Peek()
    {
        //Return the first item in the queue without removing it
        if (IsEmpty)
            throw new InvalidOperationException("Queue is empty");
        return buffer[start]!;
    }
}
