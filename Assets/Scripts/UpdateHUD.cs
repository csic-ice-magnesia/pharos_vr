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
//

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component to handle all the HUD information updating and also
/// to activate and deactivate pulsars on look.
/// </summary>
public class UpdateHUD : MonoBehaviour
{
    // A reference to the previously raycasted pulsar so that we can
    // disable it when we are no longer looking at it.
    private GameObject mPreviousHit;

    // The current player to which the VR camera is attached to. We need
    // it to know its current position and speed to calculate distances
    // and velocities to display in the HUD.
    public GameObject mPlayer;
    // The text reference where the velocity counter will be displayed.
    public Text mVelocityText;
    // The text reference where the pulsar name will be displayed.
    public Text mPulsarNameText;
    // The text reference where the pulsar description will be displayed.
    public Text mPulsarDescriptionText;

    public Text mRightAscensionText;
    public Text mDeclinationText;

    /// <summary>
    /// Start is called before the first frame update.
    /// 
    /// At the beginning, we did not hit any pulsar so we have no records
    /// on our previous hit.
    /// </summary>
    void Start()
    {
        mPreviousHit = null;
    }

    /// <summary>
    /// Update every frame.
    /// 
    /// Raycast from camera look to see which object are we hitting. If such
    /// object happens to be a pulsar (it has a pulsar component) then we
    /// will activate it completely (the jet, the companion and the orbit)
    /// and also update the HUD information with its data.
    /// 
    /// When we are no longer looking at the pulsar, we will disable it.
    ///
    /// This is mainly done for effiency reasons (it is really costly to have
    /// a lot of pulsars rendering and orbiting at the same time in the
    /// headset) plus it helps the visualization and enforces discovery.
    /// 
    /// Note: this now has problems if you look at two pulsars simultaneously,
    /// so it would be nice to keep a list of pulsars to disable them all.
    /// </summary>
    void Update()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        // If the ray hits any pulsar, we activate it, update the orbit
        // and display all its data in the HUD.

        if (Physics.Raycast(ray, out RaycastHit hit) &&
            hit.transform.gameObject.GetComponentInChildren<Pulsar>())
        {
            mPreviousHit = hit.transform.gameObject;

            Pulsar p = hit.transform.gameObject.GetComponent<Pulsar>();
            Transform objectHit = hit.transform;

            // Activate pulsar and make it orbit!

            p.Activate();
            p.OrbitStep();

            // Update HUD information.

            mPulsarNameText.text = objectHit.gameObject.name;

            mPulsarDescriptionText.text = "Distance: " + 
                Mathf.Round
                (
                    Vector3.Distance
                    (
                        mPlayer.transform.position,
                        objectHit.position
                    )
                ).ToString() + " [kpc]\n";

            mPulsarDescriptionText.text += "Frequency: " + 
                p.mFrequency.ToString() + " [Hz]\n";

            mPulsarDescriptionText.text += "B surface: " + 
                p.mSurfaceMagneticIntensity.ToString() + " [G]\n";

            mPulsarDescriptionText.text += "Type: " + 
                p.mType.ToString() + "\n";

            mPulsarDescriptionText.text += "Companion Temperature: " +
                p.mCompanionTemperature.ToString() + " [K]";

            mRightAscensionText.text = p.mRightAscension.x + ":" + 
                p.mRightAscension.y + ":" + p.mRightAscension.z;
            mDeclinationText.text = p.mDeclination.x + ":" +
                p.mDeclination.y + ":" + p.mDeclination.z;
        }
        // If the ray no longer hits any pulsar, we disable it and
        // clear all the information in the HUD.
        else
        {
            // Disable the pulsar if we are no longer looking at it.
            if (mPreviousHit != null)
            {
                Pulsar p = mPreviousHit.transform.gameObject.GetComponent<Pulsar>();
                p.Deactivate();
                mPreviousHit = null;
            }

            // Also clear the HUD information about it.
            mPulsarNameText.text = "";
            mPulsarDescriptionText.text = "";
            mRightAscensionText.text = "???";
            mDeclinationText.text = "???";
        }

        // Whatever happens, update the velocity display every frame.
        mVelocityText.text =
            Mathf.Round
            (
                mPlayer.GetComponent<Rigidbody>().velocity.magnitude
            ).ToString() + " [kpc/s]";
    }
}
