// Copyright(C) 2020 MAGNESIA (ICE-CSIC)
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.If not, see<https://www.gnu.org/licenses/>.
//
// Contributors to this file:
//  * Alberto Garcia Garcia (garciagarcia @ ice.csic.es)
//  * Thanks to Dual Core studio.
//

using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Component to handle the load and transition to another scene.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public GameObject mLoadingScreenObject;
    public Slider mSlider;
    public Text mText;
    public int mSceneToLoad;
    public Image mImage;
    public Animator mAnimator;

    private AsyncOperation mAsyncOperation;

    /// <summary>
    /// Start is called before the first frame update.
    /// 
    /// Immediately start loading the next scene asynchronously if this
    /// component is present in the scene. This is intended to happen
    /// in the introductory screen.
    /// </summary>
    public void Start()
    {
        StartCoroutine(LoadScene(mSceneToLoad));
    }

    /// <summary>
    /// Load a certain scene.
    /// 
    /// Loads a given scene asynchronously while updating a progress bar/text.
    /// Once the scene is loaded, it does not automatically transition to it
    /// but rather waits on user input (Headset "A" or Keyboard "Space") and
    /// then triggers this scene to fade out onto the next one.
    /// </summary>
    /// <param name="sceneToLoad">Number of the scene to load.</param>
    /// <returns>Yield until next frame or finish.</returns>
    IEnumerator LoadScene(int sceneToLoad)
    {
        // Start the required scene loading in an async manner and do not allow
        // its automatic activation once loaded.
        mAsyncOperation = SceneManager.LoadSceneAsync(
            sceneToLoad,
            LoadSceneMode.Single
        );
        mAsyncOperation.allowSceneActivation = false;

        // Update the progress text and slider while the scene is still loading.
        while (!mAsyncOperation.isDone)
        {
            mText.text = "Loading " + mAsyncOperation.progress.ToString() + "%";
            mSlider.value = mAsyncOperation.progress;

            // If the automatic scene activation is disabled, the async loader
            // will be finished once its progress reachs 90%. Then we wait for
            // the user's input to fade out this scene.
            if (mAsyncOperation.progress >= 0.9f)
            {
                mText.text = "Press <Space>/<A> to start the observatory...";

                if (OVRInput.GetDown(OVRInput.RawButton.A) ||
                    Input.GetKeyDown(KeyCode.Space)
                )
                {
                    StartCoroutine(Fade());
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// Fade out the scene.
    /// 
    /// Trigger the animation to fade out this scene by leveraging the
    /// animator's corresponding flag. Then wait until the fading transition
    /// has been completed and activate the next scene.
    /// </summary>
    /// <returns>Yield until next frame or finish.</returns>
    IEnumerator Fade()
    {
        mAnimator.SetBool("Fading", true);
        yield return new WaitUntil(() => mImage.color.a == 1);
        mAsyncOperation.allowSceneActivation = true;
    }
}