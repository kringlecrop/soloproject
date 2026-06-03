using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    public GameObject resetWall;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RestartGame();
        }
    }
    public void RestartGame()
    {
        Debug.Log("restarted game");
        SceneManager.LoadScene("MainGame");

    }
}
