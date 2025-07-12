using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_LoadingSceneManager : MonoBehaviour
{
    static string nextScene;

    [SerializeField] Image image_LoadingGauge;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                image_LoadingGauge.fillAmount = op.progress;
            }
            else
            {
                // 0.9 ~ 1.0 구간 로딩바 진행
                timer += Time.unscaledDeltaTime;
                image_LoadingGauge.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if (image_LoadingGauge.fillAmount >= 1f)
                {
                    yield return new WaitForSecondsRealtime(0.5f);

                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
