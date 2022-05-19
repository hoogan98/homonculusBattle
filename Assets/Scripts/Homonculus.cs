using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homonculus : MonoBehaviour
{
    private Part _brain;
    private Dictionary<Part, List<Part>> _connections;

    public void Start()
    {
        if (_connections == null)
        {
            _connections = new Dictionary<Part, List<Part>>();
        }

    }

    public void RecalculateNeurons()
    {

        Part[] allParts = GetComponentsInChildren<Part>();

        foreach (Part p in allParts)
        {
            p.visited = false;
            p.brainConnected = false;
        }

        RecursiveNeuronRecalculation(new List<Part>() { _brain });
    }

    private void RecursiveNeuronRecalculation(List<Part> incomingParts)
    {
        if (incomingParts.Count == 0)
        {
            return;
        }

        List<Part> outgoingParts = new List<Part>();

        foreach (Part p in incomingParts)
        {
            p.visited = true;
            p.brainConnected = true;

            List<Part> connectedParts;

            if (_connections.TryGetValue(p, out connectedParts))
            {
                foreach (Part p2 in connectedParts)
                {
                    if (!p2.visited)
                    {
                        outgoingParts.Add(p2);
                    }
                }
            }
        }

        RecursiveNeuronRecalculation(outgoingParts);
    }

    public void ReportBrain(Part brain)
    {
        _brain = brain;
    }

    public void ReportConnection(Part p1, Part p2)
    {
        Debug.Log("made connection: " + p1.gameObject.name + " " + p2.gameObject.name);

        if (_connections.ContainsKey(p1))
        {
            _connections[p1].Add(p2);
        }
        else
        {
            _connections.Add(p1, new List<Part> { p2 });
        }

        if (_connections.ContainsKey(p2))
        {
            _connections[p2].Add(p1);
        }
        else
        {
            _connections.Add(p2, new List<Part> { p1 });
        }
    }

    public void InvalidateConnection(Part p1, Part p2)
    {
        _connections[p1].Remove(p2);

        _connections[p2].Remove(p1);
    }

    public void ReportDestroyedPart(Part deadPart)
    {

        if (!_connections.ContainsKey(deadPart))
        {
            return;
        }

        foreach (Part p in _connections[deadPart])
        {
            _connections[p].Remove(deadPart);
        }

        _connections.Remove(deadPart);
    }

    public void ReportDestroyedPartAndRecalc(Part deadPart)
    {
        ReportDestroyedPart(deadPart);

        RecalculateNeurons();
    }
}
