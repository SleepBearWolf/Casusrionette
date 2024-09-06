using UnityEngine;

public class Combatant : MonoBehaviour
{
    public string combatantName;
    public int maxHealth;
    public int currentHealth;
    public int attackPower;

    void Start()
    {
        currentHealth = maxHealth;  
    }

    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            
            return true; 
        }

        return false;
    }
}
