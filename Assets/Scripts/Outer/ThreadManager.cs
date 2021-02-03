using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour {
    static readonly List<Action> executeOnMainThread = new List<Action>();
    static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
    static bool actionToExecuteOnMainThread = false;

    void Update()
    {
        UpdateMain();
    }

    public static void ExecuteOnMainThread(Action action)
    {
        if(action == null)
        {
            Debug.Log("No action found!");
            return;
        }

        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(action);
            actionToExecuteOnMainThread = true;
        }
    }

    public static void UpdateMain()
    {
        if (!actionToExecuteOnMainThread) return;

        executeCopiedOnMainThread.Clear();

        lock (executeOnMainThread)
        {
            executeCopiedOnMainThread.AddRange(executeOnMainThread);
            executeOnMainThread.Clear();
            actionToExecuteOnMainThread = false;
        }

        for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
        {
            executeCopiedOnMainThread[i]();
        }
    }
}
