using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite sprite;
    public UIBar playerHealthBar;
    public UIBar playerManaBar;
    public string playerName = "Player";
    public bool isSelected { get;set;}

    // player stats
    public float maxHealth = 100;
    public int maxMana = 100;
    public float currentHealth { get;set;}
    public int currentMana;

    [SerializeField]
    public ISpell currentSpell;
    public GameObject projectile;
    public int Strength = 10, Dexterity = 10, Constitution = 10, Intelligence = 10, Wisdom = 10, Charisma = 10;
    public int experiencePoints = 0;
    private GameObject selectedTarget;

    void Start()
    {
        Fireball fb = new Fireball();
        currentSpell = fb;
        currentHealth = maxHealth;
        currentMana = maxMana;
        playerHealthBar.SetMaxValue(maxHealth);
        playerManaBar.SetMaxValue(maxMana);
        ChangeSprite();
    }
    void Update()
    {
        if(Input.GetKeyDown("r"))
        {
            if(TakeMana(currentSpell.manaCost))
                CastSpell();
        }
    }

    public void CastSpell(){
        Vector3 firePoint = new Vector3(transform.position.x +0.33f, transform.position.y, transform.position.z);
        Instantiate(projectile, firePoint, transform.rotation);
    }

    public void TakeDamage(float damage){
        currentHealth -= damage;
        playerHealthBar.SetValue(currentHealth);
        Debug.Log(currentHealth);
        if(currentHealth <= 0){
            Destroy(gameObject);
        }
    }
    public bool TakeMana(int damage){
        if(currentMana <= 0){
            Debug.Log("out of mana");
            return false;
        }
        if(currentMana < damage){
            Debug.Log("not enough mana");
            return false;
        }        
        currentMana -= damage;
        playerManaBar.SetValue(currentMana);
        Debug.Log("Spell cost: " + damage + "mana remaining : " + currentMana);
        return true;
    }

    void ChangeSprite()
    {
        spriteRenderer.sprite = sprite; 
    }

    public void Damage(float damageAmount){
        currentHealth -= damageAmount;
    }
    public string Name(){
        return name;
    }
}
