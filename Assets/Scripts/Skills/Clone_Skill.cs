using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{

    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool createCloneOnCounterAttack;
    [Header("Duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal Instead Of Clone")]
    public bool crystalInsteadOfClone;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_clonePosition, cloneDuration,canAttack,_offset,FindClosestEnemy(_clonePosition),canDuplicateClone,chanceToDuplicate);
    }

    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }
    public void CreateCloneOnDashOver()
    {
        if (createCloneOnDashOver)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }
    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
       if (createCloneOnCounterAttack)
       {
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(player.facingDir * 2, 0)));
       }
    }

    private IEnumerator CreateCloneWithDelay(Transform _enemyTransform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_enemyTransform, _offset);
    }
}
