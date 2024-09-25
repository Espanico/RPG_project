using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Combat")]
    public float maxHealth;
    [SerializeField]
    private float currentHealth;
    public float attackDamage;
    public GameObject player;

    [Header("Movement")]
    public float moveSpeed;
    public LayerMask solidObjectLayer;
    public LayerMask playerLayer;
    public LayerMask interactableLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float dmg) {
        currentHealth -= dmg;
        if(currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    public void PerformAction() {
        float playerDistance = (player.transform.position - transform.position).magnitude;
        if(playerDistance > .46f) {
            RandomMovement();
        } else {
            Attack();
            Debug.Log("ENEMY ATTACK");
        }
    }

    private void RandomMovement() {
        bool walked = false;
        var targetPos = transform.position;
        while(!walked) {
            targetPos = transform.position;
            int number = Random.Range(0,5);
            switch (number)
            {
            case 0: 
                break;
            case 1: 
                targetPos.x += .32f;
                break;
            case 2:
                targetPos.x -= .32f;
                break;
            case 3:
                targetPos.y += .32f;
                break;
            case 4:
                targetPos.y -= .32f;
                break;
            }
            if(isWalkable(targetPos)) {
                walked = true;
            }
        }
        StartCoroutine(Move(targetPos));
    }

    private bool isWalkable(Vector3 targetPos) {
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectLayer | interactableLayer | playerLayer) != null) {
            return false;
        }
        return true;
    }

    IEnumerator Move(Vector3 targetPos) {
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
    }

    IEnumerator AttackMovement() {
        var startPosition = transform.position;
        var attackedPosition = player.transform.position;
        bool movingForward = true;

        while((attackedPosition - transform.position).sqrMagnitude > Mathf.Epsilon && movingForward) {
            transform.position = Vector3.MoveTowards(transform.position, attackedPosition, 3*moveSpeed * Time.deltaTime);
            yield return null;
        }
        movingForward = false;
        while((startPosition - transform.position).sqrMagnitude > Mathf.Epsilon && !movingForward) {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, 3*moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPosition;
    }

    private void Attack() {
        StartCoroutine(AttackMovement());
        player.GetComponent<Player>().TakeDamage(attackDamage);
    }
}
