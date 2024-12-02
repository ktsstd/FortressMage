using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillAnimationEvent
{
    void ReSpawn();

    void OnUseSkillA();

    void OnUseSkillS();
    public virtual void OnUseStoneWall() { }
}
