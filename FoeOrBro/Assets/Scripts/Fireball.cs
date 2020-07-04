using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour, ISpell
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public int manaCost {get;set;}
    public int damage = 40;

    public Fireball(){
        manaCost = 14;
    }

    void Start()
    {
        manaCost = 14;
        Cast();
    }

    public void Cast(){
        rb.velocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo){
        ISelectable enemy = hitInfo.GetComponent<ISelectable>();
        if(enemy != null){
            Debug.Log("hit : " + enemy.Name());
            enemy.Damage(damage);
        }
        Destroy(gameObject);
    }

}
