using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// ��� ��� �� ����� ������ ������� �� ���� �����
public class GNode
{
    // ������������ ����
    public GNode parent;
    // �� ���������
    public float cost;
    // ��� �������� WorldStates
    public Dictionary<string, int> state;
    public GAction action;

    public GNode(GNode parent, float cost, Dictionary<string, int> allStates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        // ������ ����� ������� �� ����� WorldStates
        this.state = new Dictionary<string, int>(allStates);
        this.action = action;
    }
}

public class GPlanner
{
    /// <summary>
    /// �����, ������� ���������� ������� ����
    /// </summary>
    /// <param name="actions">������ ���� ��������</param>
    /// <param name="goal"> </param>
    /// <param name="states"> </param>
    /// <returns></returns>
    public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates states)
    {
        // ������ ��������� ������ ��������, ������� ����� ������������
        List<GAction> usableActions = new List<GAction>();
        foreach(GAction a in actions)
        {
            // ���� �������� ����� ����������, ��������� ��� � ������
            if(a.IsAchievable())
            {
                usableActions.Add(a);
            }
        }

        List<GNode> leaves = new List<GNode>();
        GNode start = new GNode(null, 0, GWorld.Instance.GetWorld().GetStates(), null);

        // �������� �����, ������� �������� ���� �����
        bool success = BuildGraph(start, leaves, usableActions, goal);

        if(!success)
        {
            Debug.Log("NO PLAN!!!");
            return null;
        }

        // ���� ����� ������� ����
        GNode cheapest = null;
        foreach(GNode leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }
        }

        // ��������� - ������ ��������, ������� �� ���������� � �����
        List<GAction> result = new List<GAction>();
        GNode n = cheapest;
        while (n != null)
        {
            if (n.action != null)
                result.Insert(0, n.action);
            n = n.parent;
        }

        // �� ��� ������ �������� �������. ��� � ����� ����
        Queue<GAction> queue = new Queue<GAction>();
        foreach(GAction a in result)
        {
            queue.Enqueue(a);
        }

        Debug.Log("The Plan is: ");
        foreach(GAction a in queue)
        {
            Debug.Log("Q: " + a.actionName);
        }

        return queue;
    }

    bool BuildGraph(GNode parent, List<GNode> leaves, List<GAction> usuableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;
        foreach(GAction action in usuableActions)
        {
            // ���� ���: ���� �������� ��� ���� ���������� (���������), �� ��� �� ������
            if(action.IsAchievableGiven(parent.state))
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                foreach(KeyValuePair<string, int>eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                        currentState.Add(eff.Key, eff.Value);
                }

                GNode nextNode = new GNode(parent, parent.cost + action.cost, currentState, action);

                if(GoalAchieved(goal, currentState))
                {
                    leaves.Add(nextNode);
                    foundPath = true;
                }
                else
                {
                    List<GAction> subset = ActionSubset(usuableActions, action);
                    bool found = BuildGraph(nextNode, leaves, subset, goal);
                    if (found)
                        foundPath = true;
                }
            }
        }

        return foundPath;
    }

    /// <summary>
    /// ��������, ����� �� ������� �������.
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="state"></param>
    /// <returns>True, ���� �����; False, ���� ������</returns>
    bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach(KeyValuePair<string, int> g in goal)
        {
            if (!state.ContainsKey(g.Key))
                return false;
        }
        return true;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe)
    {
        List<GAction> subset = new List<GAction>();
        foreach(GAction a in actions)
        {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }
}
