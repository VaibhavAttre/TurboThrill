using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int size) {
        items = new T[size];
    }

    public void Add(T item) {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SwimUp(item);
        currentItemCount++;
    }

    public T RemoveFirst() {
        T first = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SwimDown(items[0]);
        return first;
    }
    
    public void UpdateItem(T item) {
        SwimUp(item);
    }

    public int Count {
        get {
            return currentItemCount;
        }
    }

    public bool Contains(T item) {
        return Equals(items[item.HeapIndex], item);
    }

    void SwimDown(T item) {
        while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap (item,items[swapIndex]);
				}
				else {
					return;
				}

			}
			else {
				return;
			}

		}
    }

    void SwimUp(T item) {
        int parentIdx = (item.HeapIndex-1)/2;
        while(true) {
            T parentItem = items[parentIdx];
            if(item.CompareTo(parentItem) > 0) {
                Swap(item, parentItem);
            } else {
                break;
            }
            parentIdx = (item.HeapIndex-1)/2;
        }
    }

    void Swap(T a, T b) {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        int itemAIdx = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = itemAIdx;
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex {
        get;
        set;
    }
}
