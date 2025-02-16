using System;
using Managers;
using Shared.Data.Tutorials;
using Shared.MessageCore;
using Shared.MessageData.Tutorial;

namespace Messages.Tutorial
{
public class TutorialMessage : IMessage<TutorialMessageData>
{
    static public Action<byte, bool> OnTutorialUpdated;
    static public Action             OnMainUpdated;

    public override string MessageName => MessageNames.TutorialMessage.ToString();

    protected override void Handle(ulong senderClientId, TutorialMessageData data)
    {
        byte id = data.id;

        // is a fake id - is main tutorial
        if (id == 100)
        {
            PlayerManager.TutorialData.MainIncrease();

            // Debug.Log("Main Increased!");
            OnMainUpdated?.Invoke();
        }
        else if (id == 150)
        {
            PlayerManager.TutorialData.MainFinish();

            //Debug.Log("Main Finished (everything skipped)!");
            OnMainUpdated?.Invoke();
        }
        else
        {
            if (!PlayerManager.TutorialData.GetTutorial(id, out TutorialData tutData))
            {
                PlayerManager.TutorialData.AddTutorial(id, true);
                OnTutorialUpdated?.Invoke(id, true);
                return;
            }

            if (tutData.Completed)
            {
                tutData.TutorialReset();
                OnTutorialUpdated?.Invoke(id, tutData.Completed);
                return;
            }

            tutData.TutorialCompleted();
            OnTutorialUpdated?.Invoke(id, tutData.Completed);
        }
    }
}
}