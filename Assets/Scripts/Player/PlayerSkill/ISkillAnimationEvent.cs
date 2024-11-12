using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillAnimationEvent
{
    void OnUseSkillA();

    public virtual void OnUseSkillS()
    {
        return;
    }
}
