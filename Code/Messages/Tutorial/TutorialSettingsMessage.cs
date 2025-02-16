using System;
using Managers;
using Shared.MessageCore;
using Shared.MessageData.Tutorial;
using UnityEngine;

namespace Messages.Tutorial
{
public class TutorialSettingsMessage : IMessage<TutorialSettingsMessageData>
{
    static public   Action OnSettingsButtonPressed;
    public override string MessageName => MessageNames.TutorialSettings.ToString();

    protected override void Handle(ulong senderClientId, TutorialSettingsMessageData data)
    {
        bool skip = data.skip;

        // is a fake id - is main tutorial
        if (skip)
        {
            PlayerManager.TutorialData.MainFinish();
            OnSettingsButtonPressed?.Invoke();
            Debug.Log("Main Finished");
        }
        else
        {
            PlayerManager.TutorialData.MainRestart();
            OnSettingsButtonPressed?.Invoke();
            Debug.Log("Main Restarted");
        }
    }
}
}