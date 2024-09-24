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
    public GameObject aimedPositionSquare;
    private bool attackValue;
    public List<GameObject> colliderList = new List<GameObject>();

    void Awake()
    {
        //animator = GetComponent<Animator>();
        //currentHealth = maxHealth;
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
                    colliderList[i].GetComponent<Enemy>().takeDamage(5);
                }
            }
            EndTurn();
            Debug.Log("ATTACCO");
        }
    }

    private void EndTurn() {
        gameManager.GetComponent<GameManager>().enemiesTurn();
    }
}
