//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行시版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GameDefine;
using JT.FWW.Tools;
using BlGame.GameData;
using BlGame.GameEntity;
using BlGame;

//技能受击效果
namespace BlGame.Effect
{
    public class BeAttackEffect : IEffect
    {
        public BeAttackEffect()
        {
            projectID = EffectManager.Instance.GetLocalId();
            mType = IEffect.ESkillEffectType.eET_BeAttack;
        }

        public override void Update()
        {
            if (isDead)
                return;

            base.Update();
        }

        public override void OnLoadComplete()
        {
            //判断enTarget
            Ientity enTarget;
            EntityManager.AllEntitys.TryGetValue(enTargetKey, out enTarget);

            if (enTarget != null && obj != null)
            {
                Transform hitpoit = enTarget.RealEntity.transform.Find("hitpoint");
                if (hitpoit != null)
                {
                    GetTransform().parent = hitpoit;
                    GetTransform().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                }
            }

            if (skillID == 0)
            {
                return;
            }
        }
    }	
}

