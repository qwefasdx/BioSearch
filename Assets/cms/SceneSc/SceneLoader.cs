using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadNextScene(string sceneName)
    {
        Debug.Log("��ư Ŭ����! �� ��ȯ �õ� �� �� " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
