using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public enum CombatState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }
    public CombatState state;

    public Combatant playerCombatant;
    public Combatant enemyCombatant;

    void Update()
    {
        
        if (state == CombatState.PLAYER_TURN)
        {
            HandlePlayerClick();  
        }
    }

    public void StartBattle()
    {
        state = CombatState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        

        yield return new WaitForSeconds(1f);

        state = CombatState.PLAYER_TURN;  
        
    }

    
    private void HandlePlayerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.GetComponent<Combatant>() == enemyCombatant)
            {
                
                StartCoroutine(PlayerAttack());
            }
        }
    }

    IEnumerator PlayerAttack()
    {
        
        bool isEnemyDead = enemyCombatant.TakeDamage(playerCombatant.attackPower);

        yield return new WaitForSeconds(1f);  

        if (isEnemyDead)
        {
            state = CombatState.WON;
            EndBattle();
        }
        else
        {
            state = CombatState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        

        yield return new WaitForSeconds(1f);

        
        bool isPlayerDead = playerCombatant.TakeDamage(enemyCombatant.attackPower);

        yield return new WaitForSeconds(1f);

        if (isPlayerDead)
        {
            state = CombatState.LOST;
            EndBattle();
        }
        else
        {
            state = CombatState.PLAYER_TURN;
            
        }
    }

    void EndBattle()
    {
        if (state == CombatState.WON)
        {
            Debug.Log("คุณชนะการต่อสู้!");
        }
        else if (state == CombatState.LOST)
        {
            Debug.Log("คุณแพ้การต่อสู้...");
        }

        PlayerMovement playerMovement = playerCombatant.GetComponent<PlayerMovement>();
        playerMovement.ExitCombatMode();  
    }
}
