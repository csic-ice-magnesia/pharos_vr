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

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to draw a celestial sphere.
/// </summary>
public class CelestialSphere : MonoBehaviour
{
    // Material to render the parallel lines.
    public Material mParallelMaterial;
    // Material to render the meridian lines.
    public Material mMeridianMaterial;
    // Material to render the celestial equator.
    public Material mEquatorMaterial;
    // Declination values for the parallels.
    public float[] mParallels = { 
        -90.0f, -80.0f, -70.0f,
        -60.0f, -50.0f, -40.0f,
        -30.0f, -20.0f, -10.0f, 
        10.0f, 20.0f, 30.0f,
        40.0f, 50.0f, 60.0f,
        70.0f, 80.0f, 90.0f 
    };
    // Right ascension values for the meridians.
    public float[] mMeridians = {
        -90.0f, -80.0f, -70.0f,
        -60.0f, -50.0f, -40.0f,
        -30.0f, -20.0f, -10.0f,
        0.0f, 10.0f, 20.0f,
        30.0f, 40.0f, 50.0f,
        60.0f, 70.0f, 80.0f,
        90.0f
    };
    // Radius for the celestial sphere.
    public float mSphereRadius = 1024.0f;
    // Width for the parallels/meridians lines.
    public float mLineWidth = 0.5f;
    // Opacity for the celestial sphere.
    public float mOpacity = 0.010f;

    /// <summary>
    /// Start is called before the first frame update.
    /// 
    /// Create parallels and meridians at the specified intervals and add the
    /// necessary line renderers to show them. Also create a line renderer
    /// for the celestial equator.
    /// </summary>
    void Start()
    {
        // No need for updates on the celestial sphere.
        enabled = false;

        // Parallels.
        for (int i = 0; i < mParallels.Length; ++i)
        {
            GameObject go = new GameObject("Parallel_" + i.ToString());
            go.transform.SetParent(this.transform);

            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

            List<Vector3> parallelPoints = ComputeParallel(mParallels[i], mSphereRadius, 64);
            lineRenderer.positionCount = parallelPoints.Count;
            lineRenderer.SetPositions(parallelPoints.ToArray());
            lineRenderer.material = mParallelMaterial;
            lineRenderer.material.color = new Color(0.25f, 0.28f, 0.58f, mOpacity);
            lineRenderer.startWidth = mLineWidth;
            lineRenderer.endWidth = mLineWidth;
        }

        // Meridians.
        for (int i = 0; i < mMeridians.Length; ++i)
        {
            GameObject go = new GameObject("Meridian_" + i.ToString());
            go.transform.SetParent(this.transform);

            List<Vector3> meridianPoints = ComputeMeridian(mMeridians[i], mSphereRadius, 64);

            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
            lineRenderer.positionCount = meridianPoints.Count;
            lineRenderer.SetPositions(meridianPoints.ToArray());
            lineRenderer.material = mMeridianMaterial;
            lineRenderer.material.color = new Color(0.25f, 0.28f, 0.58f, mOpacity);
            lineRenderer.startWidth = mLineWidth;
            lineRenderer.endWidth = mLineWidth;
        }

        // Equator.
        {
            GameObject go = new GameObject("CelestialEquator");
            go.transform.SetParent(this.transform);

            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

            List<Vector3> parallelPoints = ComputeParallel(0.0f, mSphereRadius, 64);

            lineRenderer.positionCount = parallelPoints.Count;
            lineRenderer.SetPositions(parallelPoints.ToArray());
            lineRenderer.material = mParallelMaterial;
            lineRenderer.material.color = new Color(0.24f, 0.50f, 0.57f, mOpacity);
            lineRenderer.startWidth = mLineWidth * 1.25f;
            lineRenderer.endWidth = mLineWidth * 1.25f;
        }
    }

    /// <summary>
    /// Compute the set of 3D points for a parallel.
    /// 
    /// Given a declination and a radius, compute the set of cartesian 3D
    /// points of a parallel by sweeping right ascension angles from 0 to
    /// 360 in a specified number of divisions.
    /// </summary>
    /// <param name="declination">Declination for the parallel [deg].</param>
    /// <param name="radius">Radius for the parallel [kpc].</param>
    /// <param name="divisions">Number of divisions to sweep ascension.</param>
    /// <returns>List of 3D points for the parallel.</returns>
    private List<Vector3> ComputeParallel(
        float declination,
        float radius,
        int divisions)
    {
        List<Vector3> parallelPoints = new List<Vector3>();

        float rightAscensionStep = 360.0f / divisions;
        for (float ra = 0.0f; ra <= 360.0f; ra += rightAscensionStep)
        {
            Vector3 point = CelestialToCartesian(ra, declination, radius);
            parallelPoints.Add(point);
        }

        return parallelPoints;
    }

    /// <summary>
    /// Compute the set of 3D points for a meridian.
    /// 
    /// Given a right ascension and a radius, compute the set of cartesian 3D
    /// points of a meridian by sweeping declination angles from 0 to 360 in
    /// a specified number of divisions.
    /// </summary>
    /// <param name="rightAscension">Right ascension for the meridian [deg].</param>
    /// <param name="radius">Radius for the meridian [kpc].</param>
    /// <param name="divisions">Number of divisions to sweep declination.</param>
    /// <returns>List of 3D points for the meridian.</returns>
    private List<Vector3> ComputeMeridian(
        float rightAscension,
        float radius,
        int divisions)
    {
        List<Vector3> meridianPoints = new List<Vector3>();

        float declinationStep = 360.0f / divisions;
        for (float decl = 0.0f; decl <= 360.0f; decl += declinationStep)
        {
            Vector3 point = CelestialToCartesian(rightAscension, decl, radius);
            meridianPoints.Add(point);
        }

        return meridianPoints;
    }

    /// <summary>
    /// Convert celestial coordinates to 3D cartesian.
    /// </summary>
    /// <param name="rightAscension">Right ascension in [deg].</param>
    /// <param name="declination">Declination in [deg].</param>
    /// <param name="distance">Distance to Earth in [kpc].</param>
    /// <returns>3D point in cartesian coordinates.</returns>
    private Vector3 CelestialToCartesian(
        float rightAscension,
        float declination,
        float distance)
    {
        Vector3 cartesian = Vector3.zero;

        cartesian.x = distance * 
                      Mathf.Cos(declination * Mathf.Deg2Rad) *
                      Mathf.Cos(rightAscension * Mathf.Deg2Rad);

        cartesian.y = distance * 
                      Mathf.Cos(declination * Mathf.Deg2Rad) *
                      Mathf.Sin(rightAscension * Mathf.Deg2Rad);

        cartesian.z = distance * 
                      Mathf.Sin(declination * Mathf.Deg2Rad);

        return cartesian;
    }
}
