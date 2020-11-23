using UnityEngine;

/* Author: Christina Pilip
 * Usage: Returns whether an object with a Render component is visible from a camera.
 */
public static class RendererExtensions
{
    public static bool isVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
