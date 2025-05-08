using UnityEngine;

[DefaultExecutionOrder(-10)]
public class CharacterModelRandomizer : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController animationController;
    [SerializeField] private CharacterModelList models;

    void Start()
    {
        // Pick model
        Animator animator = Instantiate(models.models[Random.Range(0, models.models.Length)], transform);
        animator.runtimeAnimatorController = animationController;
    }
}
