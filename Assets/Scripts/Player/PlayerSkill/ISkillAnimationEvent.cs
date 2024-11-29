using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillAnimationEvent
{
    void OnUseSkillA();

    void OnUseSkillS();
    public virtual void OnUseStoneWall() { }
}
