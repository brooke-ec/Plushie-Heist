using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    [SerializeField] private GuardAI[] guards;
    [SerializeField] private Boss boss;

    private bool fightStarted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&& !fightStarted)
        { 
            fightStarted = true;

            AudioManager.instance.PlayMusic(AudioManager.MusicEnum.bossFight, true);
            boss.StartFight();
        }
    }
}
