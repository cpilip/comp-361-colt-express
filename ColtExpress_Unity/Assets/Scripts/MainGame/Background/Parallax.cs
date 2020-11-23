using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* Author: Christina Pilip
 * Usage: Defines parallax behavior for the background layers.
 */
public class Parallax : MonoBehaviour
{
    public Vector2 speed = new Vector2(2, 2);
    public Vector2 direction = new Vector2(-1, 0);
    public bool isLooping = false;

    private List<SpriteRenderer> backgroundPart;

    void Start()
    {
        // Obtain all sprite renderers from the various background layers
        if(isLooping)
        {
            backgroundPart = new List<SpriteRenderer>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                SpriteRenderer r = child.GetComponent<SpriteRenderer>();

                if (r != null)
                {
                    backgroundPart.Add(r);
                }
            }

            // Order by position
            backgroundPart = backgroundPart.OrderBy(
                t => t.transform.position.x
                ).ToList();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Translate the background layer
        Vector3 movement = new Vector3(
            speed.x * direction.x,
            speed.y * direction.y,
            0);

        movement *= Time.deltaTime;
        transform.Translate(movement);

        if (isLooping)
        {           
            SpriteRenderer firstChild = backgroundPart.FirstOrDefault();
            if (firstChild != null) {
                // Check if current background layer is out of camera view; reset
                if (firstChild.transform.position.x < Camera.main.transform.position.x)
                {
                    if (firstChild.isVisibleFrom(Camera.main) == false)
                    {                 
                        SpriteRenderer lastChild = backgroundPart.LastOrDefault();

                        Vector3 lastPosition = lastChild.transform.position;
                        Vector3 lastSize = (lastChild.bounds.max - lastChild.bounds.min);

                        firstChild.transform.position = new Vector3(lastPosition.x + lastSize.x, firstChild.transform.position.y, firstChild.transform.position.z);

                        backgroundPart.Remove(firstChild);
                        backgroundPart.Add(firstChild);

                    }
                }
            }
        }
    }
}
