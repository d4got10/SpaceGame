using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    public class PlayerMessager : NetworkBehaviour
    {
        private void Update()
        {
            if (!isLocalPlayer) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                SendMessageToServer(netIdentity.name);
            }
        }

        [Command]
        private void SendMessageToServer(string message)
        {
            Debug.Log(message);
        }
    }
}
