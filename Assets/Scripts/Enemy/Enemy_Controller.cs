using UnityEngine;

public class Enemy_Controller : MonoBehaviour, ICharacter
{
    public Animation_Controller Animation_Controller => throw new System.NotImplementedException();

    public Transform ModelTransform => throw new System.NotImplementedException();

    public void AddBuff(BuffConfig buffConfig, int layer)
    {
    }

    public void BeHit(AttackData attackData)
    {
        Debug.Log(gameObject.name + $": �ұ�������. �˺���{attackData.attackValue}. ");
    }

    public void ChangeToIdleState()
    {
    }

    public float GetAttackValue(SkillAttackDetectionEvent detectionEvent)
    {
        return 0;
    }

    public void OnSkillMove(Vector3 deltaPosition)
    {
    }


    public void OnSkillRotate(Quaternion deltaRotation)
    {
    }

    public void OnSkillRotate()
    {
    }
}
