// Thanks to Dual Core studio.

using System.Collections;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject mLoadingScreenObject;
    public Slider mSlider;
    public Text mText;
    public int mSceneToLoad;
    public Image mImage;
    public Animator mAnimator;

    private AsyncOperation mAsyncOperation;

    public void Start()
    {
        StartCoroutine(LoadingScreen(mSceneToLoad));
    }

    IEnumerator LoadingScreen(int sceneToLoad)
    {
        mAsyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        mAsyncOperation.allowSceneActivation = false;

        while (!mAsyncOperation.isDone)
        {
            mText.text = "Loading " + mAsyncOperation.progress.ToString() + "%";
            mSlider.value = mAsyncOperation.progress;

            if (mAsyncOperation.progress >= 0.9f)
            {
                mText.text = "Press <Space> to start the observatory...";

                if (OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetKeyDown(KeyCode.Space))
                {
                    StartCoroutine(Fade());
                }
            }

            yield return null;
        }
    }

    IEnumerator Fade()
    {
        mAnimator.SetBool("Fading", true);
        yield return new WaitUntil(() => mImage.color.a == 1);
        mAsyncOperation.allowSceneActivation = true;
    }
}