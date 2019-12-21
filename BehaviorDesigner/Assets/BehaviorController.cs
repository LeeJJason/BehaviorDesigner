/******************************************************************************
 * 【本类功能概述】                                 					      *
 *  版权所有（C）2015-20XX，大拇哥                                            *
 *  保留所有权利。                                                            *
 ******************************************************************************
 *  作者 : <LIJIJIAN>
 *  版本 : 
 *  创建时间: 
 *  文件描述: 
 *****************************************************************************/

using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorController : MonoBehaviour {

    public BehaviorTree behaviorTree;
    public ExternalBehaviorTree ExternalBehaviorTree;


    [ContextMenu("StartBehaviorTree")]
    public void StartBehaviorTree()
    {
        behaviorTree.ExternalBehavior = ExternalBehaviorTree;
        behaviorTree.EnableBehavior();
    }

    [ContextMenu("GetShare")]
    public void GetShare()
    {
        SharedVariable sharedVariable = behaviorTree.GetVariable("shader");
    }
}
