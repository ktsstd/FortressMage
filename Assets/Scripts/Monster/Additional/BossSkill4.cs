using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class BossSkill4 : MonoBehaviourPunCallbacks
{
    private float alphavalue = 0f;
    private Material bossSkill4obj;
    public Transform[] BossSKill4Obj;
    public GameObject Parentobj;
    private Renderer meshrenderer;
    private bool StartAtk4 = false;

    void Start()
    {
        // GameObject BossSkill4ObjResource = Resources.Load<GameObject>("Additional/Boss_Skill_4");
        // BossSKill4Obj[0] = BossSkill4ObjResource.gameObject.transform.GetChild(0).gameObject;
        BossSKill4Obj = BossSKill4Obj
            .OrderBy(child => child.position.x)
            .ToArray();
        meshrenderer = gameObject.GetComponent<Renderer>();
        StartCoroutine(StartBossSkill4());
    }

    private IEnumerator StartBossSkill4()
    {
        while(alphavalue <= 0.7f)
        {
            if (!StartAtk4)
            {
                alphavalue += 0.02f;
                Color ColorAlpha = meshrenderer.material.color;
                ColorAlpha.a = alphavalue;
                meshrenderer.material.color = ColorAlpha;
                yield return new WaitForSeconds(0.05f);
            }
        }

        if (alphavalue >= 0.7f)
        {
            StartAtk4 = true;
            Color ColorAlpha = meshrenderer.material.color;
            ColorAlpha.a = 0f;
            meshrenderer.material.color = ColorAlpha;
            // GameObject bossObj = GameObject.FindWithTag("Enemy");
            // Boss bossScript = bossObj.GetComponent<Boss>();
            // if (bossScript == null)
            // {
            //     yield break;
            // }
            foreach (Transform SkillObj in BossSKill4Obj)
            {
                // bossScript.GetComponent<AudioSource>().PlayOneShot(bossScript.MonsterAudio[1], bossScript.MonsterAudio[1].length);
                SkillObj.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(1f);
            StartCoroutine(EndBossSkill4());
        }
        yield break;
    }

    private IEnumerator EndBossSkill4()
    {
        foreach (Transform SkillObj in BossSKill4Obj)
        {
            SkillObj.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
        photonView.RPC("Destroythis", RpcTarget.All);
        yield break;
    }

    [PunRPC]
    public void Destroythis()
    {   
        PhotonNetwork.Destroy(Parentobj);
    }
}
