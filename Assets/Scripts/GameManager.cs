using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour,IInputClickHandler
{
    CustomMessages customMessages;

    public bool IsShow { get; set; }
    // Use this for initialization
    void Start () {

        customMessages = CustomMessages.Instance;
        if (customMessages.MessageHandlers.ContainsKey(CustomMessages.CustomMessageID.TargetClicked) == false)
        {
            customMessages.MessageHandlers.Add(CustomMessages.CustomMessageID.TargetClicked, null);
        }
        if (customMessages.MessageHandlers.ContainsKey(CustomMessages.CustomMessageID.TargetDragged) == false)
        {
            customMessages.MessageHandlers.Add(CustomMessages.CustomMessageID.TargetDragged, null);
        }
        customMessages.MessageHandlers[CustomMessages.CustomMessageID.TargetClicked] += TargetClickedImpl;
        customMessages.MessageHandlers[CustomMessages.CustomMessageID.TargetDragged] += TargetDraggedImpl;
    }

    private void TargetDraggedImpl(NetworkInMessage msg)
    {
        long userID = msg.ReadInt64();

        Vector3 targetPos = CustomMessages.Instance.ReadVector3(msg);

        Quaternion targetRot = CustomMessages.Instance.ReadQuaternion(msg);

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private void AfterClicked()
    {
        foreach (Transform tf in transform)
        {
            DestroyImmediate(tf.gameObject);
        }
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.parent = transform;
        Vector3 position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        go.transform.position = position;

    }
    public void TargetClickLocal()
    {
        AfterClicked();
        customMessages.BroadcastMessage(CustomMessages.CustomMessageID.TargetClicked);
    }
    private void TargetClickedImpl(NetworkInMessage msg)
    {
        AfterClicked();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        TargetClickLocal();
    }

    private void Update()
    {
        CustomMessages.Instance.SendTargetTransform(transform.position, transform.rotation);
    }

}
