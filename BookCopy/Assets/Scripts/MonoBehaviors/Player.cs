using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    ///Field
    public HitPoints hitPoints; 
    public HealthBar healthBarPrefab;
    HealthBar healthBar;
    public Inventory inventoryPrefab;
    Inventory inventory;
    ///Event
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("CanBePickedUp"))
        {
            Item hitObject = other.gameObject.GetComponent<Consumable>().item;

            if (hitObject != null)
            {
                bool shoudDisappear = false;

                switch (hitObject.itemType)
                {
                    case Item.ItemType.COIN:
                        shoudDisappear = inventory.AddItem(hitObject);
                        break;
                    case Item.ItemType.HEALTH:
                        shoudDisappear = AdjustHitPoints(hitObject.quantity);
                        break;
                    default:
                        break;
                }

                if (shoudDisappear)
                {
                    other.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnEnable() {
        ResetCharacter();
    }
    ////Method
    public bool AdjustHitPoints(int amount)
    {
        if (hitPoints.value < maxHitPoints){

            hitPoints.value += amount;
            print("Adjusted hitpoints by: " + amount + ". New value: " + hitPoints.value);

            return true;
        }

        return false;
    }
    public override IEnumerator DamageCharacter(int damage, float interval)
    {
        while(true)
        {
            StartCoroutine(FlickerCharacter());
            
            hitPoints.value = hitPoints.value - damage;

            if (hitPoints.value <= float.Epsilon)
            {
                KillCharacter();
                break;
            }

            if (interval > float.Epsilon)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                break;
            }
        }
    }

    public override void KillCharacter()
    {
        base.KillCharacter();

        Destroy(healthBar.gameObject);
        Destroy(inventory.gameObject);
    }

    public override void ResetCharacter()
    {
        inventory = Instantiate(inventoryPrefab);
        healthBar = Instantiate(healthBarPrefab);
        healthBar.character = this;

        hitPoints.value = startingHitPoints;
    }
}

