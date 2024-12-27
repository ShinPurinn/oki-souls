using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSoundFXManager : MonoBehaviour
{
    public static WordSoundFXManager instance;
    
    [Header("Action Sounds")]
    public AudioClip rollSFX;

    private void Awake()
    {
        if (instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
