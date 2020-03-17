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

/// <summary>
/// Component to allow flying camera behavior.
/// </summary>
public class Flyer : MonoBehaviour
{
    // Factor to multiply the acceleration from input.
    public float mAccelerationMultiplier = 3.0f;
    // A forward direction to fly away.
    public Transform mDirection;
    // The rigid body component associated with this object.
    private Rigidbody mRigidBody;

    // Acceleration factor for using the keyboard as input.
    const float DISCRETE_ACCELERATION = 4.0f;
    // Factor to multiply the velocity each frame to reduce it.
    const float VELOCITY_MULTIPLIER = 0.98f;

    /// <summary>
    /// Start is called before the first frame update.
    /// 
    /// Fetch the rigid body component of this object.
    /// </summary>
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update every frame.
    /// 
    /// Fetch either the input from the headset's right thumbstick or from the
    /// keyboard W/S keys. Apply such input acceleration with a factor to the
    /// rigid body velocity. Regardless of the input, we multiply the velocity
    /// by a factor to slowly decrease it and brake every frame.
    /// </summary>
    void Update()
    {
        // Fetch headset thumbstick input.
        float axisY = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y;
        float axisX = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x;

        // Fetch keyboard W/S input and override.
        if (Input.GetKeyDown(KeyCode.W))
        {
            axisY = DISCRETE_ACCELERATION;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            axisY = -DISCRETE_ACCELERATION;
        }

        // Apply acceleration to obtain 3D velocity.
        mRigidBody.velocity += mDirection.forward * 
                               mAccelerationMultiplier *
                               axisY +
                               mDirection.right * 
                               mAccelerationMultiplier * 
                               axisX;

        // Apply braking factor to velocity.
        mRigidBody.velocity *= VELOCITY_MULTIPLIER;
    }
}
