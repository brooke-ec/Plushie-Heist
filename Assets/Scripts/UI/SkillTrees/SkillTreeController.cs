using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary> Used for ONE skill tree </summary>
public class SkillTreeController : MonoBehaviour
{
    [Header("References")]
    public SkillTree skillTree;
    public List<Skill> unlockedSkills = new List<Skill>();

    public void CreateSkillTree(SkillTree skillTree)
    {
        this.skillTree = skillTree;
        GenerateSkillTree();
        DisplaySkillTree();
    }

    public bool IsSkillUnlocked(Skill skill)
    {
        return unlockedSkills.Contains(skill);
    }

    #region Building tree
    [Header("Building tree references")]
    private Dictionary<Skill, Vector2> nodePositions = new Dictionary<Skill, Vector2>();

    public SkillButton nodePrefab;
    public Transform canvasTransform;

    #region Logic representation
    private void GenerateSkillTree()
    {
        Dictionary<Skill, int> layers = GetLayersOfSkills(skillTree.skills);

        List<List<Skill>> layeredNodes = GroupNodesByLayer(layers);

        MinimiseEdgeCrossings(layeredNodes);

        AssignPositions(layeredNodes);
    }

    /// <summary> Places skills in layers based on their requirements. Done through getting depth of each </summary>
    private Dictionary<Skill, int> GetLayersOfSkills(List<Skill> skills)
    {
        Dictionary<Skill, int> layers = new Dictionary<Skill, int>();
        foreach(Skill skill in skills)
        {
            layers[skill] = GetDepth(skill);
        }
        return layers;
    }

    /// <summary> Recursively calls each parent and gets the max depth of any of the parents, +1 </summary>
    private int GetDepth(Skill node)
    {
        if(node.requirements.Count == 0) { return 0; } //none
        return node.requirements.Max(requirement => GetDepth(requirement) + 1);
    }

    /// <summary> Groups nodes by getting a list of the list of nodes in each layer </summary>
    private List<List<Skill>> GroupNodesByLayer(Dictionary<Skill, int> layers)
    {
        List<List<Skill>> layeredNodes = new List<List<Skill>>();
        int maxLayer = layers.Values.Max();

        for(int i=0; i<=maxLayer; i++)
        {
            List<Skill> layer = skillTree.skills.Where(skill => layers[skill] == i).ToList();
            layeredNodes.Add(layer);
        }
        return layeredNodes;
    }

    /// <summary> To order nodes within each layer to minimise edge crossings. This is to help the layout, as it reduces overlapping of edges by keeping related nodes grouped together </summary>
    private void MinimiseEdgeCrossings(List<List<Skill>> layers)
    {
        //Don't need to order first layer because it has no requirements above
        for(int i=1; i<layers.Count; i++)
        {
            //Sort nodes in current layer based on where their requirements appear (in the previous layer)
            layers[i] = layers[i].OrderBy(node =>
            {
                //Get the average index of its dependencies in the previous layer (if it only has one parent, then that is the index)
                return node.requirements.Average(depth => layers[i - 1].IndexOf(depth));
            }).ToList();
        }
    }

    private void AssignPositions(List<List<Skill>> layers)
    {
        nodePositions.Clear();

        // Find the root nodes (nodes without dependencies)
        List<Skill> rootNodes = layers[0];

        float xOffset = 0f; // Track X position dynamically

        // Start recursive placement starting from the root
        foreach (var root in rootNodes)
        {
            PlaceNode(root, ref xOffset, 0);
        }
    }

    private float PlaceNode(Skill node, ref float xOffset, int depth)
    {
        float ySpacing = 150f;
        float xSpacing = 200f;

        // Get all child nodes (nodes that depend on this node)
        List<Skill> children = skillTree.skills.Where(n => n.requirements.Contains(node)).ToList();

        float startX = xOffset; // Store starting X position
        float height = -depth * ySpacing;

        if (children.Count == 0)
        {
            // If no children, just place the node here and move xOffset
            nodePositions[node] = new Vector2(xOffset, height);
            xOffset += xSpacing; // Move xOffset to avoid overlap
        }
        else
        {
            // Recursively place child nodes first
            float childStartX = xOffset;

            foreach (var child in children)
            {
                PlaceNode(child, ref xOffset, depth + 1);
            }

            // Center the parent node between its children
            //float midX = (childStartX + xOffset - xSpacing) / 2f;
            float midX = (nodePositions[children[0]].x + nodePositions[children[children.Count-1]].x)/2f;
            nodePositions[node] = new Vector2(midX, height);
        }

        return nodePositions[node].x;
    }

