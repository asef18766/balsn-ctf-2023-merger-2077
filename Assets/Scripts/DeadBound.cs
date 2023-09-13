using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBound : MonoBehaviour
{
    private new BoxCollider2D collider;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        this.collider = GetComponent<BoxCollider2D>();

        bool globalRound = false;
        while (true)
        {
            var hits = Physics2D.BoxCastAll(this.collider.bounds.center, this.collider.bounds.size, 0, Vector2.zero);
            bool thisRound = false;
            foreach (var h in hits)
            {
                if (h.collider.CompareTag(Item.ITEM_TAG))
                {
                    thisRound = true;
                    break;
                }
            }
            if (thisRound && globalRound)
            {
                // Die
                Debug.Log("game over");
            }
            globalRound = thisRound;
            yield return new WaitForSeconds(1);
        }
    }
}
