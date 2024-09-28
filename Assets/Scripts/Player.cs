using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    private Animator animator;
    [Header("Movement")]
    public float moveSpeed;
    private bool isMoving;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    private Vector2 _moveDirection;
    public InputActionReference move;
    public InputActionReference attack;

    [SerializeField]
    private Vector2 input;

    [Header("Combat")]
    //public float startHealth;
    public float maxHealth;
    private float currentHealth;
    public float startDamage;
    private float damage;
    public GameObject aimedPositionSquare;
    public List<GameObject> colliderList = new List<GameObject>();

    void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        damage = startDamage;
    }
    

    void Update()
    {
        if(!isMoving) {
            input = move.action.ReadValue<Vector2>();

            if(input != Vector2.zero) {
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                aimedPositionSquare.transform.position = targetPos;

                if(isWalkable(targetPos))  {
                    animator.SetFloat("vertical speed", 10*(targetPos.y-transform.position.y));
                    StartCoroutine(Move(targetPos));
                }
            }
        }
    }

    private bool isWalkable(Vector3 targetPos) {
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectLayer | interactableLayer) != null) {
            return false;
        }
        return true;
    }

    IEnumerator Move(Vector3 targetPos) {
        isMoving = true;
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        animator.SetFloat("vertical speed", 0);
        isMoving = false;
        EndTurn();
    }



    private void OnEnable() {
        attack.action.started += Attack;
    }

    private void OnDisable() {
        attack.action.started -= Attack;
    }

    private void Attack(InputAction.CallbackContext obj) {
        if(!isMoving) {
            int num = colliderList.Count;
            if(num > 0) {
                for (int i = 0; i < num; i++)
                {
                    colliderList[i].GetComponent<Enemy>().TakeDamage(damage);
                }
            }
            StartCoroutine(AttackMovement());
        }
    }

    IEnumerator AttackMovement() {
        isMoving = true;
        var startPosition = transform.position;
        var attackedPosition = aimedPositionSquare.transform.position;
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
        isMoving = false;
        EndTurn();
    }

    private void EndTurn() {
        gameManager.GetComponent<GameManager>().enemiesTurn();
    }

    public void TakeDamage(float dmg) {
        currentHealth -= dmg;
        if(currentHealth <= 0) {
            Destroy(gameObject);
            Debug.Log("*** GAME OVER ***");
            Time.timeScale = 0;
        }
    }
}
