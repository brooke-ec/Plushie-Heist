using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Manages ALL skill trees </summary>
public class SkillTreesManager : MonoBehaviour
{
    [SerializeField] private GameObject skillTreePrefab;
    [SerializeField] private GameObject skillTreeViewPrefab;

    [HideInInspector] public Transform canvasTransform;

    [SerializeField] private List<SkillTreeController> skillTrees = new List<SkillTreeController>();
    [SerializeField] private List<SkillTree> skillTreesReferences = new List<SkillTree>();

    // Start is called before the first frame update
    void Start()
    {
        skillTreeViewTransform = Instantiate(skillTreeViewPrefab, canvasTransform).transform;
        for(int i=0; i<skillTreesReferences.Count; i++)
        {
            skillTrees.Add(Instantiate(skillTreePrefab, skillTreeViewTransform).GetComponent<SkillTreeController>());
            skillTrees[i].CreateSkillTree(skillTreesReferences[i]);
        }
        skillTreeViewTransform.gameObject.SetActive(false);
    }

    private Transform skillTreeViewTransform;
    private bool isSkillTreeViewOpen = false;
    public void OpenSkillTreeView()
    {
        isSkillTreeViewOpen = !isSkillTreeViewOpen;
        skillTreeViewTransform.gameObject.SetActive(isSkillTreeViewOpen);
    }
}
