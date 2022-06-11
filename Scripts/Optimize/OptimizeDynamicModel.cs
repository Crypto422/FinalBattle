﻿using UnityEngine;
using System.Collections.Generic;
using GameDefine;

//optimizedynamicmodel class는 charactor들의 보임상태 즉 건물에 가리웟을때등 각종경우에 목숨띠와 객체의 active를 조종한다.
//optimizedynamicmodel class는 모든 charactor에 심어진다.
//entitymanager에 의하여 심어진다.
public class OptimizeDynamicModel : MonoBehaviour
{
    //保存碰撞体
    Collider collider = null;
    //保存Animation组件
    Animation animation = null;
    //保存Entity组件
    Entity entity = null;

    //包围盒子
    Bounds bounds;
    //是否可见
    public bool mVisible = true;
    //Entity脚本 暂시不处理
    //Entity entityScript = null;

    //子GameObject
    List<GameObject> childObjs = new List<GameObject>();


    // Use this for initialization
    void Start()
    {
        collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("Optimize dynamic model should have collider!");
        }

        animation = GetComponent<Animation>();
        if (animation == null)
        {
            Debug.LogError(gameObject.name +" Optimize dynamic model should have animation!");
        }

        entity = GetComponent<Entity>();
        if (entity == null)
        {
            Debug.LogError(gameObject.name + " Optimize dynamic model should have entity!");
        }

        



        //获取子object        
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform ts = gameObject.transform.GetChild(i);
            GameObject psObj = ts.gameObject;

            childObjs.Add(psObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMethod.GetMainCamera.useOptimizeDynamicModel)
            return;

        if (collider == null)
            return;

        Plane[] planes = GameMethod.GetMainCamera.planes;
        if (planes == null)
            return;

        //获取碰撞体的包围盒子
        bounds = collider.bounds;

        //稍稍扩大
        bounds.Expand(2.0f);

        bool isVisible = GameMethod.GetMainCamera.IsInFrustum(planes, bounds);//Frustum건축부재의 단편
        if (isVisible != mVisible)
        {
            mVisible = isVisible;
            if (mVisible)   
            {
                setActive(true);
            }
            else
            {
                setActive(false);
            }
        }
    }

    void setActive(bool flag)
    {
        collider.enabled = flag;
        animation.enabled = flag;

        ///entityScript.enabled = flag;
        foreach (GameObject obj in childObjs)
        {
            obj.SetActive(flag);
        }

        if (entity.SyncEntity != null && entity.SyncEntity.xueTiaoObj != null)
        {
            entity.SyncEntity.xueTiaoObj.SetActive(flag);
        }

    }
}