    #region OLD CENTERING - MIGHT COME BACK

    /*
    /// <summary> Calculates x and y positions for each node based on its layer and index within that layer.
    /// The layer is centered horizontally based on the number of nodes in it
    /// </summary>
    private void AssignPositions(List<List<Skill>> layers)
    {
        //NOT GOOD FOR SCREEN SIZES, ADJUSTMENT NEEDED
        float ySpacing = 150f; //even spacing between layers
        float xSpacing = 200f; //even spacing between nodes within the same layer

        for(int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            //Centering nodes in each layer
            float widthOfLayer = xSpacing * (layers[layerIndex].Count - 1);
            float startX = -widthOfLayer / 2f;

            for(int nodeIndex = 0; nodeIndex < layers[layerIndex].Count; nodeIndex++)
            {
                float xPos = startX + (nodeIndex * xSpacing); //each node is evenly spaced in layer
                float yPos = -layerIndex * ySpacing; //first layer at 0, next layer at -100, etc
                nodePositions[layers[layerIndex][nodeIndex]] = new Vector2(xPos, yPos);
            }
        }
    }*/

    //because of children with multi-parents, as these are adjusted last to avoid null references to parents not adjusted yet
    //private Dictionary<Skill, int> pendingParents = new Dictionary<Skill, int>();

    /*
    /// <summary> Positions each child centered under its parents </summary>
    private void AssignPositions(List<List<Skill>> layers)
    {
        nodePositions.Clear();
        pendingParents.Clear();
        //Find root nodes with no dependencies. Then, recursively place nodes: for each parent, position its children first. Then, if a node has no children, it takes the next available xOffset. If a node has children, it gets positioned at the centre of its children. It also adjusts xOffset dynamically to prevent overlapping

        float xOffset = 0f; //track x position dynamically

        //count how many parents each node has
        foreach(var node in skillTree.skills)
        {
            if(node.requirements.Count > 0)
            {
                pendingParents[node] = node.requirements.Count;
            }
        }

        //Recursive placement
        foreach (var root in layers[0])
        {
            PlaceNode(root, ref xOffset, 0);
        }
    }

    private float PlaceNode(Skill node, ref float xOffset, int depth)
    {
        //Debug.Log("BEGINNING PLACE NODE node " + node.skillName + " with xOffset " + xOffset + " with depth " + depth);
        float ySpacing = 150f; //even spacing between layers
        float xSpacing = 200f; //even spacing between nodes within the same layer

        //NOT: If the node has been placed due to multiple parents, adjust to be the average of their pos instead of re-placing
        if (nodePositions.ContainsKey(node))
        {
            Debug.LogWarning("here");
            return nodePositions[node].x; //if already placed, just return its XPos
        }

        //All the children node that depend on this node
        List<Skill> children = skillTree.skills.Where(n => n.requirements.Contains(node)).ToList();

        float height = -depth * ySpacing;
        float startX = xOffset;

        if(children.Count == 0)
        {
            //Place node here and move xOffset to next space
            nodePositions[node] = new Vector2(xOffset, height);
            xOffset += xSpacing;
            //Debug.Log("Position of " + node.skillName + "is " + nodePositions[node]);
        }
        else
        {
            float childStartX = xOffset;

            List<float> childXPositions = new List<float>();

            //Debug.Log("childStartX " + childStartX);
            foreach (Skill child in children)
            {
                //Only place the child if all its parents have been placed
                if (pendingParents.ContainsKey(child))
                {
                    pendingParents[child]--; //reduce parent count
                    if (pendingParents[child] > 0) continue; //Wait until all parents are placed
                    Debug.LogWarning("All parents placed of " + child.skillName);
                    pendingParents.Remove(child);
                }
                //Debug.Log("CHILD child " + child.skillName + " with xOffset "+ xOffset +" with depth "+(depth+1));
                childXPositions.Add(PlaceNode(child, ref xOffset, depth + 1));
            }

            //Recursively place child nodes first
            
            //foreach(Skill child in children)
            //{
                //PlaceNode(child, ref xOffset, depth + 1);
            //}

            //Then, centre parent node between its children
            float centreXPos;
            if (childXPositions.Count == 0)
            {
                centreXPos = (childStartX + xOffset - xSpacing) / 2f;
            }
            else
            {
                //Not quite working
                centreXPos = (childXPositions.Min() + childXPositions.Max()) / 2f;
            }
            //Used to just be be centreXPos = (childStartX + xOffset - xSpacing) / 2f; but this makes it centered to the whole tree, instead of to the children

            nodePositions[node] = new Vector2(centreXPos, height);
            //Debug.Log("Position of " + node.skillName + "is " + nodePositions[node]);
        }
        return nodePositions[node].x;
    }*/
    #endregion

