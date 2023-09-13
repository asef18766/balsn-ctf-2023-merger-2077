using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadBound : MonoBehaviour
{
    private new BoxCollider2D collider;

    private IEnumerator Start()
    {
        this.collider = GetComponent<BoxCollider2D>();
        List<GameObject> hitItems = new List<GameObject>();
        while (true)
        {
            var hits = Physics2D.BoxCastAll(this.collider.bounds.center, this.collider.bounds.size, 0, Vector2.zero);
            List<GameObject> tempHitItems = new List<GameObject>();
            foreach (var h in hits)
            {
                if (h.collider.CompareTag(Item.ITEM_TAG))
                {
                    if (hitItems.Contains(h.collider.gameObject))
                    {
                        Debug.Log("game over");
                        SceneManager.LoadScene("End");
                        yield return null;
                    }

                    tempHitItems.Add(h.collider.gameObject);
                }
            }
            hitItems = tempHitItems;
            yield return new WaitForSeconds(1);
        }
    }
}
