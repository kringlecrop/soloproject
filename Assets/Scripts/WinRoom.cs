using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinRoom : MonoBehaviour
{
    public GameObject player;

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.transform.position = new Vector3(-133.226f, 0.22f, 2.174f);
            
        }
    }
    


}
