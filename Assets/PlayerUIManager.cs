using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;
    // test network, for debugging, delete later once the game is finished -cliff
    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;

    private void Awake()
    {
        // only one instance of this script can exist at a time, if another exists, destroy it -cliff
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (startGameAsClient)
        {
            startGameAsClient = false;
            // shut down network because start as a host during title screen -cliff
            NetworkManager.Singleton.Shutdown();
            // restart network as a client -cliff
            NetworkManager.Singleton.StartClient();
        }
    }
}
