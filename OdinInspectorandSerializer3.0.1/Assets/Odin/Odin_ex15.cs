﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Odin_ex15 : MonoBehaviour
{
    [HideLabel]
    [Title("Default message", "You can just provide a default message that is always used")]
    [ValidateInput("MustBeNull", "This field should be null.")]
    public MyScripty DefaultMessage;

    [Space(12), HideLabel]
    [Title("Dynamic message", "Or the validation method can dynamically provide a custom message")]
    [ValidateInput("HasMeshRendererDynamicMessage", "Prefab must have a MeshRenderer component")] //调用HasMeshRendererDynamicMessage方法检测是否有效
    public GameObject DynamicMessage;

    [Space(12), HideLabel]
    [Title("Dynamic message type", "The validation method can also control the type of the message")]
    [ValidateInput("HasMeshRendererDynamicMessageAndType", "Prefab must have a MeshRenderer component")]
    public GameObject DynamicMessageAndType;

    [Space(8), HideLabel]
    [InfoBox("Change GameObject value to update message type", InfoMessageType.None)]
    public InfoMessageType MessageType;

    [Space(12), HideLabel]
    [Title("Dynamic default message", "Use $ to indicate a member string as default message")]
    [ValidateInput("AlwaysFalse", "$Message", InfoMessageType.Warning)]
    public string Message = "Dynamic ValidateInput message";

    private bool AlwaysFalse(string value)
    {
        return false;
    }

    private bool MustBeNull(MyScripty scripty)
    {
        return scripty == null;
    }

    private bool HasMeshRendererDefaultMessage(GameObject gameObject)
    {
        if (gameObject == null) return true;

        return gameObject.GetComponentInChildren<MeshRenderer>() != null;
    }

    private bool HasMeshRendererDynamicMessage(GameObject gameObject, ref string errorMessage)
    {
        if (gameObject == null) return true;

        if (gameObject.GetComponentInChildren<MeshRenderer>() == null)
        {
            // If errorMessage is left as null, the default error message from the attribute will be used
            errorMessage = "\"" + gameObject.name + "\" must have a MeshRenderer component";

            return false;
        }

        return true;
    }

    private bool HasMeshRendererDynamicMessageAndType(GameObject gameObject, ref string errorMessage, ref InfoMessageType? messageType)
    {
        if (gameObject == null) return true;

        if (gameObject.GetComponentInChildren<MeshRenderer>() == null)
        {
            // If errorMessage is left as null, the default error message from the attribute will be used
            errorMessage = "\"" + gameObject.name + "\" should have a MeshRenderer component";

            // If messageType is left as null, the default message type from the attribute will be used
            messageType = this.MessageType;

            return false;
        }

        return true;
    }

    public class MyScripty : ScriptableObject
    {

    }
}
