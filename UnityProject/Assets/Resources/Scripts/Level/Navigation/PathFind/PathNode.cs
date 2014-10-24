using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PathNode : IEnumerable<Cell>
{
    public Cell LastStep { get; private set; }
    public PathNode PreviousSteps { get; private set; }
    public double TotalCost { get; private set; }

    private PathNode(Cell lastStep, PathNode previousSteps, double totalCost)
    {
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }

    public PathNode(Cell start) : this(start, null, 0) { }

    public PathNode AddStep(Cell step, double stepCost)
    {
        return new PathNode(step, this, TotalCost + stepCost);
    }

    public IEnumerator<Cell> GetEnumerator()
    {
        for (var p = this; p != null; p = p.PreviousSteps)
            yield return p.LastStep;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

