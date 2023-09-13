using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawn : MonoBehaviour
{
    const float SPAWN_COOLDOWN = 0.75f;

    [SerializeField]
    private int maxLevel;
    [SerializeField]
    private ItemRepository repository;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private float minX;
    [SerializeField]
    private float maxX;
    [SerializeField]
    private InputAction move;

    private Item item;

    void Start()
    {
        this.move.started += this.OnStartDrag;
        this.move.performed += this.OnDrag;
        this.move.canceled += this.OnEndDrag;
        this.move.Enable();

        StartCoroutine(this.SpawnCoroutine());
    }

    void Update()
    {
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            this.Spawn();
            yield return new WaitUntil(() => this.item == null);
            yield return new WaitForSeconds(ItemSpawn.SPAWN_COOLDOWN);
        }
    }


    private void OnStartDrag(InputAction.CallbackContext context)
    {
    }

    private void OnEndDrag(InputAction.CallbackContext context)
    {
        if (!this.item) return;

        this.item.GetComponent<Rigidbody2D>().simulated = true;
        this.item = null;
    }

    private void OnDrag(InputAction.CallbackContext context)
    {
        if (!this.item) return;

        Vector2 v = context.ReadValue<Vector2>();
        Vector2 wv = Camera.main.ScreenToWorldPoint(v);
        Vector3 newPos = this.item.transform.position;
        newPos.x = Mathf.Clamp(wv.x, this.minX, this.maxX);
        this.item.transform.position = newPos;
    }

    public void Spawn()
    {
        Debug.Assert(this.item == null);
        Vector3 spawnPos = new Vector3(0, transform.position.y, 0);
        this.item = Instantiate(this.itemPrefab, spawnPos, Quaternion.identity).GetComponent<Item>();
        Debug.Assert(this.item != null);
        this.item.GetComponent<Rigidbody2D>().simulated = false;
        StartCoroutine(this.UpdateItemData());
    }

    // Delay 1 frame to make sure the Item.Start is executed
    private IEnumerator UpdateItemData()
    {
        yield return new WaitForEndOfFrame();
        this.item.UpdateItemData(this.repository.GetItem(Random.Range(0, this.maxLevel + 1)));
    }
}