    #endregion

    #region Visual representation
    private void DisplaySkillTree()
    {
        CentreSkillTree();

        List<SkillButton> skillButtons = new List<SkillButton>();

        foreach(Skill skill in skillTree.skills)
        {
            SkillButton node = Instantiate(nodePrefab, canvasTransform);
            node.transform.localPosition = nodePositions[skill];
            node.skill = skill;
            skillButtons.Add(node);
        }

        //DrawEdges
        DrawEdges();

        //Done after edges are drawn so that colours can be updated
        foreach(SkillButton skillButton in skillButtons)
        {
            skillButton.SetUI(this);
        }
    }

    private void CentreSkillTree()
    {
        if (nodePositions.Count == 0) return;

        //just to assign something for now
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        //get the actual mins and maxs of nodes right now
        foreach(Vector3 position in nodePositions.Values)
        {
            if (position.x < minX) minX = position.x;
            if (position.x > maxX) maxX = position.x;
            if (position.y < minY) minY = position.y;
            if (position.y > maxY) maxY = position.y;
        }

        float treeCentreX = (maxX + minX) / 2f;
        float treeCentreY = (maxY + minY) / 2f;

        SetSkillTreeContainerSize(minX, maxX, minY, maxY);

        RectTransform container = canvasTransform.GetComponent<RectTransform>();
        Vector2 screenCentre = container.rect.center;
        Vector2 offsetToApply = screenCentre - new Vector2(treeCentreX, treeCentreY);

        //Apply to all nodes
        List<Skill> skills = new List<Skill>(nodePositions.Keys);
        foreach(Skill node in skills)
        {
            nodePositions[node] += offsetToApply;
        }
    }

    private void SetSkillTreeContainerSize(float minX, float maxX, float minY, float maxY)
    {
        //Set width and weight of canvas rect transform to the biggest 
        RectTransform container = canvasTransform.parent.GetComponent<RectTransform>();
        float xSize = (maxX - minX) + 100;
        float ySize = (maxY - minY) + 100;
        container.sizeDelta = new Vector2(xSize, ySize);

    }

    public Transform edgeContainer;
    private void DrawEdges()
    {
        // Clear previous edges
        foreach (Transform child in edgeContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var node in nodePositions)
        {
            Skill parentSkill = node.Key;
            Vector2 parentPos = node.Value;

            IEnumerable<Skill> children = skillTree.skills.Where(s => s.requirements.Contains(parentSkill));

            foreach (Skill childSkill in children)
            {
                if (nodePositions.TryGetValue(childSkill, out Vector2 childPos))
                {
                    GameObject edgeObject = new GameObject("Edge " + parentSkill.skillName + " to " + childSkill.skillName);
                    edgeObject.transform.SetParent(edgeContainer, false);

                    EdgeRenderer edgeRenderer = edgeObject.AddComponent<EdgeRenderer>();
                    edgeObject.AddComponent<CanvasRenderer>();
                    edgeRenderer.AddNewPoints(parentPos, childPos, Color.white);
                }
            }
        }
    }
    #endregion

    #endregion

}
