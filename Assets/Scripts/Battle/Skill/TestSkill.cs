using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : MonoBehaviour
{
    public Skill_Player skill_Player;
    public SkillClip skillConfig;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //skill_Player.PlaySkill(skillConfig);
        }
    }
}
