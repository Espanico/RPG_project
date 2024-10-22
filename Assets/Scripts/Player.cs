using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField]
    Equipment[] equipment;
    public Vector3 updatedPlayerPosition;
    public InputActionReference attack;
    public InputActionReference move;
    public InputActionReference look;
    public InputActionReference menu;
    public GameObject inventoryUI;
    private Animator animator;

    [Header("Movement")]
    public float moveSpeed;
    private bool isMoving;
    private bool heldMovement = false;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    private Vector2 _moveDirection;

    [SerializeField]
    private Vector2 input;

    [Header("Combat")]
    //public float startHealth;
    public float maxHealth;
    [SerializeField]
    private float currentHealth;
    public float startDamage;
    [SerializeField]
    private float damage;
    public GameObject aimedPositionSquare;
    public List<GameObject> colliderList = new List<GameObject>();

    void Awake()
    {
        updatedPlayerPosition = transform.position;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        damage = startDamage;

        UpdateStats();
    }
    

    void Update()
    {
        if(gameManager.enemiesPerformingAction == 0) {
            if(!isMoving && heldMovement) {
                input = move.action.ReadValue<Vector2>();

                if(input != Vector2.zero) {
                    var targetPos = transform.position;
                    targetPos.x += input.x;
                    targetPos.y += input.y;

                    aimedPositionSquare.transform.position = targetPos;

                    if(isWalkable(targetPos))  {
                        animator.SetFloat("vertical speed", 10*(targetPos.y-transform.position.y));
                        StartCoroutine(Move(targetPos));
                        EndTurn();
                    }
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
        updatedPlayerPosition = targetPos;
        isMoving = true;
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        animator.SetFloat("vertical speed", 0);
        isMoving = false;
        //EndTurn();
    }



    private void OnEnable() {
        attack.action.started += Interact;
        move.action.performed += Movement;
        move.action.canceled += LiftMovement;
        look.action.performed += Aim;
        menu.action.performed += Menu;
    }

    private void OnDisable() {
        attack.action.started -= Interact;
        move.action.performed -= Movement;
        move.action.canceled -= LiftMovement;
        look.action.performed -= Aim;
        menu.action.performed -= Menu;
    }

    private void Interact(InputAction.CallbackContext obj) {
        if(!isMoving) {
            int num = 0;
            if(colliderList.Count > 0) {
                for (int i = 0; i < colliderList.Count; i++)
                {
                    if(colliderList[i].tag == "Enemy") {
                        colliderList[i].GetComponent<Enemy>().TakeDamage(damage);
                        num++;
                    }
                }
                if(num==0) {
                    for (int i = 0; i < colliderList.Count; i++)
                    {
                        if(colliderList[i].tag == "Item") {
                            colliderList[i].GetComponent<ItemPickup>().PickUp();
                        }
                    }
                }
            }
            StartCoroutine(AttackMovement());
        }
    }

    private void Movement(InputAction.CallbackContext obj) {
        if(gameManager.enemiesPerformingAction == 0) {
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
                        EndTurn();
                    }
                }
            }
        }
        heldMovement = true;
    }

    private void LiftMovement(InputAction.CallbackContext obj) {
        heldMovement = false;
    }

    private void Aim(InputAction.CallbackContext obj) {
        if(gameManager.enemiesPerformingAction == 0 && !isMoving) {
            input = look.action.ReadValue<Vector2>();

            aimedPositionSquare.transform.position = transform.position + new Vector3(input.x, input.y);
        }
    }

    private void Menu(InputAction.CallbackContext obj) {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    IEnumerator AttackMovement() {
        isMoving = true;
        var startPosition = transform.position;
        var attackedPosition = (aimedPositionSquare.transform.position+startPosition)/2;
        bool movingForward = true;

        while((attackedPosition - transform.position).sqrMagnitude > Mathf.Epsilon && movingForward) {
            transform.position = Vector3.MoveTowards(transform.position, attackedPosition, 2*moveSpeed * Time.deltaTime);
            yield return null;
        }
        movingForward = false;
        while((startPosition - transform.position).sqrMagnitude > Mathf.Epsilon && !movingForward) {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, 2*moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPosition;
        isMoving = false;
        EndTurn();
    }

    private void EndTurn() {
        gameManager.PerformEnemiesTurn();
    }

    public void TakeDamage(float dmg) {
        currentHealth -= dmg;
        if(currentHealth <= 0) {
            Destroy(gameObject);
            Debug.Log("*** GAME OVER ***");
            Time.timeScale = 0;
        }
    }

    public void UpdateStats() {
        equipment = EquipmentManager.instance.currentEquipment;
        
        damage = startDamage;

        for (int i = 0; i < equipment.Length; i++) {
            if(equipment[i] != null) {
                damage += equipment[i].damageModifier;
            }
        }
    }
}
