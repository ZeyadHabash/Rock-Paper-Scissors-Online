using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{

    private void Update()
    {
        if (!isLocalPlayer) return;

        // do stuff here
    }

}