using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGscript : MonoBehaviour {


	void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        
        Vector2 scale = transform.localScale;
        if (cameraSize.x >= cameraSize.y) { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        } else { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }
        
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 3f);
        transform.localScale = scale;
    }


	void Update () {
		/*Material mat = GetComponent<MeshRenderer>().material;
		Vector2 offset = mat.mainTextureOffset;
		offset.x = transform.position.x / transform.localScale.x;
		offset.y = transform.position.y / transform.localScale.y;

		mat.mainTextureOffset = offset;*/

	}
}
